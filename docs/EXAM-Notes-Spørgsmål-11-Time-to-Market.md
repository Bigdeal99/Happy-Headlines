# EKSAMEN - Spørgsmål 11: Time to Market
## Slide Outline & Speaking Notes

---

## SLIDE 1: Titel
**Speaking Notes:**
- "Jeg vil i dag præsentere Time to Market"
- "Jeg dækker tre dele: definition og relation til skaleringsprincipper, mekanismer til forbedring, og negative indvirkninger af arkitektoniske valg"
- "Jeg bruger Happy-Headlines microservices arkitektur som eksempel"

---

## PART A: FORKLAR TIME TO MARKET

---

## SLIDE 2: Time to Market - Definition
**Speaking Notes:**
- "Time to Market (TTM) er tiden fra idé til produkt er tilgængelig for brugere"
- "Formel: Time to Market = Ideation Time + Development Time + Deployment Time"
- "Eksempel: Feature idé 'Tilføj comment moderation', Development 2 uger, Deployment 1 dag, Time to Market 15 dage"
- "Mål: Minimér TTM for at være konkurrencedygtig, hurtigere feedback fra brugere, første-mover fordele"

---

## SLIDE 3: Relation til Skaleringsprincipper - Performance
**Speaking Notes:**
- "Relation til Skaleringsprincipper: Performance Skalering → Time to Market"
- "Princippet: Højere performance = Hurtigere development, færre performance-problemer = mindre tid på optimering"
- "Eksempel: Lav performance - system langsomt → udviklere venter på tests → længere TTM, Høj performance - system hurtigt → hurtigere feedback → kortere TTM"
- "Dette viser at performance direkte påvirker development hastighed"

---

## SLIDE 4: Relation til Skaleringsprincipper - Cost
**Speaking Notes:**
- "Relation til Skaleringsprincipper: Cost Skalering → Time to Market"
- "Princippet: Invester i automation = Hurtigere deployment, færre manuelle steps = kortere TTM"
- "Eksempel: Lav cost (manuel) - 30-60 min deployment → længere TTM, Høj cost (automatiseret) - 5-10 min deployment → kortere TTM"
- "Dette viser at investering i automation forbedrer deployment hastighed"

---

## SLIDE 5: Relation til Skaleringsprincipper - People
**Speaking Notes:**
- "Relation til Skaleringsprincipper: People Skalering → Time to Market"
- "Princippet: Automatisering = Færre mennesker nødvendige, parallel development = hurtigere TTM"
- "Eksempel: Lille team - sequential development → længere TTM, Stort team - parallel development → kortere TTM"
- "Dette viser at organisatorisk skalering forbedrer development hastighed"

---

## SLIDE 6: Relation til Skaleringskuben - X-akse
**Speaking Notes:**
- "Relation til Skaleringskuben: X-akse (Horizontal Duplication) → Time to Market"
- "Princippet: Flere instanser = Hurtigere deployment (ingen impact på andre), parallel deployment = kortere TTM"
- "Impact: 1 instance - deployment påvirker alle brugere → risk → længere TTM (venter på low-traffic window), 3 instances - canary deployment → deploy til 1 instance først → kortere TTM (deploy anytime)"
- "Dette viser at X-akse skalering reducerer deployment risk og forbedrer TTM"

---

## SLIDE 7: Relation til Skaleringskuben - Y-akse
**Speaking Notes:**
- "Relation til Skaleringskuben: Y-akse (Functional Decomposition) → Time to Market"
- "Princippet: Microservices = Independent deployment, teams kan deploye uafhængigt → kortere TTM"
- "Impact: Monolitisk - feature change → deploy hele systemet → koordinering nødvendig → længere TTM, Microservices - feature change → deploy kun 1 service → ingen koordinering → kortere TTM"
- "Eksempel fra kodebasen: Happy-Headlines (6 services) - CommentService feature → deploy kun CommentService → 5 min, Monolitisk → deploy hele systemet → 30 min + koordinering"
- "Forbedring: Monolitisk - 1 deployment = 30 min + koordinering → længere TTM, Microservices - 1 deployment = 5 min, uafhængigt → kortere TTM"

