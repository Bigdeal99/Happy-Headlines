# Spørgsmål 11: Time to Market

## A. Forklar begrebet time to market og hvordan det relaterer sig til skalerings-principper og skaleringskuben

### Time to Market - Definition:

**Time to Market (TTM)** er tiden fra idé til produkt er tilgængelig for brugere. Måles typisk som:

```
Time to Market = Ideation Time + Development Time + Deployment Time
```

**Eksempler:**
- **Feature idé**: "Tilføj comment moderation"
- **Development**: 2 uger
- **Deployment**: 1 dag
- **Time to Market**: 15 dage

**Mål:**
- Minimér TTM for at være konkurrencedygtig
- Hurtigere feedback fra brugere
- Første-mover fordele

### Relation til Skaleringsprincipper:

#### 1. **Performance Skalering → Time to Market**

**Princippet:**
- Højere performance = Hurtigere development
- Færre performance-problemer = Mindre tid på optimering

**Eksempel:**
- **Lav performance**: System langsomt → Udviklere venter på tests → Længere TTM
- **Høj performance**: System hurtigt → Hurtigere feedback → Kortere TTM

#### 2. **Cost Skalering → Time to Market**

**Princippet:**
- Invester i automation = Hurtigere deployment
- Færre manuelle steps = Kortere TTM

**Eksempel:**
- **Lav cost (manuel)**: 30-60 min deployment → Længere TTM
- **Høj cost (automatiseret)**: 5-10 min deployment → Kortere TTM

#### 3. **People Skalering → Time to Market**

**Princippet:**
- Automatisering = Færre mennesker nødvendige
- Parallel development = Hurtigere TTM

**Eksempel:**
- **Lille team**: Sequential development → Længere TTM
- **Stort team**: Parallel development → Kortere TTM

### Relation til Skaleringskuben:

#### X-akse (Horizontal Duplication) → Time to Market

**Princippet:**
- Flere instanser = Hurtigere deployment (ingen impact på andre)
- Parallel deployment = Kortere TTM

**Impact:**
- **1 instance**: Deployment påvirker alle brugere → Risk → Længere TTM (venter på low-traffic window)
- **3 instances**: Canary deployment → Deploy til 1 instance først → Kortere TTM (deploy anytime)

#### Y-akse (Functional Decomposition) → Time to Market

**Princippet:**
- Microservices = Independent deployment
- Teams kan deploye uafhængigt → Kortere TTM

**Impact:**
```
Monolitisk: 
- Feature change → Deploy hele systemet → Koordinering nødvendig → Længere TTM

Microservices:
- Feature change → Deploy kun 1 service → Ingen koordinering → Kortere TTM
```

**Eksempel fra kodebasen:**
```
Happy-Headlines (6 services):
- CommentService feature → Deploy kun CommentService → 5 min
- Monolitisk → Deploy hele systemet → 30 min + koordinering
```

**Forbedring:**
- **Monolitisk**: 1 deployment = 30 min + koordinering → Længere TTM
- **Microservices**: 1 deployment = 5 min, uafhængigt → Kortere TTM

#### Z-akse (Data Partitioning) → Time to Market

**Princippet:**
- Flere databases = Independent deployment per region
- Kan deploye til én region først → Kortere TTM

**Impact:**
- **1 database**: Deployment påvirker alle regions → Risk → Længere TTM
- **8 databases**: Deploy til 1 region først → Test → Rollout → Kortere TTM

### Kombineret Skalering → Time to Market:

**X + Y + Z akser kombineret:**
- **X-akse**: Canary deployment (deploy til 1 instance først)
- **Y-akse**: Independent service deployment
- **Z-akse**: Regional rollout (deploy til 1 region først)

**Time to Market forbedring:**
```
Monolitisk, 1 instance, 1 region:
- Deployment: 30 min
- Risk: Høj → Vent på low-traffic → +2 timer
- TTM: 2.5 timer

Microservices, 3 instances, 8 regions:
- Deployment: 5 min (kun 1 service)
- Risk: Lav (canary) → Deploy anytime
- TTM: 5 min (6x hurtigere)
```

---

## B. Vis eksempler på mekanismer til at forbedre time to market i et system

