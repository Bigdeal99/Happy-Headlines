# EKSAMEN - Spørgsmål 5: Automation og DevOps
## Slide Outline & Speaking Notes

---

## SLIDE 1: Titel
**Speaking Notes:**
- "Jeg vil i dag præsentere Automation og DevOps"
- "Jeg vil dække tre dele: hvorfor automation er i midten af Venn-diagrammet, pipeline der eliminerer manuelle steps, og DevOps i forskellige brancher"
- "Fokus er på hvordan automation enabler alle tre skaleringstyper"

---

## PART A: FORKLAR VENN-DIAGRAMMET

---

## SLIDE 2: Venn-diagrammet for Skaleringsprincipper
**Speaking Notes:**
- "Venn-diagrammet viser tre overlappende cirkler"
- "Performance (venstre), Cost (højre), People (nederst)"
- "Automation over people befinder sig i midten - overlap af alle tre cirkler"
- "Dette betyder at automation enabler alle tre skaleringstyper"

---

## SLIDE 3: Performance (Skalerbarhed)
**Speaking Notes:**
- "Første dimension: Performance (Skalerbarhed)"
- "Automation gør det muligt at skaleres hurtigt og konsistent"
- "Eliminerer menneskelige fejl der kan påvirke performance"
- "Gør det muligt at deploye oftere - hurtigere feedback loops"
- "Konsistent deployment - forudsigelig performance"
- "Eksempel: CI/CD pipeline deployer automatisk - ingen menneskelige fejl"

---

## SLIDE 4: Cost (Økonomi)
**Speaking Notes:**
- "Anden dimension: Cost (Økonomi)"
- "Automation reducerer manuel arbejdstid - lavere omkostninger"
- "Færre fejl - færre production incidents - lavere cost"
- "Bedre resource utilization - optimeret cost"
- "Skalerer uden at skalere team størrelse - cost-efficient skalering"
- "Eksempel: Automatiseret deployment tager 5 minutter i stedet for 60 minutter manuel arbejdstid"

---

## SLIDE 5: People (Organisatorisk)
**Speaking Notes:**
- "Tredje dimension: People (Organisatorisk)"
- "Automation frigør mennesker til højere-værdi opgaver"
- "Reducerer stress og burnout - færre manuelle tasks"
- "Gør det muligt for teams at skaleres organisatorisk"
- "Konsistent process - mindre afhængighed af individuelle eksperter"
- "Eksempel: Udviklere kan fokusere på features i stedet for deployment"

---

## SLIDE 6: Hvorfor i midten?
**Speaking Notes:**
- "Hvorfor i midten?"
- "Automation er fundamentet for alle tre dimensioner"
- "Uden automation: Performance, Cost og People skalering er alle begrænsede"
- "Med automation: Alle tre dimensioner kan skaleres samtidigt"
- "Automation er en enabler for alle tre skaleringstyper"

---

## SLIDE 7: Eksempel - Automation Impact
**Speaking Notes:**
- "Eksempel på automation impact:"
- "Performance skalering: Automation gør det muligt at deploye 10x oftere"
- "Cost skalering: Automation reducerer deployment cost med 80%"
- "People skalering: Automation gør det muligt at håndtere 5x flere services med samme team"
- "Dette viser hvordan automation enabler alle tre dimensioner"

---

## SLIDE 8: Konklusion - Venn-diagrammet
**Speaking Notes:**
- "Konklusion:"
- "Automation er en enabler for alle tre skaleringstyper"
- "Derfor er den i midten af Venn-diagrammet"
- "Uden automation: Begrænsede muligheder"
- "Med automation: Alle dimensioner kan skaleres"
- "Nu viser jeg en konkret pipeline der eliminerer manuelle steps"

---

## PART B: DEMONSTRER PIPELINE

---

## SLIDE 9: Før - Manuel Deployment Workflow
**Speaking Notes:**
- "Før: Manuel Deployment Workflow"
- "10+ manuelle steps"
- "Hver step har fejl-risiko"
- "Konsistens-problemer - forskellige udviklere gør det forskelligt"
- "Lang deployment tid - 30-60 minutter"
- "Ingen audit trail"
- "Dette er ineffektivt og fejl-risiko er høj"

---

## SLIDE 10: Problemer med Manuel Workflow
**Speaking Notes:**
- "Problemer med manuel workflow:"
- "10+ manuelle steps"
- "Fejl-risiko ved hvert step"
- "Konsistens-problemer - forskellige udviklere gør det forskelligt"
- "Lang deployment tid - 30-60 minutter"
- "Ingen audit trail"
- "Dette er uholdbart ved skalering"

---

## SLIDE 11: Efter - Automatiseret CI/CD Pipeline
**Speaking Notes:**
- "Efter: Automatiseret CI/CD Pipeline"
- "Pipeline filen er `.github/workflows/ci-cd.yml`"
- "Triggers: Push til main/master, pull requests, eller manual trigger"
- "Automatisk trigger ved push - ingen manuel intervention nødvendig"
- "Dette eliminerer første manuelle step"