---

## SLIDE 8: Relation til Skaleringskuben - Z-akse
**Speaking Notes:**
- "Relation til Skaleringskuben: Z-akse (Data Partitioning) → Time to Market"
- "Princippet: Flere databases = Independent deployment per region, kan deploye til én region først → kortere TTM"
- "Impact: 1 database - deployment påvirker alle regions → risk → længere TTM, 8 databases - deploy til 1 region først → test → rollout → kortere TTM"
- "Dette viser at Z-akse skalering reducerer deployment risk og forbedrer TTM"

---

## SLIDE 9: Kombineret Skalering → Time to Market
**Speaking Notes:**
- "Kombineret Skalering → Time to Market:"
- "X + Y + Z akser kombineret:"
- "X-akse: Canary deployment - deploy til 1 instance først"
- "Y-akse: Independent service deployment - deploy kun 1 service"
- "Z-akse: Regional rollout - deploy til 1 region først"
- "Time to Market forbedring: Monolitisk, 1 instance, 1 region - Deployment 30 min, Risk høj → vent på low-traffic → +2 timer, TTM 2.5 timer"
- "Microservices, 3 instances, 8 regions - Deployment 5 min (kun 1 service), Risk lav (canary) → deploy anytime, TTM 5 min (6x hurtigere)"

---

## PART B: MEKANISMER TIL FORBEDRING

---

## SLIDE 10: Problemstilling - Langsom Time to Market
**Speaking Notes:**
- "Problemstilling: System har langsom time to market"
- "Feature development: 2 uger, Deployment: 1 dag (manuel), Total TTM: 15 dage"
- "Mål: Feature development 2 uger (samme), Deployment 5 minutter (automatiseret), Total TTM 14 dage (1 dag forbedring)"
- "Vi skal implementere mekanismer til forbedring af TTM"

---

## SLIDE 11: Mekanisme 1 - CI/CD Automation
**Speaking Notes:**
- "Mekanisme 1: CI/CD Automation (Reducerer Deployment Time)"
- "Kodeeksempel: .github/workflows/ci-cd.yml (linje 1-111) - Automatisk trigger på push til main/master, automatisk build + push"
- "Time to Market forbedring: Før (Manuel) - 30-60 minutter deployment → 1 dag TTM, Efter (Automatiseret) - 5-10 minutter deployment → 5 min TTM"
- "Ved stigende trafik: Før - manuel deployment → vent på low-traffic window → +2 timer → længere TTM, Efter - automatisk deployment → deploy anytime → ingen ventetid → kortere TTM"
- "Dette viser hvordan CI/CD automation reducerer deployment time og forbedrer TTM"

---

## SLIDE 12: Mekanisme 2 - Parallel Build
**Speaking Notes:**
- "Mekanisme 2: Parallel Build (Matrix Strategy)"
- "Kodeeksempel: .github/workflows/ci-cd.yml (linje 29-49) - Matrix strategy med 6 services bygges parallel"
- "Time to Market forbedring: Før (Sequential) - Build 6 services sekventielt → 6 × 5 min = 30 min, Efter (Parallel) - Build 6 services parallel → 5 min (længste service)"
- "Forbedring: 30 min → 5 min = 6x hurtigere"
- "Dette viser hvordan parallel build reducerer build time og forbedrer TTM"

---

## SLIDE 13: Mekanisme 3 - Y-akse Skalering
**Speaking Notes:**
- "Mekanisme 3: Y-akse Skalering (Independent Deployment)"
- "Kodeeksempel: ArticleService/Controllers/ArticlesController.cs - Isoleret service, kan deployes uafhængigt"
- "Time to Market forbedring: Før (Monolitisk) - Feature change → deploy hele systemet → 30 min + koordinering → længere TTM, Efter (Microservices) - Feature change → deploy kun 1 service → 5 min, uafhængigt → kortere TTM"
- "Eksempel: CommentService feature → deploy kun CommentService → 5 min → kortere TTM, Monolitisk → deploy hele systemet → 30 min + koordinering → længere TTM"
- "Dette viser hvordan Y-akse skalering giver independent deployment og forbedrer TTM"

