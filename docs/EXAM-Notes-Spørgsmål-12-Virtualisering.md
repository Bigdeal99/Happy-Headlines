# EKSAMEN - Spørgsmål 12: Virtualisering
## Slide Outline & Speaking Notes

---

## SLIDE 1: Titel
**Speaking Notes:**
- "Jeg vil i dag præsentere Virtualisering"
- "Jeg dækker tre dele: forskellen mellem VMs og containers, hvordan Docker skaber konsistent miljø, og fordele/ulemper ved Docker i stor skalerbar applikation"
- "Jeg bruger Happy-Headlines microservices arkitektur som eksempel"

---

## PART A: FORKLAR FORSKEL PÅ VMs OG CONTAINERS

---

## SLIDE 2: Virtualisering - Definition
**Speaking Notes:**
- "Virtualisering er processen med at oprette en virtuel (software-baseret) version af noget, fx hardware, OS, eller applikationer"
- "Dette gør det muligt at køre flere isolerede miljøer på samme fysiske hardware"
- "Der er to hovedtyper: Virtuelle Maskiner (VMs) og Containers"

---

## SLIDE 3: Virtuelle Maskiner (VMs) - Arkitektur
**Speaking Notes:**
- "Virtuelle Maskiner (VMs) - Arkitektur:"
- "Host Operating System (Windows, Linux, macOS) → Hypervisor (VMM) - VMware, VirtualBox, Hyper-V → Guest OS (Linux, Windows) → Apps"
- "Hver VM har sit eget komplette OS (Guest OS)"
- "Hypervisor virtualiserer hele hardwaren"
- "Isolation på OS-niveau - stærk isolation"

---

## SLIDE 4: Virtuelle Maskiner (VMs) - Karakteristika
**Speaking Notes:**
- "Virtuelle Maskiner (VMs) - Karakteristika:"
- "Full Virtualization: Hver VM har sit eget komplette OS, hypervisor virtualiserer hele hardwaren, isolation på OS-niveau"
- "Resource Overhead: Høj overhead - hver VM kører fuldt OS, typisk 1-4 GB RAM per VM, langsom start - boot OS"
- "Isolation: Stærk isolation - komplet OS separation, sikkerhed på OS-niveau, kan køre forskellige OS (Linux, Windows)"
- "Use Cases: Legacy applikationer (kræver specifikt OS), forskellige OS på samme hardware, høj sikkerheds-isolation"

---

## SLIDE 5: Containers - Arkitektur
**Speaking Notes:**
- "Containers - Arkitektur:"
- "Host Operating System (Linux, Windows Server) → Container Runtime - Docker, containerd, CRI-O → Containers (App + Libs) → Shared OS Kernel"
- "Containers deler host OS kernel"
- "Ingen Guest OS overhead"
- "Isolation på process-niveau - namespaces, cgroups"

---

## SLIDE 6: Containers - Karakteristika
**Speaking Notes:**
- "Containers - Karakteristika:"
- "OS-Level Virtualization: Containers deler host OS kernel, ingen Guest OS overhead, isolation på process-niveau"
- "Resource Overhead: Lav overhead - kun app + dependencies, typisk 10-100 MB RAM per container, hurtig start - sekunder"
- "Isolation: Process isolation - namespaces, cgroups, mindre isolation end VMs, alle containers deler samme OS"
- "Use Cases: Microservices, CI/CD pipelines, cloud-native applikationer"

---

## SLIDE 7: Sammenligning - VMs vs. Containers
**Speaking Notes:**
- "Sammenligning: VMs vs. Containers"
- "Isolation: VMs - OS-niveau (stærk), Containers - Process-niveau (moderat)"
- "Overhead: VMs - Høj (1-4 GB RAM), Containers - Lav (10-100 MB RAM)"
- "Start Time: VMs - Minutter (boot OS), Containers - Sekunder"
- "OS: VMs - Fuld Guest OS, Containers - Deler host OS kernel"
- "Security: VMs - Høj (OS isolation), Containers - Moderat (process isolation)"
- "Resource Usage: VMs - Høj, Containers - Lav"
- "Portability: VMs - Moderat (VM images), Containers - Høj (container images)"
- "Use Case: VMs - Legacy apps, forskellige OS, Containers - Microservices, cloud-native"
- "Scaling: VMs - Langsom (tung), Containers - Hurtig (let)"

---

