# Google Slides Content - Spørgsmål 5: Automation og DevOps
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Automation og DevOps
**Undertitel:** Automation over people i skaleringsprincipper
**Footer:** Dit navn | Dato

---

## SLIDE 2: Venn-diagrammet for Skaleringsprincipper
**Titel:** Venn-diagrammet for Skaleringsprincipper

**Indhold:**
- **Diagram:** Tre overlappende cirkler
  - Venstre: Performance
  - Højre: Cost
  - Nederst: People
- **Midten (overlap):** "Automation over people"

**Note:** Tegn diagram med tre overlappende cirkler

---

## SLIDE 3: Performance (Skalerbarhed)
**Titel:** Performance (Skalerbarhed)

**Indhold:**
- Automation gør det muligt at skaleres hurtigt og konsistent
- Eliminerer menneskelige fejl der kan påvirke performance
- Gør det muligt at deploye oftere → hurtigere feedback loops
- Konsistent deployment → forudsigelig performance

---

## SLIDE 4: Cost (Økonomi)
**Titel:** Cost (Økonomi)

**Indhold:**
- Automation reducerer manuel arbejdstid → lavere omkostninger
- Færre fejl → færre production incidents → lavere cost
- Bedre resource utilization → optimeret cost
- Skalerer uden at skalere team størrelse → cost-efficient skalering

---

## SLIDE 5: People (Organisatorisk)
**Titel:** People (Organisatorisk)

**Indhold:**
- Automation frigør mennesker til højere-værdi opgaver
- Reducerer stress og burnout (færre manuelle tasks)
- Gør det muligt for teams at skaleres organisatorisk
- Konsistent process → mindre afhængighed af individuelle eksperter

---

## SLIDE 6: Hvorfor i midten?
**Titel:** Hvorfor i midten?

**Indhold:**
- Automation er fundamentet for alle tre dimensioner
- **Uden automation:** Performance, Cost og People skalering er alle begrænsede
- **Med automation:** Alle tre dimensioner kan skaleres samtidigt

---

## SLIDE 7: Eksempel - Automation Impact
**Titel:** Eksempel - Automation Impact

**Indhold:**
- **Performance skalering:** Automation gør det muligt at deploye 10x oftere
- **Cost skalering:** Automation reducerer deployment cost med 80%
- **People skalering:** Automation gør det muligt at håndtere 5x flere services med samme team

---

## SLIDE 8: Konklusion - Venn-diagrammet
**Titel:** Konklusion - Venn-diagrammet

**Indhold:**
- Automation er en enabler for alle tre skaleringstyper
- Derfor er den i midten af Venn-diagrammet
- **Uden automation:** Begrænsede muligheder
- **Med automation:** Alle dimensioner kan skaleres

---

## SLIDE 9: Før - Manuel Deployment Workflow
**Titel:** Før - Manuel Deployment Workflow

**Indhold:**
- **Manuelle steps:**
  1. Developer committer kode ✅
  2. Build Docker image lokalt ❌
  3. Test image lokalt ❌
  4. Tag image med version ❌
  5. Push til registry ❌
  6. SSH til server ❌
  7. Pull image ❌
  8. Stop container ❌
  9. Start ny container ❌
  10. Test deployment ❌
  11. Rollback hvis fejl ❌

---

## SLIDE 10: Problemer med Manuel Workflow
**Titel:** Problemer med Manuel Workflow

**Indhold:**
- 10+ manuelle steps
- Fejl-risiko ved hvert step
- Konsistens-problemer (forskellige udviklere)
- Lang deployment tid (30-60 minutter)
- Ingen audit trail

---

## SLIDE 11: Efter - Automatiseret CI/CD Pipeline
**Titel:** Efter - Automatiseret CI/CD Pipeline

**Indhold:**
- **Fil:** `.github/workflows/ci-cd.yml`
- **Kode:**
  ```yaml
  name: CI/CD Pipeline
  on:
    push:
      branches: [main, master]
    pull_request:
      branches: [main, master]
    workflow_dispatch:
  ```
- **Resultat:** Automatisk trigger ved push

**Screenshot:** Vis linje 1-16 (name, on triggers)

---

## SLIDE 12: Pipeline - Matrix Strategy
**Titel:** Pipeline - Matrix Strategy

**Indhold:**
- **Fil:** `.github/workflows/ci-cd.yml`
- **Kode:**
  ```yaml
  strategy:
    matrix:
      service:
        - name: article-service
        - name: comment-service
        # ... 6 services
  ```
