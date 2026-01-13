# Spørgsmål 6: Recovery

## A. Forklar hvilke muligheder feature flags giver i deployment-processen

### Feature Flags - Definition:

Feature flags (også kaldet feature toggles) er mekanismer der gør det muligt at aktivere/deaktivere funktionalitet i runtime uden at redeploye koden.

### Muligheder i Deployment-processen:

#### 1. **Gradual Rollout (Canary Deployment)**
- **Mulighed**: Aktivér feature for en lille procentdel af brugere først
- **Værdi**: Test i production med minimal risiko
- **Eksempel**: Aktivér ny comment-funktionalitet for 5% af brugere → Monitor → Skaler til 100%

#### 2. **Instant Rollback uden Redeployment**
- **Mulighed**: Sluk feature med ét klik i stedet for at redeploye
- **Værdi**: Hurtig recovery (sekunder vs. minutter)
- **Eksempel**: Ny feature forårsager fejl → Sluk feature flag → Systemet fungerer igen

#### 3. **A/B Testing**
- **Mulighed**: Test forskellige versioner af samme feature
- **Værdi**: Data-driven beslutninger om hvilken version der fungerer bedst
- **Eksempel**: Test to forskellige cache-strategier og sammenlign performance

#### 4. **Environment-specific Features**
- **Mulighed**: Aktivér features kun i bestemte miljøer (staging, production)
- **Værdi**: Test features i staging før production
- **Eksempel**: Ny newsletter-feature kun i staging → Test → Aktivér i production

#### 5. **Emergency Kill Switch**
- **Mulighed**: Sluk hele features ved kritiske fejl
- **Værdi**: Hurtig mitigation af production incidents
- **Eksempel**: ProfanityService fejler → Sluk profanity check feature flag → Systemet fortsætter

#### 6. **Deploy Code før Feature Release**
- **Mulighed**: Deploy kode med feature flag OFF, aktivér senere
- **Værdi**: Separerer deployment fra feature release
- **Eksempel**: Deploy ny artikel-cache strategi med flag OFF → Test → Aktivér når klar

#### 7. **User Segmentation**
- **Mulighed**: Aktivér features for specifikke brugergrupper
- **Værdi**: Test med beta-brugere først
- **Eksempel**: Ny comment-feature kun for premium brugere → Test → Udvid til alle

### Eksempel Implementation:

```csharp
// ArticleService - Feature flag eksempel
public class ArticlesController : ControllerBase
{
    private readonly IFeatureFlags _flags;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int top = 5)
    {
        // Feature flag: Ny cache strategi
        if (await _flags.IsEnabledAsync("new-cache-strategy"))
        {
            return await GetWithNewCacheStrategy(top);
        }
        else
        {
            return await GetWithOldCacheStrategy(top);
        }
    }
}
```

**Deployment flow med feature flag:**
1. Deploy kode med `new-cache-strategy` flag = OFF
2. Systemet kører med gammel strategi (ingen risiko)
3. Aktivér flag for 10% trafik → Monitor
4. Hvis OK → Aktivér for 100%
5. Hvis fejl → Sluk flag (instant rollback)

---

## B. Skitser en fremgangsmåde til implementering af rollback mekanismer i en CI/CD-pipeline

### Rollback Strategi:

**Principper:**
1. **Version Tagging**: Alle images tagges med SHA for nem rollback
2. **Health Checks**: Automatisk rollback ved health check failures
3. **Monitoring**: Rollback trigger baseret på metrics
4. **Database Rollback**: Rollback scripts for database migrations

### Pipeline Implementation:

```yaml
# .github/workflows/ci-cd.yml - Udvidet med rollback

name: CI/CD Pipeline with Rollback

on:
  push:
    branches: [main, master]
  workflow_dispatch:
    inputs:
      rollback_to:
        description: 'SHA to rollback to'
        required: false

env:
  REGISTRY: ghcr.io
  IMAGE_PREFIX: ${{ github.repository_owner }}/happy-headlines
  CURRENT_SHA: ${{ github.sha }}

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    outputs:
      image_tags: ${{ steps.tags.outputs.tags }}
      image_sha: ${{ github.sha }}
    steps:
      # ... eksisterende build steps ...
      - name: Generate version tags
        id: tags
        run: |
          SHORT_SHA=$(echo "${{ github.sha }}" | cut -c1-7)
          TAGS="${{ env.REGISTRY }}/${{ env.IMAGE_PREFIX }}/article-service:${{ github.sha }},${{ env.REGISTRY }}/${{ env.IMAGE_PREFIX }}/article-service:${SHORT_SHA}"
          echo "tags=${TAGS}" >> $GITHUB_OUTPUT

  deploy-staging:
    needs: build-and-push
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to staging
        run: |
          kubectl set image deployment/article-service \
            article-service=${{ needs.build-and-push.outputs.image_tags }} \
            --namespace=staging
      
      - name: Wait for deployment
        run: |
          kubectl rollout status deployment/article-service --namespace=staging --timeout=5m

      - name: Health check validation
        id: healthcheck
        run: |
          max_attempts=30
          for i in $(seq 1 $max_attempts); do
            if curl -f http://staging.article-service:8080/health; then
              echo "healthy=true" >> $GITHUB_OUTPUT
              exit 0
            fi
            sleep 2
          done
          echo "healthy=false" >> $GITHUB_OUTPUT
          exit 1

      - name: Rollback on health check failure
        if: steps.healthcheck.outputs.healthy == 'false'
        run: |
          echo "Health check failed - rolling back"
          kubectl rollout undo deployment/article-service --namespace=staging
          exit 1

  smoke-tests:
    needs: deploy-staging
    runs-on: ubuntu-latest
    steps:
      - name: Run smoke tests
        run: |
          ./scripts/smoke-tests.sh staging.article-service:8080
        continue-on-error: false

  deploy-production:
    needs: [deploy-staging, smoke-tests]
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    steps:
      - name: Backup current version
        id: backup
        run: |
          CURRENT_VERSION=$(kubectl get deployment/article-service -n production -o jsonpath='{.spec.template.spec.containers[0].image}')
          echo "previous_version=${CURRENT_VERSION}" >> $GITHUB_OUTPUT
          echo "Backed up previous version: ${CURRENT_VERSION}"

      - name: Deploy to production
        run: |
          kubectl set image deployment/article-service \
            article-service=${{ needs.build-and-push.outputs.image_tags }} \
            --namespace=production

      - name: Wait for deployment
        run: |
          kubectl rollout status deployment/article-service --namespace=production --timeout=10m

      - name: Health check validation
        id: prod_healthcheck
        run: |
          max_attempts=60
          for i in $(seq 1 $max_attempts); do
            if curl -f http://production.article-service:8080/health; then
              echo "healthy=true" >> $GITHUB_OUTPUT
              exit 0
            fi
            sleep 2
          done
          echo "healthy=false" >> $GITHUB_OUTPUT
          exit 1

      - name: Monitor error rate
        id: monitor
        run: |
          # Check error rate from Prometheus
          ERROR_RATE=$(curl -s http://prometheus:9090/api/v1/query?query=rate(http_requests_total{status=~"5.."}[5m]) | jq -r '.data.result[0].value[1]')
          THRESHOLD=0.05  # 5% error rate
          
          if (( $(echo "$ERROR_RATE > $THRESHOLD" | bc -l) )); then
            echo "error_rate_high=true" >> $GITHUB_OUTPUT
            exit 1
          else
            echo "error_rate_high=false" >> $GITHUB_OUTPUT
          fi

      - name: Automatic rollback on failure
        if: steps.prod_healthcheck.outputs.healthy == 'false' || steps.monitor.outputs.error_rate_high == 'true'
        run: |
          echo "Deployment failed - rolling back to previous version"
          kubectl rollout undo deployment/article-service --namespace=production
          
          # Wait for rollback to complete
          kubectl rollout status deployment/article-service --namespace=production --timeout=10m
          
          # Verify rollback
          if curl -f http://production.article-service:8080/health; then
            echo "Rollback successful"
          else
            echo "Rollback failed - manual intervention required"
            exit 1
          fi

  manual-rollback:
    if: github.event_name == 'workflow_dispatch' && github.event.inputs.rollback_to != ''
    runs-on: ubuntu-latest
    steps:
      - name: Manual rollback to specific SHA
        run: |
          ROLLBACK_SHA=${{ github.event.inputs.rollback_to }}
          kubectl set image deployment/article-service \
            article-service=${{ env.REGISTRY }}/${{ env.IMAGE_PREFIX }}/article-service:${ROLLBACK_SHA} \
            --namespace=production
          
          kubectl rollout status deployment/article-service --namespace=production --timeout=10m
          
          echo "Rolled back to SHA: ${ROLLBACK_SHA}"
```

### Rollback Mekanismer:

#### 1. **Automatic Rollback på Health Check Failure**

```yaml
- name: Health check validation
  id: healthcheck
  run: |
    max_attempts=30
    for i in $(seq 1 $max_attempts); do
      if curl -f http://service:8080/health; then
        echo "healthy=true" >> $GITHUB_OUTPUT
        exit 0
      fi
      sleep 2
    done
    echo "healthy=false" >> $GITHUB_OUTPUT
    exit 1

- name: Rollback on health check failure
  if: steps.healthcheck.outputs.healthy == 'false'
  run: |
    kubectl rollout undo deployment/article-service
```