## SLIDE 8: Konkrete Eksempler
**Speaking Notes:**
- "Konkrete Eksempler:"
- "VMs: VMware vSphere - Enterprise virtualization, VirtualBox - Development environments, AWS EC2 - Cloud VMs"
- "Containers: Docker - Application containers, Kubernetes - Container orchestration, Docker Compose - Multi-container apps"
- "I Happy-Headlines bruger vi Docker og Docker Compose"

---

## PART B: DEMONSTRER DOCKER

---

## SLIDE 9: Problemstilling - Uden Docker
**Speaking Notes:**
- "Problemstilling: Uden Docker"
- "Development: Windows 10, .NET 8.0, SQL Server 2019"
- "Production: Linux, .NET 8.0, SQL Server 2022"
- "Problemer: 'Works on my machine', forskellige versions, setup kompleksitet"
- "Dette skaber inkonsistens mellem development og production"

---

## SLIDE 10: Løsning - Med Docker
**Speaking Notes:**
- "Løsning: Med Docker"
- "Development: Docker container (samme som production)"
- "Production: Docker container (samme som development)"
- "Resultat: Konsistent miljø, 'works everywhere'"
- "Dette eliminerer 'works on my machine' problemer"

---

## SLIDE 11: Eksempel 1 - Dockerfile (Konsistent Build)
**Speaking Notes:**
- "Eksempel 1: Dockerfile (Konsistent Build)"
- "Stage 1: Base image (runtime) - FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base"
- "Stage 2: Build stage - FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build, RUN dotnet restore, RUN dotnet publish"
- "Stage 3: Final image - FROM base AS final, COPY --from=build /out ."
- "Multi-stage Build: Build stage - SDK image (større, til compilation), Final stage - Runtime image (mindre, kun runtime)"
- "Fordel: Mindre production image (hurtigere deployment)"
- "Konsistens: Development docker build → samme image som production, Production docker build → samme image som development, Resultat: Identisk miljø"

---

## SLIDE 12: Eksempel 2 - Docker Compose (Konsistent Environment)
**Speaking Notes:**
- "Eksempel 2: Docker Compose (Konsistent Environment)"
- "Kodeeksempel: docker-compose.yml (linje 1-18) - article-service, article-db-global, redis, osv."
- "Alle Services: 6 microservices - article-service, comment-service, profanity-service, draft-service, publisher-service, newsletter-service"
- "Dependencies: redis, rabbitmq, prometheus, grafana, jaeger, seq"
- "Konsistens: Development docker-compose up → alle services kører lokalt, Production docker-compose up → samme services, samme konfiguration"
- "Resultat: Identisk miljø - services, databases, networking"
- "Development: Alle services kører lokalt → test hele systemet"
- "Production: Samme services → samme opførsel"

---

## SLIDE 13: Eksempel 3 - CI/CD Pipeline (Konsistent Build Process)
**Speaking Notes:**
- "Eksempel 3: CI/CD Pipeline (Konsistent Build Process)"
- "Kodeeksempel: .github/workflows/ci-cd.yml (linje 29-50) - Matrix strategy, Build and push Docker image"
- "Matrix Strategy: 6 services bygges parallel - article-service, comment-service, profanity-service, osv."
- "Konsistens: Development docker build → lokal image, CI/CD docker build → samme process → push til registry, Production docker pull → samme image fra registry"
- "Resultat: Identisk image fra development til production"
- "Samme Dockerfile som development, samme build process, image tagget med version (SHA, latest)"

---

## SLIDE 14: Eksempel 4 - Environment Variables (Konsistent Konfiguration)
**Speaking Notes:**
- "Eksempel 4: Environment Variables (Konsistent Konfiguration)"
- "Kodeeksempel: docker-compose.yml (linje 12-15) - Environment variables: DB_CONNECTION, OTLP_ENDPOINT, REDIS_CONNECTION"
- "Konsistens: Development - environment variables i docker-compose.yml, Production - samme environment variables (via docker-compose eller Kubernetes)"
- "Resultat: Samme konfiguration, forskellige værdier (fx connection strings)"
- "Best Practice: Development - DB_CONNECTION=Server=article-db-global;..., Production - DB_CONNECTION=Server=prod-db;... (via .env file eller Kubernetes secrets)"

---