---

## SLIDE 14: Mekanisme 4 - Build Caching
**Speaking Notes:**
- "Mekanisme 4: Build Caching (Hurtigere Builds)"
- "Kodeeksempel: .github/workflows/ci-cd.yml (linje 100-101) - cache-from: type=gha, cache-to: type=gha,mode=max"
- "Time to Market forbedring: Før (No Cache) - Hver build = 5 min → længere TTM, Efter (Cache) - Hver build = 2 min (cache hit) → kortere TTM"
- "Forbedring: 5 min → 2 min = 2.5x hurtigere builds"
- "Dette viser hvordan build caching reducerer build time og forbedrer TTM"

---

## SLIDE 15: Mekanisme 5 - Asynkron Processing
**Speaking Notes:**
- "Mekanisme 5: Asynkron Processing (Decoupling)"
- "Kodeeksempel: PublisherService/Controllers/PublishController.cs (linje 34-37) - RabbitMQ queue, returnerer 202 Accepted umiddelbart"
- "Time to Market forbedring: Før (Synkron) - PublisherService venter på ArticleService → blokeret → længere TTM, Efter (Asynkron) - PublisherService returnerer umiddelbart → ingen blokering → kortere TTM"
- "Ved stigende trafik: Før - synkron → ArticleService overbelastet → PublisherService blokeret → længere TTM, Efter - asynkron → ArticleService processerer i baggrunden → PublisherService fortsætter → kortere TTM"
- "Dette viser hvordan asynkron processing decoupler services og forbedrer TTM"

---

## SLIDE 16: Mekanisme 6 - Feature Flags
**Speaking Notes:**
- "Mekanisme 6: Feature Flags (Deploy Code før Feature Release)"
- "Kodeeksempel: Konceptuelt - Feature flag implementation, if (await _flags.IsEnabledAsync("new-cache-strategy"))"
- "Time to Market forbedring: Før - Deploy kode → aktivér feature → 2 steps → længere TTM, Efter - Deploy kode med flag OFF → aktivér flag når klar → separerer deployment fra release → kortere TTM"
- "Eksempel: Deploy 5 min (med flag OFF), Aktivér feature 1 sek (toggle flag), Total TTM 5 min (vs. 30 min hvis venter på feature completion)"
- "Dette viser hvordan feature flags separerer deployment fra release og forbedrer TTM"

---

## SLIDE 17: Mekanisme 7 - Canary Deployment
**Speaking Notes:**
- "Mekanisme 7: Canary Deployment (X-akse Skalering)"
- "Kodeeksempel: docker-compose.yml - Kan skaleres til flere replicas, deploy.replicas: 3 for canary deployment"
- "Time to Market forbedring: Før (1 instance) - Deploy påvirker alle brugere → risk → vent på low-traffic → +2 timer → længere TTM, Efter (3 instances) - Deploy til 1 instance først → test → rollout → deploy anytime → kortere TTM"
- "Eksempel: Canary - deploy til 1/3 instances → test → hvis OK, deploy til resten, TTM 5 min (deploy anytime, ingen ventetid)"
- "Dette viser hvordan canary deployment reducerer deployment risk og forbedrer TTM"

---

