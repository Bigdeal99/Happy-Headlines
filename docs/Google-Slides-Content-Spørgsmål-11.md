# Google Slides Content - Spørgsmål 11: Time to Market
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Time to Market
**Undertitel:** Hvordan skaleringsprincipper og arkitektur påvirker deployment hastighed
**Footer:** Dit navn | Dato

---

## SLIDE 2: Time to Market - Definition
**Titel:** Time to Market - Definition

**Indhold:**
- **Time to Market (TTM) = Ideation Time + Development Time + Deployment Time**
- **Eksempel:** Feature idé "Tilføj comment moderation" → Development 2 uger → Deployment 1 dag → TTM 15 dage
- **Mål:** Minimér TTM for at være konkurrencedygtig, hurtigere feedback fra brugere, første-mover fordele

---

## SLIDE 3: Relation til Skaleringsprincipper - Performance
**Titel:** Relation til Skaleringsprincipper - Performance

**Indhold:**
- **Performance Skalering → Time to Market**
- Højere performance = Hurtigere development
- Færre performance-problemer = Mindre tid på optimering
- **Eksempel:** Lav performance (system langsomt → udviklere venter på tests → længere TTM), Høj performance (system hurtigt → hurtigere feedback → kortere TTM)

---

## SLIDE 4: Relation til Skaleringsprincipper - Cost
**Titel:** Relation til Skaleringsprincipper - Cost

**Indhold:**
- **Cost Skalering → Time to Market**
- Invester i automation = Hurtigere deployment
- Færre manuelle steps = Kortere TTM
- **Eksempel:** Lav cost (manuel) 30-60 min deployment → længere TTM, Høj cost (automatiseret) 5-10 min deployment → kortere TTM

---

## SLIDE 5: Relation til Skaleringsprincipper - People
**Titel:** Relation til Skaleringsprincipper - People

**Indhold:**
- **People Skalering → Time to Market**
- Automatisering = Færre mennesker nødvendige
- Parallel development = Hurtigere TTM
- **Eksempel:** Lille team (sequential development → længere TTM), Stort team (parallel development → kortere TTM)

---

## SLIDE 6: Relation til Skaleringskuben - X-akse
**Titel:** Relation til Skaleringskuben - X-akse

**Indhold:**
- **X-akse (Horizontal Duplication) → Time to Market**
- Flere instanser = Hurtigere deployment (ingen impact på andre)
- Parallel deployment = Kortere TTM
- **Eksempel:** 1 instance (deployment påvirker alle brugere → risk → længere TTM), 3 instances (canary deployment → deploy til 1 instance først → kortere TTM)

---

## SLIDE 7: Relation til Skaleringskuben - Y-akse
**Titel:** Relation til Skaleringskuben - Y-akse

**Indhold:**
- **Y-akse (Functional Decomposition) → Time to Market**
- Microservices = Independent deployment
- Teams kan deploye uafhængigt → Kortere TTM
- **Eksempel:** Monolitisk (feature change → deploy hele systemet → koordinering nødvendig → længere TTM), Microservices (feature change → deploy kun 1 service → ingen koordinering → kortere TTM)

---

## SLIDE 8: Relation til Skaleringskuben - Z-akse
**Titel:** Relation til Skaleringskuben - Z-akse

**Indhold:**
- **Z-akse (Data Partitioning) → Time to Market**
- Flere databases = Independent deployment per region
- Kan deploye til én region først → Kortere TTM
- **Eksempel:** 1 database (deployment påvirker alle regions → risk → længere TTM), 8 databases (deploy til 1 region først → test → rollout → kortere TTM)

---

## SLIDE 9: Kombineret Skalering → Time to Market
**Titel:** Kombineret Skalering → Time to Market

**Indhold:**
- **X + Y + Z akser kombineret**
- X-akse: Canary deployment (deploy til 1 instance først)
- Y-akse: Independent service deployment
- Z-akse: Regional rollout (deploy til 1 region først)
- **TTM forbedring:** Monolitisk, 1 instance, 1 region (2.5 timer) → Microservices, 3 instances, 8 regions (5 min) = 6x hurtigere

---

## SLIDE 10: Problemstilling - Langsom Time to Market
**Titel:** Problemstilling - Langsom Time to Market

**Indhold:**
- System har langsom time to market
- Feature development: 2 uger
- Deployment: 1 dag (manuel)
- Total TTM: 15 dage
- **Mål:** Feature development 2 uger (samme), Deployment 5 minutter (automatiseret), Total TTM 14 dage (1 dag forbedring)

---

## SLIDE 11: Mekanisme 1 - CI/CD Automation
**Titel:** Mekanisme 1 - CI/CD Automation