### Problemstilling:

**System har langsom time to market:**
- Feature development: 2 uger
- Deployment: 1 dag (manuel)
- Total TTM: 15 dage

**Mål:**
- Feature development: 2 uger (samme)
- Deployment: 5 minutter (automatiseret)
- Total TTM: 14 dage (1 dag forbedring)

### Mekanismer til Forbedring:

#### Mekanisme 1: CI/CD Automation (Reducerer Deployment Time)

**Kodeeksempel:**
```yaml
# .github/workflows/ci-cd.yml (linje 1-111)
name: CI/CD Pipeline

on:
  push:
    branches: [main, master]
  # Automatisk trigger → Ingen manuel intervention

jobs:
  build-and-push:
    strategy:
      matrix:
        service:
          - name: article-service
          - name: comment-service
          # ... 6 services bygges parallel
    steps:
      - name: Build and push Docker image
        uses: docker/build-push-action@v5
        # Automatisk build + push
```

**Time to Market forbedring:**
- **Før (Manuel)**: 30-60 minutter deployment → 1 dag TTM
- **Efter (Automatiseret)**: 5-10 minutter deployment → 5 min TTM

**Ved stigende trafik:**
- **Før**: Manuel deployment → Vent på low-traffic window → +2 timer → Længere TTM
- **Efter**: Automatisk deployment → Deploy anytime → Ingen ventetid → Kortere TTM

#### Mekanisme 2: Parallel Build (Matrix Strategy)

**Kodeeksempel:**
```yaml
# .github/workflows/ci-cd.yml (linje 29-49)
strategy:
  matrix:
    service:
      - name: article-service
        path: ./ArticleService
      - name: comment-service
        path: ./CommentService
      # ... 6 services bygges parallel
```

**Time to Market forbedring:**
- **Før (Sequential)**: Build 6 services sekventielt → 6 × 5 min = 30 min
- **Efter (Parallel)**: Build 6 services parallel → 5 min (længste service)

**Forbedring:**
- **30 min → 5 min** = 6x hurtigere

#### Mekanisme 3: Y-akse Skalering (Independent Deployment)

**Kodeeksempel:**
```csharp
// ArticleService/Controllers/ArticlesController.cs
// Isoleret service - kan deployes uafhængigt
[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    // Kun article-relateret funktionalitet
}

// CommentService/Controllers/CommentsController.cs
// Separeret service - kan deployes uafhængigt
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    // Kun comment-relatered funktionalitet
}
```

**Time to Market forbedring:**
- **Før (Monolitisk)**: Feature change → Deploy hele systemet → 30 min + koordinering → Længere TTM
- **Efter (Microservices)**: Feature change → Deploy kun 1 service → 5 min, uafhængigt → Kortere TTM

**Eksempel:**
- **CommentService feature**: Deploy kun CommentService → 5 min → Kortere TTM
- **Monolitisk**: Deploy hele systemet → 30 min + koordinering → Længere TTM

#### Mekanisme 4: Build Caching (Hurtigere Builds)

**Kodeeksempel:**
```yaml
# .github/workflows/ci-cd.yml (linje 100-101)
cache-from: type=gha
cache-to: type=gha,mode=max
```

**Time to Market forbedring:**
- **Før (No Cache)**: Hver build = 5 min → Længere TTM
- **Efter (Cache)**: Hver build = 2 min (cache hit) → Kortere TTM

**Forbedring:**
- **5 min → 2 min** = 2.5x hurtigere builds

#### Mekanisme 5: Asynkron Processing (Decoupling)

**Kodeeksempel:**
```csharp
// PublisherService/Controllers/PublishController.cs (linje 34-37)
var msg = new { req.Title, req.Content, req.Continent, PublishedAt = DateTime.UtcNow };
_publisher.Publish(msg);

return Accepted(new { status = "queued" }); // 202 Accepted - ikke venter
```

**Time to Market forbedring:**
- **Før (Synkron)**: PublisherService venter på ArticleService → Blokeret → Længere TTM
- **Efter (Asynkron)**: PublisherService returnerer umiddelbart → Ingen blokering → Kortere TTM