## SLIDE 18: Kombineret Approach - Optimal Time to Market
**Speaking Notes:**
- "Kombineret Approach (Optimal Time to Market):"
- "Alle mekanismer kombineret: CI/CD Pipeline - Parallel build (matrix strategy), Build caching, Independent deployment (Y-akse), Asynkron processing, Feature flags, Canary deployment (X-akse)"
- "Time to Market ved stigende trafik:"
- "Feature development: Uden Mekanismer 2 uger, Med Alle Mekanismer 2 uger, Forbedring Samme"
- "Deployment (manuel): Uden Mekanismer 30-60 min, Med Alle Mekanismer - (elimineret), Forbedring Elimineret"
- "Deployment (automatiseret): Uden Mekanismer - (ikke implementeret), Med Alle Mekanismer 5 min, Forbedring ✅"
- "Ventetid (low-traffic): Uden Mekanismer +2 timer, Med Alle Mekanismer 0 min, Forbedring ✅ Elimineret"
- "Total TTM: Uden Mekanismer 15 dage, Med Alle Mekanismer 14 dage, Forbedring 1 dag forbedring"
- "Ved flere features: 10 features/år → 10 dage forbedring/år, ROI høj (hurtigere market response)"

---

## PART C: NEGATIVE INDRIVNINGER

---

## SLIDE 19: Negativ Indvirkning 1 - Monolitisk Arkitektur
**Speaking Notes:**
- "Negativ Indvirkning 1: Monolitisk Arkitektur"
- "Problem: Alle features i samme codebase → alle deployment sammen"
- "Negativ Impact: Feature change → deploy hele systemet → 30 min, Koordinering → alle teams skal koordinere → +1 dag, Risk høj → vent på low-traffic → +2 timer, Total TTM 1 dag + 2 timer = længere TTM"
- "Eksempel: Monolitisk - CommentService feature → deploy hele monolit → 30 min, ArticleService feature → deploy hele monolit → 30 min, Koordinering nødvendig → +1 dag, Total TTM 1 dag + 30 min"
- "Løsning: Y-akse skalering (microservices) → independent deployment → 5 min per service"
- "Dette viser hvordan monolitisk arkitektur reducerer TTM"

---

## SLIDE 20: Negativ Indvirkning 2 - Tæt Service Coupling
**Speaking Notes:**
- "Negativ Indvirkning 2: Tæt Service Coupling"
- "Problem: Services er tæt koblet, kan ikke deployes uafhængigt"
- "Eksempel fra kodebasen: PublisherService/Controllers/PublishController.cs (linje 24-27) - Synkron dependency på ProfanityService"
- "Negativ Impact: PublisherService deployment skal koordinere med ProfanityService → længere TTM, Breaking changes skal deploye begge services samtidigt → længere TTM, Testing skal teste integration → +1 dag → længere TTM"
- "Løsning: Asynkron processing (decoupling), API versioning (backward compatibility), Contract testing"
- "Dette viser hvordan tæt service coupling reducerer TTM"

---

## SLIDE 21: Negativ Indvirkning 3 - Manuel Deployment Process
**Speaking Notes:**
- "Negativ Indvirkning 3: Manuel Deployment Process"
- "Problem: Ingen automation → mange manuelle steps"
- "Negativ Impact: Deployment time 30-60 minutter (manuel), Fejl-risiko høj → rework → +1 dag → længere TTM, Konsistens lav → forskellige udviklere → længere TTM"
- "Eksempel: Manuel deployment - 1. Build Docker image lokalt (10 min), 2. Test image lokalt (5 min), 3. Tag image (2 min), 4. Push til registry (5 min), 5. SSH til server (2 min), 6. Pull image (3 min), 7. Stop container (1 min), 8. Start container (2 min), 9. Test deployment (10 min), Total 40 minutter"
- "Løsning: CI/CD automation → 5-10 minutter automatisk"
- "Dette viser hvordan manuel deployment process reducerer TTM"

---

## SLIDE 22: Negativ Indvirkning 4 - Sequential Build Process
**Speaking Notes:**
- "Negativ Indvirkning 4: Sequential Build Process"
- "Problem: Services bygges sekventielt i stedet for parallel"
- "Negativ Impact: Build time 6 services × 5 min = 30 minutter, Længere TTM 30 min ekstra per deployment"
- "Eksempel: Sequential build - Service 1: 5 min, Service 2: 5 min (venter på Service 1), Service 3: 5 min (venter på Service 2), ... Total 30 minutter"
- "Løsning: Parallel build (matrix strategy) → 5 minutter (længste service)"
- "Dette viser hvordan sequential build process reducerer TTM"