**Indhold:**
- **CI/CD Automation (Reducerer Deployment Time)**
- Kodeeksempel: `.github/workflows/ci-cd.yml` (linje 1-111)
- **Før (Manuel):** 30-60 minutter deployment → 1 dag TTM
- **Efter (Automatiseret):** 5-10 minutter deployment → 5 min TTM
- **Ved stigende trafik:** Automatisk deployment → deploy anytime → ingen ventetid → kortere TTM

---

## SLIDE 12: Mekanisme 2 - Parallel Build
**Titel:** Mekanisme 2 - Parallel Build

**Indhold:**
- **Parallel Build (Matrix Strategy)**
- Kodeeksempel: `.github/workflows/ci-cd.yml` (linje 29-49)
- **Før (Sequential):** Build 6 services sekventielt → 6 × 5 min = 30 min
- **Efter (Parallel):** Build 6 services parallel → 5 min (længste service)
- **Forbedring:** 30 min → 5 min = 6x hurtigere

---

## SLIDE 13: Mekanisme 3 - Y-akse Skalering
**Titel:** Mekanisme 3 - Y-akse Skalering

**Indhold:**
- **Y-akse Skalering (Independent Deployment)**
- Kodeeksempel: `ArticleService/Controllers/ArticlesController.cs` - Isoleret service, kan deployes uafhængigt
- **Før (Monolitisk):** Feature change → deploy hele systemet → 30 min + koordinering → længere TTM
- **Efter (Microservices):** Feature change → deploy kun 1 service → 5 min, uafhængigt → kortere TTM
- **Eksempel:** CommentService feature → deploy kun CommentService → 5 min → kortere TTM

---

## SLIDE 14: Mekanisme 4 - Build Caching
**Titel:** Mekanisme 4 - Build Caching

**Indhold:**
- **Build Caching (Hurtigere Builds)**
- Kodeeksempel: `.github/workflows/ci-cd.yml` (linje 100-101) - cache-from: type=gha, cache-to: type=gha,mode=max
- **Før (No Cache):** Hver build = 5 min → længere TTM
- **Efter (Cache):** Hver build = 2 min (cache hit) → kortere TTM
- **Forbedring:** 5 min → 2 min = 2.5x hurtigere builds

---

## SLIDE 15: Mekanisme 5 - Asynkron Processing
**Titel:** Mekanisme 5 - Asynkron Processing

**Indhold:**
- **Asynkron Processing (Decoupling)**
- Kodeeksempel: `PublisherService/Controllers/PublishController.cs` (linje 34-37) - RabbitMQ queue, returnerer 202 Accepted umiddelbart
- **Før (Synkron):** PublisherService venter på ArticleService → blokeret → længere TTM
- **Efter (Asynkron):** PublisherService returnerer umiddelbart → ingen blokering → kortere TTM
- **Ved stigende trafik:** Asynkron → ArticleService processerer i baggrunden → PublisherService fortsætter → kortere TTM

---

## SLIDE 16: Mekanisme 6 - Feature Flags
**Titel:** Mekanisme 6 - Feature Flags

**Indhold:**
- **Feature Flags (Deploy Code før Feature Release)**
- Kodeeksempel: Konceptuelt - Feature flag implementation
- **Før:** Deploy kode → aktivér feature → 2 steps → længere TTM
- **Efter:** Deploy kode med flag OFF → aktivér flag når klar → separerer deployment fra release → kortere TTM
- **Eksempel:** Deploy 5 min (med flag OFF), Aktivér feature 1 sek (toggle flag), Total TTM 5 min (vs. 30 min hvis venter på feature completion)

---

## SLIDE 17: Mekanisme 7 - Canary Deployment
**Titel:** Mekanisme 7 - Canary Deployment

**Indhold:**
- **Canary Deployment (X-akse Skalering)**
- Kodeeksempel: `docker-compose.yml` - Kan skaleres til flere replicas, deploy.replicas: 3
- **Før (1 instance):** Deploy påvirker alle brugere → risk → vent på low-traffic → +2 timer → længere TTM
- **Efter (3 instances):** Deploy til 1 instance først → test → rollout → deploy anytime → kortere TTM
- **Eksempel:** Canary - deploy til 1/3 instances → test → hvis OK, deploy til resten, TTM 5 min (deploy anytime, ingen ventetid)

---

## SLIDE 18: Kombineret Approach - Optimal Time to Market
**Titel:** Kombineret Approach - Optimal Time to Market