## SLIDE 15: Eksempel 5 - Database Consistency
**Speaking Notes:**
- "Eksempel 5: Database Consistency"
- "Kodeeksempel: docker-compose.yml (linje 21-27) - article-db-global: image mcr.microsoft.com/mssql/server:2022-latest"
- "Alle Databases: 8 databases (global + 7 kontinenter) - article-db-global, article-db-europe, article-db-asia, osv. - alle bruger samme image"
- "Konsistens: Development - SQL Server 2022 i container → test mod samme version, Production - SQL Server 2022 i container → samme version"
- "Resultat: Ingen 'works on my machine' problemer"
- "Dette sikrer at database version er konsistent mellem development og production"

---

## SLIDE 16: Workflow - Development → Production
**Speaking Notes:**
- "Workflow: Development → Production"
- "Step 1: Development - Developer lokalt: cd Happy-Headlines, docker-compose up, alle services kører: article-service (localhost:8080), comment-service (localhost:8081), profanity-service (localhost:8082), osv."
- "Konsistens: Samme Docker images som production, samme docker-compose.yml konfiguration, samme environment variables (development værdier)"
- "Step 2: CI/CD Build - .github/workflows/ci-cd.yml, automatisk build når code pushes, Build and push Docker image"
- "Konsistens: Samme Dockerfile som development, samme build process, image tagget med version (SHA, latest)"
- "Step 3: Production Deployment - Production server: docker pull ghcr.io/owner/happy-headlines/article-service:latest, docker-compose -f docker-compose.prod.yml up, eller Kubernetes: kubectl set image deployment/article-service article-service=ghcr.io/owner/happy-headlines/article-service:latest"
- "Konsistens: Samme image som development, samme konfiguration (forskellige environment variables), samme opførsel"

---

## SLIDE 17: Fordele ved Konsistent Miljø
**Speaking Notes:**
- "Fordele ved Konsistent Miljø:"
- "1. 'Works on my machine' → 'Works everywhere': Development = Production miljø, ingen surprises i production"
- "2. Hurtigere Onboarding: Ny developer - docker-compose up → alt kører, ingen kompleks setup"
- "3. Reproducerbarhed: Samme image = samme opførsel, test i development = test i production"
- "4. Version Control: Dockerfile i Git → versioneret, Docker images tagget → kan rollback"
- "5. Isolation: Hver service i sin egen container, ingen konflikter mellem services"

---

## PART C: FORDELE OG ULEMPER

---

## SLIDE 18: Fordel 1 - Horizontal Skalering (X-akse)
**Speaking Notes:**
- "Fordel 1: Horizontal Skalering (X-akse)"
- "Fordel: Let at skalerer op/ned (flere container instances), hurtig deployment af nye instances"
- "Eksempel fra kodebasen: docker-compose.yml (linje 8-9) - deploy.replicas: 1 (kan øges til 3, 5, 10...)"
- "I Production (Kubernetes): replicas: 10 (10 instances af article-service)"
- "Fordel: 1 instance - 500 req/s, 10 instances - 5000 req/s (10x skalering), Auto-scaling - kan skalerer baseret på trafik"
- "Dette viser hvordan Docker gør det let at skalerer horizontal"

---

## SLIDE 19: Fordel 2 - Microservices Isolation (Y-akse)
**Speaking Notes:**
- "Fordel 2: Microservices Isolation (Y-akse)"
- "Fordel: Hver service i sin egen container, independent deployment og skalering"
- "Eksempel fra kodebasen: docker-compose.yml - 6 separate services - article-service, comment-service, profanity-service, draft-service, publisher-service, newsletter-service - hver service isoleret"
- "Fordel: ArticleService skalering - deploy kun article-service containers, CommentService skalering - deploy kun comment-service containers, Independent - ingen impact på andre services"
- "Dette viser hvordan Docker gør det let at isolere microservices"

---

## SLIDE 20: Fordel 3 - Resource Efficiency
**Speaking Notes:**
- "Fordel 3: Resource Efficiency"
- "Fordel: Lav overhead (10-100 MB per container), kan køre mange containers på samme server"
- "Eksempel: Server med 16 GB RAM - VMs: 4 VMs × 4 GB = 16 GB (max 4 VMs), Containers: 100 containers × 100 MB = 10 GB (kan køre 100+ containers)"
- "Fordel: Højere density - flere applikationer per server, lavere cost - færre servere nødvendige"
- "Dette viser hvordan Docker er mere resource-efficient end VMs"

---