---

## SLIDE 23: Negativ Indvirkning 5 - Ingen Build Caching
**Speaking Notes:**
- "Negativ Indvirkning 5: Ingen Build Caching"
- "Problem: Hver build starter fra scratch"
- "Negativ Impact: Build time 5 minutter hver gang, Længere TTM 5 min ekstra per deployment"
- "Løsning: Build caching → 2 minutter (cache hit)"
- "Dette viser hvordan manglende build caching reducerer TTM"

---

## SLIDE 24: Negativ Indvirkning 6 - Database Schema Changes
**Speaking Notes:**
- "Negativ Indvirkning 6: Database Schema Changes"
- "Problem: Breaking database changes kræver koordineret deployment"
- "Negativ Impact: Migration time database migration → +30 min, Koordinering alle services skal deployes samtidigt → +1 dag, Risk høj → vent på low-traffic → +2 timer, Total TTM 1 dag + 2.5 timer = længere TTM"
- "Løsning: Backward-compatible migrations, Feature flags (deploy code først, aktivér senere), Database versioning"
- "Dette viser hvordan database schema changes reducerer TTM"

---

## SLIDE 25: Negativ Indvirkning 7 - Ingen Feature Flags
**Speaking Notes:**
- "Negativ Indvirkning 7: Ingen Feature Flags"
- "Problem: Kode deployment = feature release (samme step)"
- "Negativ Impact: Risk høj → vent på feature completion → længere TTM, Testing skal teste feature før deployment → +1 dag → længere TTM"
- "Eksempel: Uden feature flags - Develop feature (2 uger), Test feature (1 dag), Deploy feature (5 min), Total TTM 15 dage"
- "Løsning: Feature flags → deploy kode med flag OFF → test → aktivér flag → kortere TTM"
- "Dette viser hvordan manglende feature flags reducerer TTM"

---

## SLIDE 26: Negativ Indvirkning 8 - Single Deployment Window
**Speaking Notes:**
- "Negativ Indvirkning 8: Single Deployment Window"
- "Problem: Kan kun deploye i bestemte tidsvinduer (fx natten)"
- "Negativ Impact: Ventetid vent på deployment window → +12 timer → længere TTM, Risk høj → vent på low-traffic → +2 timer → længere TTM"
- "Eksempel: Single deployment window - Feature klar kl. 10:00, Deployment window 02:00-04:00, Ventetid 16 timer, Total TTM +16 timer"
- "Løsning: Canary deployment → deploy anytime, Feature flags → deploy kode, aktivér senere, Blue-green deployment → zero-downtime"
- "Dette viser hvordan single deployment window reducerer TTM"

---

## SLIDE 27: Sammenligning - Negativ vs. Positiv Impact
**Speaking Notes:**
- "Sammenligning: Negativ vs. Positiv Impact"
- "Monolitisk: ❌ Længere (30 min + koordinering) - Deploy hele systemet"
- "Microservices (Y-akse): ✅ Kortere (5 min, uafhængigt) - Deploy kun 1 service"
- "Manuel deployment: ❌ Længere (30-60 min) - 10+ manuelle steps"
- "CI/CD automation: ✅ Kortere (5-10 min) - Automatisk pipeline"
- "Sequential build: ❌ Længere (30 min) - 6 services × 5 min"
- "Parallel build: ✅ Kortere (5 min) - Matrix strategy"
- "Ingen caching: ❌ Længere (5 min/build) - Hver build fra scratch"
- "Build caching: ✅ Kortere (2 min/build) - Cache hit"
- "Dette viser tydeligt forskellen mellem negative og positive arkitektoniske valg"

---