**Indhold:**
- **Alle mekanismer kombineret**
- CI/CD Pipeline: Parallel build (matrix strategy), Build caching, Independent deployment (Y-akse), Asynkron processing, Feature flags, Canary deployment (X-akse)
- **Tabel:**
  | Scenario | Uden Mekanismer | Med Alle Mekanismer | Forbedring |
  |----------|-----------------|---------------------|------------|
  | Feature development | 2 uger | 2 uger | Samme |
  | Deployment (manuel) | 30-60 min | - | Elimineret |
  | Deployment (automatiseret) | - | 5 min | ✅ |
  | Ventetid (low-traffic) | +2 timer | 0 min | ✅ Elimineret |
  | Total TTM | 15 dage | 14 dage | 1 dag forbedring |

---

## SLIDE 19: Negativ Indvirkning 1 - Monolitisk Arkitektur
**Titel:** Negativ Indvirkning 1 - Monolitisk Arkitektur

**Indhold:**
- **Monolitisk Arkitektur**
- Problem: Alle features i samme codebase → alle deployment sammen
- **Negativ Impact:** Feature change → deploy hele systemet → 30 min, Koordinering → alle teams skal koordinere → +1 dag, Risk høj → vent på low-traffic → +2 timer, Total TTM 1 dag + 2 timer = længere TTM
- **Løsning:** Y-akse skalering (microservices) → independent deployment → 5 min per service

---

## SLIDE 20: Negativ Indvirkning 2 - Tæt Service Coupling
**Titel:** Negativ Indvirkning 2 - Tæt Service Coupling

**Indhold:**
- **Tæt Service Coupling**
- Problem: Services er tæt koblet, kan ikke deployes uafhængigt
- Eksempel: `PublisherService/Controllers/PublishController.cs` (linje 24-27) - Synkron dependency på ProfanityService
- **Negativ Impact:** PublisherService deployment skal koordinere med ProfanityService → længere TTM, Breaking changes skal deploye begge services samtidigt → længere TTM
- **Løsning:** Asynkron processing (decoupling), API versioning (backward compatibility), Contract testing

---

## SLIDE 21: Negativ Indvirkning 3 - Manuel Deployment Process
**Titel:** Negativ Indvirkning 3 - Manuel Deployment Process

**Indhold:**
- **Manuel Deployment Process**
- Problem: Ingen automation → mange manuelle steps
- **Negativ Impact:** Deployment time 30-60 minutter (manuel), Fejl-risiko høj → rework → +1 dag → længere TTM, Konsistens lav → forskellige udviklere → længere TTM
- **Eksempel:** Manuel deployment - 9 steps (40 minutter total)
- **Løsning:** CI/CD automation → 5-10 minutter automatisk

---

## SLIDE 22: Negativ Indvirkning 4 - Sequential Build Process
**Titel:** Negativ Indvirkning 4 - Sequential Build Process

**Indhold:**
- **Sequential Build Process**
- Problem: Services bygges sekventielt i stedet for parallel
- **Negativ Impact:** Build time 6 services × 5 min = 30 minutter, Længere TTM 30 min ekstra per deployment
- **Eksempel:** Sequential build - Service 1: 5 min, Service 2: 5 min (venter på Service 1), Service 3: 5 min (venter på Service 2), ... Total 30 minutter
- **Løsning:** Parallel build (matrix strategy) → 5 minutter (længste service)

---

## SLIDE 23: Negativ Indvirkning 5 - Ingen Build Caching
**Titel:** Negativ Indvirkning 5 - Ingen Build Caching

**Indhold:**
- **Ingen Build Caching**
- Problem: Hver build starter fra scratch
- **Negativ Impact:** Build time 5 minutter hver gang, Længere TTM 5 min ekstra per deployment
- **Løsning:** Build caching → 2 minutter (cache hit)

---

## SLIDE 24: Negativ Indvirkning 6 - Database Schema Changes
**Titel:** Negativ Indvirkning 6 - Database Schema Changes

**Indhold:**
- **Database Schema Changes**
- Problem: Breaking database changes kræver koordineret deployment
- **Negativ Impact:** Migration time database migration → +30 min, Koordinering alle services skal deployes samtidigt → +1 dag, Risk høj → vent på low-traffic → +2 timer, Total TTM 1 dag + 2.5 timer = længere TTM
- **Løsning:** Backward-compatible migrations, Feature flags (deploy code først, aktivér senere), Database versioning

---

## SLIDE 25: Negativ Indvirkning 7 - Ingen Feature Flags
**Titel:** Negativ Indvirkning 7 - Ingen Feature Flags

**Indhold:**
- **Ingen Feature Flags**
- Problem: Kode deployment = feature release (samme step)
- **Negativ Impact:** Risk høj → vent på feature completion → længere TTM, Testing skal teste feature før deployment → +1 dag → længere TTM
- **Eksempel:** Uden feature flags - Develop feature (2 uger), Test feature (1 dag), Deploy feature (5 min), Total TTM 15 dage
- **Løsning:** Feature flags → deploy kode med flag OFF → test → aktivér flag → kortere TTM