**Fra FMEA dokumentation:**
```yaml
# docs/FMEA-CICD-Risk-Analysis.md (linje 207-221)
- name: Wait for Service Health
  run: |
    max_attempts=30
    for i in $(seq 1 $max_attempts); do
      if curl -f http://service:8080/health; then
        echo "Service is healthy"
        exit 0
      fi
      sleep 2
    done
    echo "Service failed health check"
    exit 1
```

#### 2. **Automatic Rollback på Error Rate Threshold**

```yaml
- name: Monitor error rate
  id: monitor
  run: |
    ERROR_RATE=$(curl -s http://prometheus:9090/api/v1/query?query=rate(http_requests_total{status=~"5.."}[5m]))
    THRESHOLD=0.05  # 5% error rate
    
    if (( $(echo "$ERROR_RATE > $THRESHOLD" | bc -l) )); then
      echo "error_rate_high=true" >> $GITHUB_OUTPUT
      exit 1
    fi

- name: Automatic rollback on high error rate
  if: steps.monitor.outputs.error_rate_high == 'true'
  run: |
    kubectl rollout undo deployment/article-service
```

#### 3. **Manual Rollback til Specifik Version**

```yaml
# Trigger: workflow_dispatch med rollback_to input
- name: Manual rollback to specific SHA
  run: |
    ROLLBACK_SHA=${{ github.event.inputs.rollback_to }}
    kubectl set image deployment/article-service \
      article-service=${{ env.REGISTRY }}/${{ env.IMAGE_PREFIX }}/article-service:${ROLLBACK_SHA}
```

#### 4. **Database Migration Rollback**

```yaml
- name: Backup database before migration
  run: |
    # Backup database
    kubectl exec -it article-db -- /opt/mssql-tools/bin/sqlcmd \
      -S localhost -U sa -P $DB_PASSWORD \
      -Q "BACKUP DATABASE Articles TO DISK='/backup/articles-${GITHUB_SHA}.bak'"

- name: Run database migration
  run: |
    dotnet ef database update --project ArticleService

- name: Rollback database on failure
  if: failure()
  run: |
    # Restore from backup
    kubectl exec -it article-db -- /opt/mssql-tools/bin/sqlcmd \
      -S localhost -U sa -P $DB_PASSWORD \
      -Q "RESTORE DATABASE Articles FROM DISK='/backup/articles-${GITHUB_SHA}.bak' WITH REPLACE"
```

### Rollback Flow Diagram:

```
Deploy New Version
    ↓
Wait for Deployment (5-10 min)
    ↓
Health Check (30 attempts × 2 sec = 60 sec)
    ↓
┌─────────────────┐
│ Health OK?      │
└─────────────────┘
    │        │
   YES      NO
    │        │
    ↓        ↓
Monitor    ROLLBACK
Error Rate   (automatic)
    ↓
┌─────────────────┐
│ Error < 5%?     │
└─────────────────┘
    │        │
   YES      NO
    │        │
    ↓        ↓
SUCCESS   ROLLBACK
          (automatic)
```

### Rollback Barriers (fra FMEA):

**Barrier 1: Version Tagging**
- Alle images tagges med SHA → Nem rollback til specifik version
- Eksempel: `article-service:abc123def456`

**Barrier 2: Health Check Validation**
- Automatisk rollback hvis health check fejler
- Eksempel: Service ikke healthy efter 60 sekunder → Rollback

**Barrier 3: Error Rate Monitoring**
- Automatisk rollback hvis error rate > threshold
- Eksempel: Error rate > 5% → Rollback

**Barrier 4: Database Backup**
- Backup før migration → Rollback muligt
- Eksempel: Restore fra backup ved migration fejl

---

## C. Sammenlign principperne "Design to be disabled" og "Design for rollback" med fokus på hvor de hver især kan anvendes

### Sammenligning:

| Aspekt | Design to be disabled | Design for rollback |
|--------|----------------------|---------------------|
| **Formål** | Sluk funktionalitet i runtime | Gå tilbage til tidligere version |
| **Granularitet** | Feature-niveau | System/version-niveau |
| **Hastighed** | Sekunder (instant) | Minutter (redeployment) |
| **Scope** | Specifik feature | Hele deployment |
| **Data impact** | Ingen (feature slukket) | Kan påvirke data (hvis migration) |
| **Use case** | Feature fejl, A/B testing | System-wide fejl, breaking changes |

### "Design to be disabled" - Anvendelse:

#### Hvor det anvendes:

**1. Feature-level Problems**
- En specifik feature forårsager problemer
- Anden funktionalitet fungerer fint
- **Eksempel**: Ny comment-feature forårsager fejl → Sluk feature flag → Kommentar-systemet fungerer igen