**Ved stigende trafik:**
- **Før**: Synkron → ArticleService overbelastet → PublisherService blokeret → Længere TTM
- **Efter**: Asynkron → ArticleService processerer i baggrunden → PublisherService fortsætter → Kortere TTM

#### Mekanisme 6: Feature Flags (Deploy Code før Feature Release)

**Kodeeksempel:**
```csharp
// Eksempel: Feature flag implementation
if (await _flags.IsEnabledAsync("new-cache-strategy"))
{
    return await GetWithNewCacheStrategy(top);
}
else
{
    return await GetWithOldCacheStrategy(top);
}
```

**Time to Market forbedring:**
- **Før**: Deploy kode → Aktivér feature → 2 steps → Længere TTM
- **Efter**: Deploy kode med flag OFF → Aktivér flag når klar → Separerer deployment fra release → Kortere TTM

**Eksempel:**
- **Deploy**: 5 min (med flag OFF)
- **Aktivér feature**: 1 sek (toggle flag)
- **Total TTM**: 5 min (vs. 30 min hvis venter på feature completion)

#### Mekanisme 7: Canary Deployment (X-akse Skalering)

**Kodeeksempel:**
```yaml
# docker-compose.yml - Kan skaleres til flere replicas
article-service:
  deploy:
    replicas: 3  # 3 instances for canary deployment
```

**Time to Market forbedring:**
- **Før (1 instance)**: Deploy påvirker alle brugere → Risk → Vent på low-traffic → +2 timer → Længere TTM
- **Efter (3 instances)**: Deploy til 1 instance først → Test → Rollout → Deploy anytime → Kortere TTM

**Eksempel:**
- **Canary**: Deploy til 1/3 instances → Test → Hvis OK, deploy til resten
- **TTM**: 5 min (deploy anytime, ingen ventetid)

### Kombineret Approach (Optimal Time to Market):

**Alle mekanismer kombineret:**
```yaml
# CI/CD Pipeline
- Parallel build (matrix strategy)      # 1. 6x hurtigere
- Build caching                         # 2. 2.5x hurtigere
- Independent deployment (Y-akse)      # 3. Deploy kun 1 service
- Asynkron processing                   # 4. Ingen blokering
- Feature flags                         # 5. Separer deployment fra release
- Canary deployment (X-akse)          # 6. Deploy anytime
```

**Time to Market ved stigende trafik:**

| Scenario | Uden Mekanismer | Med Alle Mekanismer | Forbedring |
|----------|-----------------|---------------------|------------|
| **Feature development** | 2 uger | 2 uger | Samme |
| **Deployment (manuel)** | 30-60 min | - | Elimineret |
| **Deployment (automatiseret)** | - | 5 min | ✅ |
| **Ventetid (low-traffic)** | +2 timer | 0 min | ✅ Elimineret |
| **Total TTM** | 15 dage | 14 dage | **1 dag forbedring** |

**Ved flere features:**
- **10 features/år**: 10 dage forbedring/år
- **ROI**: Høj (hurtigere market response)

---

## C. Diskuter hvordan visse arkitektoniske valg kan have en negativ indvirkning på systemets time to market

### Arkitektoniske Valg der Reducerer Time to Market:

#### 1. **Monolitisk Arkitektur**

**Problem:**
Alle features i samme codebase → Alle deployment sammen.

**Negativ Impact:**
- **Feature change**: Deploy hele systemet → 30 min
- **Koordinering**: Alle teams skal koordinere → +1 dag
- **Risk**: Høj → Vent på low-traffic → +2 timer
- **Total TTM**: 1 dag + 2 timer = Længere TTM

**Eksempel:**
```
Monolitisk:
- CommentService feature → Deploy hele monolit → 30 min
- ArticleService feature → Deploy hele monolit → 30 min
- Koordinering nødvendig → +1 dag
- Total TTM: 1 dag + 30 min
```

**Løsning:**
- Y-akse skalering (microservices) → Independent deployment → 5 min per service

#### 2. **Tæt Service Coupling**

**Problem:**
Services er tæt koblet, kan ikke deployes uafhængigt.