## SLIDE 21: Fordel 4 - Fast Deployment
**Speaking Notes:**
- "Fordel 4: Fast Deployment"
- "Fordel: Hurtig container start (sekunder), hurtig skalering (start flere containers)"
- "Eksempel: VM deployment - start VM 2-5 minutter, deploy app 5 minutter, total 7-10 minutter"
- "Container deployment - start container 5-10 sekunder, deploy app 0 sekunder (i image), total 5-10 sekunder"
- "Fordel: Hurtigere skalering - kan reagere hurtigt på trafik-spikes, hurtigere rollback - start gammel container version"
- "Dette viser hvordan Docker gør deployment meget hurtigere"

---

## SLIDE 22: Fordel 5 - Consistency Across Environments
**Speaking Notes:**
- "Fordel 5: Consistency Across Environments"
- "Fordel: Samme image i development, staging, production, ingen 'works on my machine' problemer"
- "Eksempel: Development - docker-compose up, Staging - docker-compose -f docker-compose.staging.yml up, Production - Kubernetes (samme images)"
- "Fordel: Færre bugs - test i development = test i production, hurtigere deployment - ingen surprises"
- "Dette viser hvordan Docker sikrer konsistens på tværs af miljøer"

---

## SLIDE 23: Ulempe 1 - Orchestration Complexity
**Speaking Notes:**
- "Ulempe 1: Orchestration Complexity"
- "Ulempe: Kubernetes, Docker Swarm er komplekse, kræver ekspertise at operere"
- "Eksempel: Simple deployment - docker-compose up (simpelt)"
- "Production deployment - Kubernetes cluster setup, Service mesh (Istio, Linkerd), Monitoring (Prometheus, Grafana), Logging (ELK, Loki), Security (RBAC, network policies)"
- "Ulempe: Learning curve - kræver ekspertise, Operational overhead - kompleks at vedligeholde"
- "Dette viser at Docker kan være kompleks i production"

---

## SLIDE 24: Ulempe 2 - Networking Complexity
**Speaking Notes:**
- "Ulempe 2: Networking Complexity"
- "Ulempe: Container networking kan være kompleks, service discovery, load balancing"
- "Eksempel: docker-compose.yml - article-service → article-db-global (internal network), article-service → redis (internal network), article-service → jaeger (internal network)"
- "Ulempe: Network debugging - svært at debug container networking, Service discovery - kræver DNS eller service registry"
- "Dette viser at container networking kan være kompleks"

---

## SLIDE 25: Ulempe 3 - Security Concerns
**Speaking Notes:**
- "Ulempe 3: Security Concerns"
- "Ulempe: Containers deler host OS kernel, mindre isolation end VMs"
- "Eksempel: VM - hvis en VM kompromitteres → kun den VM påvirkes, stærk isolation"
- "Container - hvis en container kompromitteres → kan påvirke host OS, mindre isolation"
- "Ulempe: Kernel exploits - kan påvirke alle containers, Privileged containers - kan få root access til host"
- "Mitigation: Read-only filesystems, Non-root users, Security scanning (Trivy, Snyk), Network policies"
- "Dette viser at containers har mindre isolation end VMs"

---

## SLIDE 26: Ulempe 4 - Image Size
**Speaking Notes:**
- "Ulempe 4: Image Size"
- "Ulempe: Store images = langsommere deployment, storage overhead"
- "Eksempel: Large image (500 MB) - pull time 30 sekunder, storage 500 MB × 10 instances = 5 GB"
- "Small image (50 MB) - pull time 3 sekunder, storage 50 MB × 10 instances = 500 MB"
- "Ulempe: Deployment time - store images tager længere tid at pull, Storage cost - store images bruger mere disk"
- "Mitigation: Multi-stage builds - mindre final image (som i ArticleService/Dockerfile), Image optimization, Layer caching"
- "Dette viser at store images kan være et problem"

---

## SLIDE 27: Ulempe 5 - Debugging Complexity
**Speaking Notes:**
- "Ulempe 5: Debugging Complexity"
- "Ulempe: Svært at debug containerized apps, logs spredt over flere containers"
- "Eksempel: Debugging - Container logs: docker logs article-service, Multiple containers: 6 services × 10 instances = 60 containers, Distributed tracing: OpenTelemetry, Jaeger"
- "Ulempe: Log aggregation - kræver centraliseret logging (Seq, ELK), Debugging tools - kræver container-aware tools"
- "Mitigation: Centralized logging (Seq, ELK), Distributed tracing (Jaeger, Zipkin), Monitoring (Prometheus, Grafana)"
- "Dette viser at debugging kan være kompleks med containers"

---