---

## SLIDE 26: Negativ Indvirkning 8 - Single Deployment Window
**Titel:** Negativ Indvirkning 8 - Single Deployment Window

**Indhold:**
- **Single Deployment Window**
- Problem: Kan kun deploye i bestemte tidsvinduer (fx natten)
- **Negativ Impact:** Ventetid vent på deployment window → +12 timer → længere TTM, Risk høj → vent på low-traffic → +2 timer → længere TTM
- **Eksempel:** Single deployment window - Feature klar kl. 10:00, Deployment window 02:00-04:00, Ventetid 16 timer, Total TTM +16 timer
- **Løsning:** Canary deployment → deploy anytime, Feature flags → deploy kode, aktivér senere, Blue-green deployment → zero-downtime

---

## SLIDE 27: Sammenligning - Negativ vs. Positiv Impact
**Titel:** Sammenligning - Negativ vs. Positiv Impact

**Indhold:**
- **Tabel:**
  | Arkitektonisk Valg | Time to Market Impact | Eksempel |
  |---------------------|----------------------|----------|
  | Monolitisk | ❌ Længere (30 min + koordinering) | Deploy hele systemet |
  | Microservices (Y-akse) | ✅ Kortere (5 min, uafhængigt) | Deploy kun 1 service |
  | Manuel deployment | ❌ Længere (30-60 min) | 10+ manuelle steps |
  | CI/CD automation | ✅ Kortere (5-10 min) | Automatisk pipeline |
  | Sequential build | ❌ Længere (30 min) | 6 services × 5 min |
  | Parallel build | ✅ Kortere (5 min) | Matrix strategy |
  | Ingen caching | ❌ Længere (5 min/build) | Hver build fra scratch |
  | Build caching | ✅ Kortere (2 min/build) | Cache hit |

---

## SLIDE 28: Best Practices for Kort Time to Market
**Titel:** Best Practices for Kort Time to Market

**Indhold:**
- **1. Automatisering:** CI/CD pipeline (automatisk build, test, deploy), Build caching (hurtigere builds), Parallel execution (matrix strategy)
- **2. Decoupling:** Y-akse skalering (microservices), Asynkron processing (queues), API versioning (backward compatibility)
- **3. Risk Reduction:** Feature flags (deploy kode først), Canary deployment (deploy til 1 instance først), Blue-green deployment (zero-downtime)
- **4. Independent Deployment:** Microservices (deploy kun 1 service), Database versioning (backward-compatible migrations), Contract testing (test interfaces)

---

## SLIDE 29: Konklusion
**Titel:** Konklusion

**Indhold:**
- **Arkitektoniske valg der reducerer Time to Market:** ❌ Monolitisk arkitektur, ❌ Tæt service coupling, ❌ Manuel deployment process, ❌ Sequential build process, ❌ Ingen build caching, ❌ Database schema changes, ❌ Ingen feature flags, ❌ Single deployment window
- **Arkitektoniske valg der forbedrer Time to Market:** ✅ Microservices (Y-akse), ✅ CI/CD automation, ✅ Parallel build, ✅ Build caching, ✅ Asynkron processing, ✅ Feature flags, ✅ Canary deployment, ✅ Independent deployment
- **Anbefaling:** Design systemet med Time to Market i tankerne fra starten
- **Resultat:** Før 15 dage TTM (2 uger development + 1 dag deployment) → Efter 14 dage TTM (2 uger development + 5 min deployment) = 1 dag forbedring (6.7% forbedring)

---

## SLIDE 30: Spørgsmål?
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
   - Tilføj tabeller som billeder eller tekstbokse
4. **Kodeeksempler:**
   - Slide 11-17: Vis kode snippets eller referencer til filer
   - Brug monospace font for kode
5. **Tabel:**
   - Slide 18, 27: Tegn tabeller eller brug tekstbokse
6. **Screenshots forslag:**
   - Slide 11: Screenshot af `.github/workflows/ci-cd.yml` (CI/CD pipeline)
   - Slide 12: Screenshot af `.github/workflows/ci-cd.yml` (matrix strategy)
   - Slide 13: Screenshot af `ArticleService/Controllers/ArticlesController.cs` (independent service)
   - Slide 14: Screenshot af `.github/workflows/ci-cd.yml` (build caching)
   - Slide 15: Screenshot af `PublisherService/Controllers/PublishController.cs` (asynkron)
   - Slide 17: Screenshot af `docker-compose.yml` (replicas)
   - Slide 20: Screenshot af `PublisherService/Controllers/PublishController.cs` (synkron dependency)