**Screenshot Instructions:**
1. Åbn `.github/workflows/ci-cd.yml`
2. Marker linje 1-16 (name, on triggers)
3. Vis at pipeline triggeres automatisk ved push

---

## SLIDE 12: Pipeline - Matrix Strategy
**Speaking Notes:**
- "Pipeline Matrix Strategy"
- "6 services bygges parallel i matrix"
- "ArticleService, CommentService, ProfanityService, DraftService, PublisherService, NewsletterService"
- "Parallel build - alle services bygges samtidigt"
- "Dette eliminerer manuel build step"

**Screenshot Instructions:**
1. Åbn `.github/workflows/ci-cd.yml`
2. Marker linje 29-49 (strategy matrix)
3. Vis alle 6 services i matrix

---

## SLIDE 13: Pipeline - Automatiseret Steps
**Speaking Notes:**
- "Pipeline Steps - Alle Automatiseret"
- "Step 1: Checkout code - automatisk"
- "Step 2: Setup Docker Buildx - automatisk"
- "Step 3: Login to GHCR - automatisk"
- "Step 4: Generate version tags - automatisk (SHA, short-SHA, latest)"
- "Step 5: Build and push - automatisk"
- "Alle steps er automatiseret - ingen manuel intervention"

**Screenshot Instructions:**
1. Åbn `.github/workflows/ci-cd.yml`
2. Marker linje 51-101 (steps)
3. Vis alle automatiseret steps

---

## SLIDE 14: Eliminerede Manuel Steps
**Speaking Notes:**
- "Eliminerede Manuel Steps"
- "Build Docker image: Automatiseret via docker/build-push-action"
- "Tag image: Automatiseret via Generate version tags step"
- "Push til registry: Automatiseret via Build and push step"
- "SSH til server: Ikke nødvendig - container registry"
- "Pull image: Automatisk via registry"
- "Alle manuelle steps er elimineret"

---

## SLIDE 15: Pipeline Flow
**Speaking Notes:**
- "Pipeline Flow - Fuldstændig Automatiseret"
- "Developer push til GitHub"
- "Automatisk trigger pipeline"
- "Automatisk checkout code"
- "Automatisk setup Docker Buildx"
- "Automatisk login til GHCR"
- "Automatisk generate version tags"
- "Automatisk build 6 Docker images parallel"
- "Automatisk push til GHCR"
- "Deployment klar - ingen manuel intervention"

---

## SLIDE 16: Resultat - Før vs Efter
**Speaking Notes:**
- "Resultat - Før vs Efter"
- "Tid: Fra 30-60 minutter til 5-10 minutter - 6x hurtigere"
- "People: Fra 1-2 udviklere til 0 udviklere - ingen manuel intervention"
- "Fejl-risiko: Fra høj (10+ steps) til lav (konsistent process)"
- "Konsistens: Fra lav (forskellige udviklere) til høj (samme process hver gang)"
- "Dette viser kraften i automation"

---

## SLIDE 17: Build Summary Job
**Speaking Notes:**
- "Build Summary Job"
- "Automatisk summary af alle built images"
- "Viser alle services med tags (latest, SHA, short-SHA)"
- "Automatisk audit trail"
- "Dette eliminerer manuel tracking"

**Screenshot Instructions:**
1. Åbn `.github/workflows/ci-cd.yml`
2. Marker linje 112-149 (build-summary job)
3. Vis automatisk summary generation

---

## PART C: RELATER TIL BRANCHER

---

## SLIDE 18: DevOps Principper
**Speaking Notes:**
- "DevOps Principper"
- "Automation - Automatiser repetitive tasks"
- "Continuous Integration - Integrer kode ofte"
- "Continuous Deployment - Deploy automatisk"
- "Monitoring - Overvåg kontinuerligt"
- "Infrastructure as Code - Versioner infrastruktur"
- "Collaboration - Tæt samarbejde mellem Dev og Ops"
- "Nu viser jeg hvordan disse anvendes i forskellige brancher"

---

## SLIDE 19: E-commerce (fx Amazon, Zalando)
**Speaking Notes:**
- "E-commerce (fx Amazon, Zalando)"
- "CI/CD: Deploy flere gange dagligt - Black Friday skalering"
- "Automation: Auto-scaling baseret på trafik"
- "Monitoring: Real-time metrics for salg og performance"
- "Værdi: Hurtigere time-to-market, kan håndtere trafik-spikes, reducerer downtime"
- "Eksempel: Happy-Headlines ArticleService kan deployes automatisk ved nye features"

---