## SLIDE 28: Sammenligning - Fordele vs. Ulemper
**Speaking Notes:**
- "Sammenligning: Fordele vs. Ulemper"
- "Skalering: ✅ Let horizontal skalering, ⚠️ Orchestration kompleksitet"
- "Resource Efficiency: ✅ Lav overhead, ⚠️ Resource limits nødvendige"
- "Deployment Speed: ✅ Hurtig (sekunder), ⚠️ Store images = langsom pull"
- "Consistency: ✅ Samme miljø overalt, ⚠️ Debugging kompleksitet"
- "Isolation: ✅ Process isolation, ⚠️ Mindre isolation end VMs"
- "Orchestration: ✅ Kubernetes support, ⚠️ Learning curve"
- "State Management: ✅ Stateless = let skalering, ⚠️ Stateful services komplekse"
- "Security: ✅ Container security, ⚠️ Kernel exploits risiko"
- "Dette viser en balanceret vurdering af Docker"

---

## SLIDE 29: Best Practices for Stor Skalerbar Applikation
**Speaking Notes:**
- "Best Practices for Stor Skalerbar Applikation:"
- "1. Orchestration: Brug Kubernetes for production, automatisk scaling, load balancing"
- "2. Monitoring: Prometheus for metrics, Centralized logging (Seq, ELK), Distributed tracing (Jaeger)"
- "3. Security: Non-root users, Security scanning, Network policies"
- "4. Resource Management: Set resource limits, Monitor resource usage, Auto-scaling"
- "5. Image Optimization: Multi-stage builds (som i ArticleService/Dockerfile), Small base images, Layer caching"
- "Dette er de vigtigste principper for at bruge Docker i stor skalerbar applikation"

---

## SLIDE 30: Konklusion
**Speaking Notes:**
- "Konklusion:"
- "Docker er godt til stor skalerbar applikation hvis: ✅ Du har ekspertise i orchestration (Kubernetes), ✅ Du har monitoring og logging setup, ✅ Du kan håndtere kompleksitet"
- "Docker er mindre godt hvis: ❌ Du mangler ekspertise, ❌ Du har simple applikationer (overkill), ❌ Du har høje sikkerhedskrav (overvej VMs)"
- "Anbefaling for Happy-Headlines (6 microservices, skalering behov): ✅ Brug Docker - microservices, skalering, consistency, ✅ Kubernetes - for production orchestration, ✅ Monitoring - Prometheus, Grafana, Jaeger, Seq, ✅ Security - scanning, non-root users, network policies"
- "Resultat: Skalering - let at skalerer op/ned, Consistency - samme miljø overalt, Deployment - hurtig (sekunder), Complexity - høj (kræver ekspertise)"

---

## SLIDE 31: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres opmærksomhed"
- "Jeg er klar til spørgsmål"

---

## EKSTRA NOTER TIL EKSAMEN:

### Hvis de spørger om specifikke filer:
- **`ArticleService/Dockerfile`**: Multi-stage build eksempel
- **`docker-compose.yml`**: Konsistent environment eksempel
- **`.github/workflows/ci-cd.yml`**: CI/CD pipeline eksempel

### Hvis de spørger om VMs vs. Containers:
- VMs: OS-niveau isolation, høj overhead (1-4 GB RAM), langsom start (minutter), fuld Guest OS
- Containers: Process-niveau isolation, lav overhead (10-100 MB RAM), hurtig start (sekunder), deler host OS kernel

### Hvis de spørger om Docker konsistens:
- Dockerfile: Samme build process → samme image
- Docker Compose: Samme services → samme miljø
- CI/CD Pipeline: Samme build process → samme image
- Environment Variables: Samme konfiguration → forskellige værdier
- Database Consistency: Samme database version → samme opførsel

### Hvis de spørger om fordele:
- Horizontal Skalering: Let at skalerer op/ned
- Microservices Isolation: Independent deployment
- Resource Efficiency: Lav overhead
- Fast Deployment: Hurtig (sekunder)
- Consistency: Samme miljø overalt
- Orchestration Support: Kubernetes support
- Version Control: Docker images tagget

### Hvis de spørger om ulemper:
- Orchestration Complexity: Kubernetes er kompleks
- Networking Complexity: Container networking kan være kompleks
- Security Concerns: Mindre isolation end VMs
- Image Size: Store images = langsommere deployment
- Debugging Complexity: Svært at debug containerized apps
- Resource Limits: Containers kan forbruge for mange ressourcer
- Vendor Lock-in: Docker-specifik features