**2. A/B Testing**
- Test forskellige implementationer
- Skift mellem versioner i runtime
- **Eksempel**: Test to cache-strategier → Skift mellem dem med feature flags

**3. Gradual Rollout**
- Aktivér feature for få brugere først
- Skaler gradvist op
- **Eksempel**: Ny newsletter-feature for 10% → 50% → 100%

**4. Emergency Kill Switch**
- Hurtig mitigation af kritiske fejl
- Ingen redeployment nødvendig
- **Eksempel**: ProfanityService fejler → Sluk profanity check → Systemet fortsætter

#### Eksempel fra kodebasen:

```csharp
// CommentService - Design to be disabled eksempel
public class CommentsController : ControllerBase
{
    private readonly IProfanityClient _profanity;
    private readonly IFeatureFlags _flags;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Comment comment)
    {
        // Feature flag: Profanity check kan slås fra
        if (await _flags.IsEnabledAsync("profanity-check"))
        {
            var hasProfanity = await _profanity.ContainsProfanity(comment.Text);
            if (hasProfanity) return BadRequest("Comment rejected");
        }
        // Hvis flag slukket → Skip profanity check

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return Ok(comment);
    }
}
```

**Fordele:**
- ✅ Instant recovery (sekunder)
- ✅ Ingen redeployment
- ✅ Granular kontrol (kun én feature)
- ✅ Ingen data impact

**Begrænsninger:**
- ❌ Kræver feature flag infrastructure
- ❌ Kun for features designet til at være disabled
- ❌ Ikke for breaking changes i core system

### "Design for rollback" - Anvendelse:

#### Hvor det anvendes:

**1. System-wide Failures**
- Hele deployment forårsager problemer
- Alle features påvirkes
- **Eksempel**: Ny version har breaking change → Rollback til forrige version

**2. Database Migration Failures**
- Migration fejler eller korrumperer data
- **Eksempel**: Schema migration fejler → Rollback database + deployment

**3. Infrastructure Changes**
- Changes i infrastructure (networking, config)
- **Eksempel**: Ny configuration forårsager connectivity issues → Rollback

**4. Breaking API Changes**
- API changes der bryder integrationer
- **Eksempel**: ArticleService API breaking change → Rollback til kompatibel version

#### Eksempel fra kodebasen:

```yaml
# CI/CD Pipeline - Design for rollback
# Alle images tagges med SHA for nem rollback
- name: Generate version tags
  run: |
    TAGS="article-service:${{ github.sha }},article-service:${SHORT_SHA}"
    # Previous version: article-service:abc123 (kan rollback til)

# Automatic rollback på health check failure
- name: Rollback on health check failure
  if: steps.healthcheck.outputs.healthy == 'false'
  run: |
    kubectl rollout undo deployment/article-service
    # Roller tilbage til forrige version automatisk
```

**Fordele:**
- ✅ Håndterer system-wide failures
- ✅ Håndterer database migrations
- ✅ Håndterer breaking changes
- ✅ Standard Kubernetes/Docker pattern

**Begrænsninger:**
- ❌ Langsommere (minutter vs. sekunder)
- ❌ Kræver redeployment
- ❌ Kan påvirke data (hvis migration)
- ❌ Roller hele systemet tilbage (ikke granular)

### Kombineret Approach (Best Practice):

**Brug begge principper sammen:**

```csharp
// Design to be disabled (Feature flags)
if (await _flags.IsEnabledAsync("new-feature"))
{
    // Ny feature
}
else
{
    // Fallback til gammel feature
}

// Design for rollback (Version tagging)
// CI/CD pipeline:
// - Tag images med SHA
// - Automatic rollback på health check failure
// - Manual rollback til specifik version
```

**Anvendelses-scenarie:**

1. **Deploy ny version** med feature flag OFF
2. **Aktivér feature flag** for 10% trafik
3. **Monitor** - Hvis fejl:
   - **Feature-level**: Sluk feature flag (instant)
   - **System-level**: Rollback deployment (hvis hele systemet fejler)

### Konklusion:

| Scenario | Anvend Princippet |
|----------|-------------------|
| **Feature fejler** | Design to be disabled (feature flag) |
| **System fejler** | Design for rollback (deployment rollback) |
| **A/B testing** | Design to be disabled (feature flags) |
| **Database migration fejler** | Design for rollback (database + deployment) |
| **Breaking API change** | Design for rollback (deployment rollback) |
| **Emergency kill switch** | Design to be disabled (feature flag) |

**Anbefaling**: Implementer begge principper:
- **Feature flags** for granular, instant control
- **Rollback mechanisms** for system-wide recovery