- **Resultat:** Parallel build af alle services

**Screenshot:** Vis linje 29-49 (strategy matrix)

---

## SLIDE 13: Pipeline - Automatiseret Steps
**Titel:** Pipeline - Automatiseret Steps

**Indhold:**
- **Fil:** `.github/workflows/ci-cd.yml`
- **Steps:**
  1. Checkout code ✅
  2. Setup Docker Buildx ✅
  3. Login to GHCR ✅
  4. Generate version tags ✅
  5. Build and push ✅
- **Resultat:** Alle steps automatiseret

**Screenshot:** Vis linje 51-101 (steps)

---

## SLIDE 14: Eliminerede Manuel Steps
**Titel:** Eliminerede Manuel Steps

**Indhold:**
| Manuel Step | Automatiseret i Pipeline |
|-------------|--------------------------|
| Build Docker image lokalt | ✅ docker/build-push-action@v5 |
| Tag image med version | ✅ Generate version tags step |
| Push til registry | ✅ Build and push step |
| SSH til server | ✅ Ikke nødvendig (container registry) |
| Pull image | ✅ Automatisk via registry |
| Stop/start container | ✅ (Kan tilføjes: deployment step) |

---

## SLIDE 15: Pipeline Flow
**Titel:** Pipeline Flow

**Indhold:**
```
Developer Push → GitHub
    ↓
[AUTOMATISERET] Trigger pipeline
    ↓
[AUTOMATISERET] Checkout code
    ↓
[AUTOMATISERET] Setup Docker Buildx
    ↓
[AUTOMATISERET] Login to GHCR
    ↓
[AUTOMATISERET] Generate version tags
    ↓
[AUTOMATISERET] Build 6 Docker images (parallel)
    ↓
[AUTOMATISERET] Push to GHCR
    ↓
✅ Deployment klar
```

---

## SLIDE 16: Resultat - Før vs Efter
**Titel:** Resultat - Før vs Efter

**Indhold:**
| Metrik | Før (Manuel) | Efter (Automatiseret) |
|--------|--------------|----------------------|
| Tid | 30-60 minutter | 5-10 minutter |
| People | 1-2 udviklere | 0 udviklere |
| Fejl-risiko | Høj (10+ steps) | Lav (konsistent) |
| Konsistens | Lav (forskellige) | Høj (samme process) |

---

## SLIDE 17: Build Summary Job
**Titel:** Build Summary Job

**Indhold:**
- **Fil:** `.github/workflows/ci-cd.yml`
- **Kode:**
  ```yaml
  build-summary:
    runs-on: ubuntu-latest
    needs: build-and-push
    steps:
      - name: Build Summary
        run: |
          echo "## 🚀 Build Summary"
  ```
- **Resultat:** Automatisk summary af alle services

**Screenshot:** Vis linje 112-149 (build-summary job)

---

## SLIDE 18: DevOps Principper
**Titel:** DevOps Principper

**Indhold:**
- **Liste:**
  1. Automation - Automatiser repetitive tasks
  2. Continuous Integration - Integrer kode ofte
  3. Continuous Deployment - Deploy automatisk
  4. Monitoring - Overvåg kontinuerligt
  5. Infrastructure as Code - Versioner infrastruktur
  6. Collaboration - Tæt samarbejde mellem Dev og Ops

---

## SLIDE 19: E-commerce (fx Amazon, Zalando)
**Titel:** E-commerce (fx Amazon, Zalando)

**Indhold:**
- **DevOps Anvendelse:**
  - CI/CD: Deploy flere gange dagligt (Black Friday skalering)
  - Automation: Auto-scaling baseret på trafik
  - Monitoring: Real-time metrics for salg og performance
- **Værdi:**
  - ✅ Hurtigere time-to-market for nye features
  - ✅ Kan håndtere trafik-spikes (Black Friday)
  - ✅ Reducerer downtime → tabt salg

---

## SLIDE 20: Finans (fx Banker, Fintech)
**Titel:** Finans (fx Banker, Fintech)

**Indhold:**
- **DevOps Anvendelse:**
  - CI/CD: Automatiseret testing og compliance checks
  - Security: Automated security scanning i pipeline
  - Audit: Alle deployment actions logges
- **Værdi:**
  - ✅ Compliance (GDPR, PCI-DSS) automatiseret
  - ✅ Security vulnerabilities opdages tidligt
  - ✅ Audit trail for regulatorer

---

## SLIDE 21: Healthcare (fx Sygehuse, Medicinsk Software)
**Titel:** Healthcare (fx Sygehuse, Medicinsk Software)