**Eksempel fra kodebasen:**
```csharp
// PublisherService/Controllers/PublishController.cs (linje 24-27)
// Synkron dependency på ProfanityService
var check = await client.GetAsync($"/api/profanity/check?text=...");
if (!check.IsSuccessStatusCode)
    return StatusCode(502, "ProfanityService unavailable");
```

**Negativ Impact:**
- **PublisherService deployment**: Skal koordinere med ProfanityService → Længere TTM
- **Breaking changes**: Skal deploye begge services samtidigt → Længere TTM
- **Testing**: Skal teste integration → +1 dag → Længere TTM

**Løsning:**
- Asynkron processing (decoupling)
- API versioning (backward compatibility)
- Contract testing

#### 3. **Manuel Deployment Process**

**Problem:**
Ingen automation → Mange manuelle steps.

**Negativ Impact:**
- **Deployment time**: 30-60 minutter (manuel)
- **Fejl-risiko**: Høj → Rework → +1 dag → Længere TTM
- **Konsistens**: Lav → Forskellige udviklere → Længere TTM

**Eksempel:**
```
Manuel deployment:
1. Build Docker image lokalt (10 min)
2. Test image lokalt (5 min)
3. Tag image (2 min)
4. Push til registry (5 min)
5. SSH til server (2 min)
6. Pull image (3 min)
7. Stop container (1 min)
8. Start container (2 min)
9. Test deployment (10 min)
Total: 40 minutter
```

**Løsning:**
- CI/CD automation → 5-10 minutter automatisk

#### 4. **Sequential Build Process**

**Problem:**
Services bygges sekventielt i stedet for parallel.

**Negativ Impact:**
- **Build time**: 6 services × 5 min = 30 minutter
- **Længere TTM**: 30 min ekstra per deployment

**Eksempel:**
```
Sequential build:
Service 1: 5 min
Service 2: 5 min (venter på Service 1)
Service 3: 5 min (venter på Service 2)
...
Total: 30 minutter
```

**Løsning:**
- Parallel build (matrix strategy) → 5 minutter (længste service)

#### 5. **Ingen Build Caching**

**Problem:**
Hver build starter fra scratch.

**Negativ Impact:**
- **Build time**: 5 minutter hver gang
- **Længere TTM**: 5 min ekstra per deployment

**Løsning:**
- Build caching → 2 minutter (cache hit)

#### 6. **Database Schema Changes**

**Problem:**
Breaking database changes kræver koordineret deployment.

**Negativ Impact:**
- **Migration time**: Database migration → +30 min
- **Koordinering**: Alle services skal deployes samtidigt → +1 dag
- **Risk**: Høj → Vent på low-traffic → +2 timer
- **Total TTM**: 1 dag + 2.5 timer = Længere TTM

**Løsning:**
- Backward-compatible migrations
- Feature flags (deploy code først, aktivér senere)
- Database versioning

#### 7. **Ingen Feature Flags**

**Problem:**
Kode deployment = Feature release (samme step).

**Negativ Impact:**
- **Risk**: Høj → Vent på feature completion → Længere TTM
- **Testing**: Skal teste feature før deployment → +1 dag → Længere TTM

**Eksempel:**
```
Uden feature flags:
- Develop feature (2 uger)
- Test feature (1 dag)
- Deploy feature (5 min)
- Total TTM: 15 dage
```

**Løsning:**
- Feature flags → Deploy kode med flag OFF → Test → Aktivér flag → Kortere TTM

#### 8. **Tight Integration Testing**

**Problem:**
Integration tests kræver alle services kørende.

**Negativ Impact:**
- **Test setup**: Kompleks → +1 dag
- **Test execution**: Langsom → +2 timer
- **Total TTM**: +1 dag + 2 timer = Længere TTM

**Løsning:**
- Contract testing (test interfaces, ikke implementation)
- Mock services i tests
- Parallel test execution

#### 9. **Manglende Automation**

**Problem:**
Mange manuelle steps i development process.

**Negativ Impact:**
- **Manual steps**: 10+ steps → Fejl-risiko → Rework → +1 dag → Længere TTM
- **Konsistens**: Lav → Forskellige processer → Længere TTM