## SLIDE 28: Best Practices for Kort Time to Market
**Speaking Notes:**
- "Best Practices for Kort Time to Market:"
- "1. Automatisering: CI/CD pipeline (automatisk build, test, deploy), Build caching (hurtigere builds), Parallel execution (matrix strategy)"
- "2. Decoupling: Y-akse skalering (microservices), Asynkron processing (queues), API versioning (backward compatibility)"
- "3. Risk Reduction: Feature flags (deploy kode først), Canary deployment (deploy til 1 instance først), Blue-green deployment (zero-downtime)"
- "4. Independent Deployment: Microservices (deploy kun 1 service), Database versioning (backward-compatible migrations), Contract testing (test interfaces)"
- "Dette er de vigtigste principper for kort time to market"

---

## SLIDE 29: Konklusion
**Speaking Notes:**
- "Konklusion:"
- "Arkitektoniske valg der reducerer Time to Market: ❌ Monolitisk arkitektur, ❌ Tæt service coupling, ❌ Manuel deployment process, ❌ Sequential build process, ❌ Ingen build caching, ❌ Database schema changes, ❌ Ingen feature flags, ❌ Tight integration testing, ❌ Manglende automation, ❌ Single deployment window"
- "Arkitektoniske valg der forbedrer Time to Market: ✅ Microservices (Y-akse), ✅ CI/CD automation, ✅ Parallel build, ✅ Build caching, ✅ Asynkron processing, ✅ Feature flags, ✅ Canary deployment, ✅ Independent deployment, ✅ Contract testing, ✅ Anytime deployment"
- "Anbefaling: Design systemet med Time to Market i tankerne fra starten - Automatiser alt (CI/CD), Decouple services (microservices, asynkron), Reducer risk (feature flags, canary deployment), Enable independent deployment (Y-akse skalering)"
- "Resultat: Før 15 dage TTM (2 uger development + 1 dag deployment) → Efter 14 dage TTM (2 uger development + 5 min deployment) = 1 dag forbedring (6.7% forbedring)"

---

## SLIDE 30: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres opmærksomhed"
- "Jeg er klar til spørgsmål"

---

## EKSTRA NOTER TIL EKSAMEN:

### Hvis de spørger om specifikke filer:
- **`.github/workflows/ci-cd.yml`**: CI/CD pipeline med parallel build, build caching, automatisk deployment
- **`docker-compose.yml`**: X-akse skalering (replicas), Y-akse skalering (6 services), Z-akse skalering (8 databases)
- **`PublisherService/Controllers/PublishController.cs`**: Asynkron processing eksempel
- **`ArticleService/Controllers/ArticlesController.cs`**: Independent service eksempel

### Hvis de spørger om time to market beregninger:
- Time to Market = Ideation Time + Development Time + Deployment Time
- Før: 15 dage TTM (2 uger development + 1 dag deployment)
- Efter: 14 dage TTM (2 uger development + 5 min deployment)
- Forbedring: 1 dag (6.7% forbedring)

### Hvis de spørger om skaleringskuben:
- X-akse: Horizontal Duplication → Canary deployment → Kortere TTM
- Y-akse: Functional Decomposition → Independent deployment → Kortere TTM
- Z-akse: Data Partitioning → Regional rollout → Kortere TTM
- Kombineret: X + Y + Z → 6x hurtigere TTM

### Hvis de spørger om mekanismer:
- CI/CD Automation: 30-60 min → 5-10 min (6-12x hurtigere)
- Parallel Build: 30 min → 5 min (6x hurtigere)
- Y-akse Skalering: 30 min + koordinering → 5 min (uafhængigt)
- Build Caching: 5 min → 2 min (2.5x hurtigere)
- Asynkron Processing: Blokering → Ingen blokering
- Feature Flags: Separerer deployment fra release
- Canary Deployment: Deploy anytime → Ingen ventetid

### Hvis de spørger om negative indvirkninger:
- Monolitisk: 30 min + koordinering → Længere TTM
- Tæt coupling: Koordinering nødvendig → Længere TTM
- Manuel deployment: 30-60 min → Længere TTM
- Sequential build: 30 min → Længere TTM
- Ingen caching: 5 min/build → Længere TTM
- Database schema changes: +1 dag + 2.5 timer → Længere TTM
- Ingen feature flags: Vent på completion → Længere TTM
- Single deployment window: +16 timer → Længere TTM