**Indhold:**
- **DevOps Anvendelse:**
  - CI/CD: Automatiseret testing af kritiske systemer
  - Monitoring: Real-time health monitoring
  - Rollback: Hurtig rollback ved fejl
- **Værdi:**
  - ✅ Patient safety (hurtig fejl-detektion)
  - ✅ Compliance (HIPAA, GDPR)
  - ✅ Zero-downtime deployments

---

## SLIDE 22: Manufacturing (fx Produktion, IoT)
**Titel:** Manufacturing (fx Produktion, IoT)

**Indhold:**
- **DevOps Anvendelse:**
  - CI/CD: Deploy firmware updates automatisk
  - Monitoring: IoT device monitoring
  - Infrastructure as Code: Versioner device configurations
- **Værdi:**
  - ✅ OTA (Over-The-Air) updates til devices
  - ✅ Centraliseret device management
  - ✅ Hurtigere bug fixes i produktion

---

## SLIDE 23: Media & Entertainment (fx Netflix, Spotify)
**Titel:** Media & Entertainment (fx Netflix, Spotify)

**Indhold:**
- **DevOps Anvendelse:**
  - CI/CD: Deploy nye features ofte
  - A/B Testing: Automatiseret feature flags
  - Monitoring: Real-time streaming metrics
- **Værdi:**
  - ✅ Hurtigere content delivery
  - ✅ Bedre user experience (A/B testing)
  - ✅ Kan håndtere globale trafik-spikes

---

## SLIDE 24: Government & Public Sector
**Titel:** Government & Public Sector

**Indhold:**
- **DevOps Anvendelse:**
  - CI/CD: Automatiseret compliance checks
  - Security: Automated security scanning
  - Audit: Komplet audit trail
- **Værdi:**
  - ✅ Transparency (alle changes logges)
  - ✅ Security (automated scanning)
  - ✅ Compliance (regulatory requirements)

---

## SLIDE 25: Fælles Mønstre på tværs af Brancher
**Titel:** Fælles Mønstre på tværs af Brancher

**Indhold:**
| DevOps Princip | Alle Brancher | Branche-specifik Værdi |
|----------------|---------------|------------------------|
| Automation | ✅ Reducerer fejl | E-commerce: Hurtigere features<br>Finans: Compliance<br>Healthcare: Patient safety |
| CI/CD | ✅ Hurtigere deployment | Alle: Time-to-market<br>Finans: Security<br>Healthcare: Zero-downtime |
| Monitoring | ✅ Proaktiv problem-detektion | E-commerce: Salg-metrics<br>Healthcare: Patient monitoring<br>Manufacturing: Device health |

---

## SLIDE 26: Konklusion - Brancher
**Titel:** Konklusion - Brancher

**Indhold:**
- DevOps principper er universelle, men værdien varierer per branche
- **E-commerce:** Fokus på speed og skalering
- **Finans:** Fokus på security og compliance
- **Healthcare:** Fokus på safety og reliability
- **Manufacturing:** Fokus på device management
- **Media:** Fokus på user experience
- **Government:** Fokus på transparency og compliance

---

## SLIDE 27: Happy-Headlines (Media/News)
**Titel:** Happy-Headlines (Media/News)

**Indhold:**
- Happy-Headlines anvender DevOps til:
  - ✅ Hurtigere deployment af nye features
  - ✅ Håndtering af trafik-spikes (breaking news)
  - ✅ Kontinuerlig monitoring (Prometheus, Grafana)
  - ✅ Automatiseret CI/CD pipeline

---

## SLIDE 28: Spørgsmål?
**Titel:** Spørgsmål?

**Indhold:**
- Dit navn
- Kontaktinfo (hvis relevant)

---

## INSTRUKTIONER TIL GOOGLE SLIDES:

1. **Opret ny Google Slides præsentation**
2. **For hver slide:**
   - Kopier titlen som slide-titel
   - Kopier indholdet som slide-body
   - Tilføj screenshots hvor angivet
3. **Design forslag:**
   - Brug konsistent farveskema
   - Brug store, læsbare fonte
   - Tilføj kode-snippets som billeder eller tekstbokse
4. **Screenshots:**
   - Tag screenshots af de angivne filer
   - Indsæt som billeder på relevante slides
   - Tilføj annotations hvis nødvendigt
5. **Diagram:**
   - Slide 2: Tegn Venn-diagram med tre overlappende cirkler
   - Slide 15: Tegn flow diagram eller brug tekst