## SLIDE 20: Finans (fx Banker, Fintech)
**Speaking Notes:**
- "Finans (fx Banker, Fintech)"
- "CI/CD: Automatiseret testing og compliance checks"
- "Security: Automated security scanning i pipeline"
- "Audit: Alle deployment actions logges"
- "Værdi: Compliance automatiseret, security vulnerabilities opdages tidligt, audit trail"
- "Eksempel: Happy-Headlines FMEA risk analysis dokumenterer barriers"

---

## SLIDE 21: Healthcare (fx Sygehuse, Medicinsk Software)
**Speaking Notes:**
- "Healthcare (fx Sygehuse, Medicinsk Software)"
- "CI/CD: Automatiseret testing af kritiske systemer"
- "Monitoring: Real-time health monitoring"
- "Rollback: Hurtig rollback ved fejl"
- "Værdi: Patient safety, compliance, zero-downtime deployments"
- "Eksempel: Automated health checks i pipeline, rollback mechanisms ved deployment failures"

---

## SLIDE 22: Manufacturing (fx Produktion, IoT)
**Speaking Notes:**
- "Manufacturing (fx Produktion, IoT)"
- "CI/CD: Deploy firmware updates automatisk"
- "Monitoring: IoT device monitoring"
- "Infrastructure as Code: Versioner device configurations"
- "Værdi: OTA updates, centraliseret device management, hurtigere bug fixes"
- "Dette er relevant for IoT og produktion"

---

## SLIDE 23: Media & Entertainment (fx Netflix, Spotify)
**Speaking Notes:**
- "Media & Entertainment (fx Netflix, Spotify)"
- "CI/CD: Deploy nye features ofte"
- "A/B Testing: Automatiseret feature flags"
- "Monitoring: Real-time streaming metrics"
- "Værdi: Hurtigere content delivery, bedre user experience, kan håndtere globale trafik-spikes"
- "Eksempel: Happy-Headlines NewsletterService kan deployes automatisk, ArticleService kan håndtere breaking news trafik"

---

## SLIDE 24: Government & Public Sector
**Speaking Notes:**
- "Government & Public Sector"
- "CI/CD: Automatiseret compliance checks"
- "Security: Automated security scanning"
- "Audit: Komplet audit trail"
- "Værdi: Transparency, security, compliance"
- "Dette er kritisk for offentlig sektor"

---

## SLIDE 25: Fælles Mønstre på tværs af Brancher
**Speaking Notes:**
- "Fælles Mønstre på tværs af Brancher"
- "Automation: Reducerer fejl - E-commerce: Hurtigere features, Finans: Compliance, Healthcare: Patient safety"
- "CI/CD: Hurtigere deployment - Alle: Time-to-market, Finans: Security, Healthcare: Zero-downtime"
- "Monitoring: Proaktiv problem-detektion - E-commerce: Salg-metrics, Healthcare: Patient monitoring, Manufacturing: Device health"
- "DevOps principper er universelle, men værdien varierer per branche"

---

## SLIDE 26: Konklusion - Brancher
**Speaking Notes:**
- "Konklusion - Brancher"
- "DevOps principper er universelle, men værdien varierer per branche"
- "E-commerce: Fokus på speed og skalering"
- "Finans: Fokus på security og compliance"
- "Healthcare: Fokus på safety og reliability"
- "Manufacturing: Fokus på device management"
- "Media: Fokus på user experience"
- "Government: Fokus på transparency og compliance"

---

## SLIDE 27: Happy-Headlines (Media/News)
**Speaking Notes:**
- "Happy-Headlines (Media/News)"
- "Anvender DevOps til:"
- "Hurtigere deployment af nye features"
- "Håndtering af trafik-spikes - breaking news"
- "Kontinuerlig monitoring - Prometheus, Grafana"
- "Automatiseret CI/CD pipeline"
- "Dette er relevant for media/news branchen"

---

## SLIDE 28: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres opmærksomhed"
- "Jeg er klar til spørgsmål"

---

## EKSTRA NOTER TIL EKSAMEN:

### Hvis de spørger om specifikke filer:
- **`.github/workflows/ci-cd.yml`**: CI/CD pipeline (linje 1-150)
- **`docs/CI-CD-Setup.md`**: Pipeline dokumentation

### Hvis de spørger om automation metrics:
- Tid: 30-60 minutter → 5-10 minutter (6x hurtigere)
- People: 1-2 udviklere → 0 udviklere (ingen manuel intervention)
- Fejl-risiko: Høj → Lav (konsistent process)
- Konsistens: Lav → Høj (samme process hver gang)

### Hvis de spørger om Venn-diagrammet:
- Performance: Automation gør det muligt at deploye 10x oftere
- Cost: Automation reducerer deployment cost med 80%
- People: Automation gør det muligt at håndtere 5x flere services med samme team

### Hvis de spørger om brancher:
- E-commerce: Speed og skalering
- Finans: Security og compliance
- Healthcare: Safety og reliability
- Manufacturing: Device management
- Media: User experience
- Government: Transparency og compliance