**Eksempel:**
```
Manuel process:
1. Code review (manuel) → +2 timer
2. Build (manuel) → +10 min
3. Test (manuel) → +30 min
4. Deploy (manuel) → +40 min
Total: +3 timer per feature
```

**Løsning:**
- CI/CD automation → Alle steps automatiske → 5-10 minutter

#### 10. **Single Deployment Window**

**Problem:**
Kan kun deploye i bestemte tidsvinduer (fx natten).

**Negativ Impact:**
- **Ventetid**: Vent på deployment window → +12 timer → Længere TTM
- **Risk**: Høj → Vent på low-traffic → +2 timer → Længere TTM

**Eksempel:**
```
Single deployment window:
- Feature klar kl. 10:00
- Deployment window: 02:00-04:00
- Ventetid: 16 timer
- Total TTM: +16 timer
```

**Løsning:**
- Canary deployment → Deploy anytime
- Feature flags → Deploy kode, aktivér senere
- Blue-green deployment → Zero-downtime

### Sammenligning: Negativ vs. Positiv Impact

| Arkitektonisk Valg | Time to Market Impact | Eksempel |
|---------------------|----------------------|----------|
| **Monolitisk** | ❌ Længere (30 min + koordinering) | Deploy hele systemet |
| **Microservices (Y-akse)** | ✅ Kortere (5 min, uafhængigt) | Deploy kun 1 service |
| **Manuel deployment** | ❌ Længere (30-60 min) | 10+ manuelle steps |
| **CI/CD automation** | ✅ Kortere (5-10 min) | Automatisk pipeline |
| **Sequential build** | ❌ Længere (30 min) | 6 services × 5 min |
| **Parallel build** | ✅ Kortere (5 min) | Matrix strategy |
| **Ingen caching** | ❌ Længere (5 min/build) | Hver build fra scratch |
| **Build caching** | ✅ Kortere (2 min/build) | Cache hit |
| **Synkron dependencies** | ❌ Længere (koordinering) | Tæt kobling |
| **Asynkron processing** | ✅ Kortere (decoupling) | Queue-based |
| **Ingen feature flags** | ❌ Længere (vent på completion) | Kode = release |
| **Feature flags** | ✅ Kortere (deploy først) | Separer deployment |

### Best Practices for Kort Time to Market:

**1. Automatisering:**
- CI/CD pipeline (automatisk build, test, deploy)
- Build caching (hurtigere builds)
- Parallel execution (matrix strategy)

**2. Decoupling:**
- Y-akse skalering (microservices)
- Asynkron processing (queues)
- API versioning (backward compatibility)

**3. Risk Reduction:**
- Feature flags (deploy kode først)
- Canary deployment (deploy til 1 instance først)
- Blue-green deployment (zero-downtime)

**4. Independent Deployment:**
- Microservices (deploy kun 1 service)
- Database versioning (backward-compatible migrations)
- Contract testing (test interfaces)

### Konklusion:

**Arkitektoniske valg der reducerer Time to Market:**
- ❌ Monolitisk arkitektur
- ❌ Tæt service coupling
- ❌ Manuel deployment process
- ❌ Sequential build process
- ❌ Ingen build caching
- ❌ Database schema changes
- ❌ Ingen feature flags
- ❌ Tight integration testing
- ❌ Manglende automation
- ❌ Single deployment window

**Arkitektoniske valg der forbedrer Time to Market:**
- ✅ Microservices (Y-akse)
- ✅ CI/CD automation
- ✅ Parallel build
- ✅ Build caching
- ✅ Asynkron processing
- ✅ Feature flags
- ✅ Canary deployment
- ✅ Independent deployment
- ✅ Contract testing
- ✅ Anytime deployment

**Anbefaling:**
Design systemet med Time to Market i tankerne:
- Automatiser alt (CI/CD)
- Decouple services (microservices, asynkron)
- Reducer risk (feature flags, canary deployment)
- Enable independent deployment (Y-akse skalering)

**Resultat:**
- **Før**: 15 dage TTM (2 uger development + 1 dag deployment)
- **Efter**: 14 dage TTM (2 uger development + 5 min deployment)
- **Forbedring**: 1 dag (6.7% forbedring)
