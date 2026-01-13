# Google Slides Content - Spørgsmål 12: Virtualisering
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Virtualisering
**Undertitel:** VMs vs. Containers og Docker i skalerbar applikation
**Footer:** Dit navn | Dato

---

## SLIDE 2: Virtualisering - Definition
**Titel:** Virtualisering - Definition

**Indhold:**
- **Virtualisering = Processen med at oprette en virtuel (software-baseret) version af noget**
- Eksempler: Hardware, OS, eller applikationer
- Gør det muligt at køre flere isolerede miljøer på samme fysiske hardware

---

## SLIDE 3: Virtuelle Maskiner (VMs) - Arkitektur
**Titel:** Virtuelle Maskiner (VMs) - Arkitektur

**Indhold:**
- **Diagram:** Host OS → Hypervisor (VMM) → Guest OS → Apps
- Hypervisor: VMware, VirtualBox, Hyper-V
- Hver VM har sit eget komplette OS (Guest OS)
- Isolation på OS-niveau

---

## SLIDE 4: Virtuelle Maskiner (VMs) - Karakteristika
**Titel:** Virtuelle Maskiner (VMs) - Karakteristika

**Indhold:**
- **Full Virtualization:** Hver VM har sit eget komplette OS
- **Resource Overhead:** Høj overhead (hver VM kører fuldt OS), typisk 1-4 GB RAM per VM, langsom start (boot OS)
- **Isolation:** Stærk isolation (komplet OS separation), sikkerhed på OS-niveau, kan køre forskellige OS
- **Use Cases:** Legacy applikationer, forskellige OS på samme hardware, høj sikkerheds-isolation

---

## SLIDE 5: Containers - Arkitektur
**Titel:** Containers - Arkitektur

**Indhold:**
- **Diagram:** Host OS → Container Runtime → Containers (Apps + Libs) → Shared OS Kernel
- Container Runtime: Docker, containerd, CRI-O
- Containers deler host OS kernel
- Isolation på process-niveau

---

## SLIDE 6: Containers - Karakteristika
**Titel:** Containers - Karakteristika

**Indhold:**
- **OS-Level Virtualization:** Containers deler host OS kernel, ingen Guest OS overhead, isolation på process-niveau
- **Resource Overhead:** Lav overhead (kun app + dependencies), typisk 10-100 MB RAM per container, hurtig start (sekunder)
- **Isolation:** Process isolation (namespaces, cgroups), mindre isolation end VMs, alle containers deler samme OS
- **Use Cases:** Microservices, CI/CD pipelines, cloud-native applikationer

---

## SLIDE 7: Sammenligning - VMs vs. Containers
**Titel:** Sammenligning - VMs vs. Containers

**Indhold:**
- **Tabel:**
  | Aspekt | VMs | Containers |
  |--------|-----|------------|
  | Isolation | OS-niveau (stærk) | Process-niveau (moderat) |
  | Overhead | Høj (1-4 GB RAM) | Lav (10-100 MB RAM) |
  | Start Time | Minutter (boot OS) | Sekunder |
  | OS | Fuld Guest OS | Deler host OS kernel |
  | Security | Høj (OS isolation) | Moderat (process isolation) |
  | Resource Usage | Høj | Lav |
  | Portability | Moderat (VM images) | Høj (container images) |
  | Use Case | Legacy apps, forskellige OS | Microservices, cloud-native |
  | Scaling | Langsom (tung) | Hurtig (let) |

---

## SLIDE 8: Konkrete Eksempler
**Titel:** Konkrete Eksempler

**Indhold:**
- **VMs:** VMware vSphere (Enterprise virtualization), VirtualBox (Development environments), AWS EC2 (Cloud VMs)
- **Containers:** Docker (Application containers), Kubernetes (Container orchestration), Docker Compose (Multi-container apps)

---

## SLIDE 9: Problemstilling - Uden Docker
**Titel:** Problemstilling - Uden Docker

**Indhold:**
- **Uden Docker:**
  - Development: Windows 10, .NET 8.0, SQL Server 2019
  - Production: Linux, .NET 8.0, SQL Server 2022
- **Problemer:** "Works on my machine", forskellige versions, setup kompleksitet

---

## SLIDE 10: Løsning - Med Docker
**Titel:** Løsning - Med Docker

**Indhold:**
- **Med Docker:**
  - Development: Docker container (samme som production)
  - Production: Docker container (samme som development)
- **Resultat:** Konsistent miljø, "works everywhere"

---

## SLIDE 11: Eksempel 1 - Dockerfile (Konsistent Build)
**Titel:** Eksempel 1 - Dockerfile (Konsistent Build)

**Indhold:**
- **Kodeeksempel:** `ArticleService/Dockerfile` (linje 1-14)
- **Multi-stage Build:** Build stage (SDK image - større, til compilation), Final stage (Runtime image - mindre, kun runtime)
- **Konsistens:** Development `docker build` → samme image som production, Production `docker build` → samme image som development, Resultat: Identisk miljø

---

## SLIDE 12: Eksempel 2 - Docker Compose (Konsistent Environment)
**Titel:** Eksempel 2 - Docker Compose (Konsistent Environment)

**Indhold:**
- **Kodeeksempel:** `docker-compose.yml` (linje 1-18)
- **Alle Services:** 6 microservices + dependencies (redis, rabbitmq, prometheus, grafana, jaeger, seq)
- **Konsistens:** Development `docker-compose up` → alle services kører lokalt, Production `docker-compose up` → samme services, samme konfiguration, Resultat: Identisk miljø (services, databases, networking)

---

## SLIDE 13: Eksempel 3 - CI/CD Pipeline (Konsistent Build Process)
**Titel:** Eksempel 3 - CI/CD Pipeline (Konsistent Build Process)

**Indhold:**
- **Kodeeksempel:** `.github/workflows/ci-cd.yml` (linje 29-50)
- **Matrix Strategy:** 6 services bygges parallel
- **Konsistens:** Development `docker build` → lokal image, CI/CD `docker build` → samme process → push til registry, Production `docker pull` → samme image fra registry, Resultat: Identisk image fra development til production

---

## SLIDE 14: Eksempel 4 - Environment Variables (Konsistent Konfiguration)
**Titel:** Eksempel 4 - Environment Variables (Konsistent Konfiguration)

**Indhold:**
- **Kodeeksempel:** `docker-compose.yml` (linje 12-15) - Environment variables
- **Konsistens:** Development (environment variables i docker-compose.yml), Production (samme environment variables via docker-compose eller Kubernetes), Resultat: Samme konfiguration, forskellige værdier (fx connection strings)
- **Best Practice:** Development (DB_CONNECTION=Server=article-db-global;...), Production (DB_CONNECTION=Server=prod-db;...)

---

## SLIDE 15: Eksempel 5 - Database Consistency
**Titel:** Eksempel 5 - Database Consistency

**Indhold:**
- **Kodeeksempel:** `docker-compose.yml` (linje 21-27) - article-db-global: image mcr.microsoft.com/mssql/server:2022-latest
- **Alle Databases:** 8 databases (global + 7 kontinenter) - alle bruger samme image
- **Konsistens:** Development (SQL Server 2022 i container → test mod samme version), Production (SQL Server 2022 i container → samme version), Resultat: Ingen "works on my machine" problemer

---

## SLIDE 16: Workflow - Development → Production
**Titel:** Workflow - Development → Production

**Indhold:**
- **Step 1: Development** - `docker-compose up` → alle services kører lokalt, samme Docker images som production, samme docker-compose.yml konfiguration
- **Step 2: CI/CD Build** - automatisk build når code pushes, samme Dockerfile som development, image tagget med version (SHA, latest)
- **Step 3: Production Deployment** - `docker pull` → samme image fra registry, samme konfiguration (forskellige environment variables), samme opførsel

---

## SLIDE 17: Fordele ved Konsistent Miljø
**Titel:** Fordele ved Konsistent Miljø

**Indhold:**
- **1. "Works on my machine" → "Works everywhere":** Development = Production miljø, ingen surprises i production
- **2. Hurtigere Onboarding:** Ny developer `docker-compose up` → alt kører, ingen kompleks setup
- **3. Reproducerbarhed:** Samme image = samme opførsel, test i development = test i production
- **4. Version Control:** Dockerfile i Git → versioneret, Docker images tagget → kan rollback
- **5. Isolation:** Hver service i sin egen container, ingen konflikter mellem services

---

## SLIDE 18: Fordel 1 - Horizontal Skalering (X-akse)
**Titel:** Fordel 1 - Horizontal Skalering (X-akse)

**Indhold:**
- **Horizontal Skalering (X-akse)**
- Fordel: Let at skalerer op/ned (flere container instances), hurtig deployment af nye instances
- Eksempel: `docker-compose.yml` (linje 8-9) - deploy.replicas: 1 (kan øges til 3, 5, 10...)
- I Production (Kubernetes): replicas: 10 (10 instances af article-service)
- Fordel: 1 instance (500 req/s), 10 instances (5000 req/s - 10x skalering), Auto-scaling (kan skalerer baseret på trafik)

---

## SLIDE 19: Fordel 2 - Microservices Isolation (Y-akse)
**Titel:** Fordel 2 - Microservices Isolation (Y-akse)

**Indhold:**
- **Microservices Isolation (Y-akse)**
- Fordel: Hver service i sin egen container, independent deployment og skalering
- Eksempel: `docker-compose.yml` - 6 separate services (article-service, comment-service, profanity-service, osv.)
- Fordel: ArticleService skalering (deploy kun article-service containers), CommentService skalering (deploy kun comment-service containers), Independent (ingen impact på andre services)

---

## SLIDE 20: Fordel 3 - Resource Efficiency
**Titel:** Fordel 3 - Resource Efficiency

**Indhold:**
- **Resource Efficiency**
- Fordel: Lav overhead (10-100 MB per container), kan køre mange containers på samme server
- Eksempel: Server med 16 GB RAM - VMs (4 VMs × 4 GB = 16 GB - max 4 VMs), Containers (100 containers × 100 MB = 10 GB - kan køre 100+ containers)
- Fordel: Højere density (flere applikationer per server), lavere cost (færre servere nødvendige)

---

## SLIDE 21: Fordel 4 - Fast Deployment
**Titel:** Fordel 4 - Fast Deployment

**Indhold:**
- **Fast Deployment**
- Fordel: Hurtig container start (sekunder), hurtig skalering (start flere containers)
- Eksempel: VM deployment (start VM 2-5 minutter, deploy app 5 minutter, total 7-10 minutter), Container deployment (start container 5-10 sekunder, deploy app 0 sekunder - i image, total 5-10 sekunder)
- Fordel: Hurtigere skalering (kan reagere hurtigt på trafik-spikes), hurtigere rollback (start gammel container version)

---

## SLIDE 22: Fordel 5 - Consistency Across Environments
**Titel:** Fordel 5 - Consistency Across Environments

**Indhold:**
- **Consistency Across Environments**
- Fordel: Samme image i development, staging, production, ingen "works on my machine" problemer
- Eksempel: Development (docker-compose up), Staging (docker-compose -f docker-compose.staging.yml up), Production (Kubernetes - samme images)
- Fordel: Færre bugs (test i development = test i production), hurtigere deployment (ingen surprises)

---

## SLIDE 23: Ulempe 1 - Orchestration Complexity
**Titel:** Ulempe 1 - Orchestration Complexity

**Indhold:**
- **Orchestration Complexity**
- Ulempe: Kubernetes, Docker Swarm er komplekse, kræver ekspertise at operere
- Eksempel: Simple deployment (docker-compose up - simpelt), Production deployment (Kubernetes cluster setup, Service mesh Istio/Linkerd, Monitoring Prometheus/Grafana, Logging ELK/Loki, Security RBAC/network policies)
- Ulempe: Learning curve (kræver ekspertise), Operational overhead (kompleks at vedligeholde)

---

## SLIDE 24: Ulempe 2 - Networking Complexity
**Titel:** Ulempe 2 - Networking Complexity

**Indhold:**
- **Networking Complexity**
- Ulempe: Container networking kan være kompleks, service discovery, load balancing
- Eksempel: `docker-compose.yml` - article-service → article-db-global (internal network), article-service → redis (internal network), article-service → jaeger (internal network)
- Ulempe: Network debugging (svært at debug container networking), Service discovery (kræver DNS eller service registry)

---

## SLIDE 25: Ulempe 3 - Security Concerns
**Titel:** Ulempe 3 - Security Concerns

**Indhold:**
- **Security Concerns**
- Ulempe: Containers deler host OS kernel, mindre isolation end VMs
- Eksempel: VM (hvis en VM kompromitteres → kun den VM påvirkes, stærk isolation), Container (hvis en container kompromitteres → kan påvirke host OS, mindre isolation)
- Ulempe: Kernel exploits (kan påvirke alle containers), Privileged containers (kan få root access til host)
- Mitigation: Read-only filesystems, Non-root users, Security scanning (Trivy, Snyk), Network policies

---

## SLIDE 26: Ulempe 4 - Image Size
**Titel:** Ulempe 4 - Image Size

**Indhold:**
- **Image Size**
- Ulempe: Store images = langsommere deployment, storage overhead
- Eksempel: Large image 500 MB (pull time 30 sekunder, storage 500 MB × 10 instances = 5 GB), Small image 50 MB (pull time 3 sekunder, storage 50 MB × 10 instances = 500 MB)
- Ulempe: Deployment time (store images tager længere tid at pull), Storage cost (store images bruger mere disk)
- Mitigation: Multi-stage builds (mindre final image), Image optimization, Layer caching

---

## SLIDE 27: Ulempe 5 - Debugging Complexity
**Titel:** Ulempe 5 - Debugging Complexity

**Indhold:**
- **Debugging Complexity**
- Ulempe: Svært at debug containerized apps, logs spredt over flere containers
- Eksempel: Debugging - Container logs (docker logs article-service), Multiple containers (6 services × 10 instances = 60 containers), Distributed tracing (OpenTelemetry, Jaeger)
- Ulempe: Log aggregation (kræver centraliseret logging Seq/ELK), Debugging tools (kræver container-aware tools)
- Mitigation: Centralized logging (Seq, ELK), Distributed tracing (Jaeger, Zipkin), Monitoring (Prometheus, Grafana)

---

## SLIDE 28: Sammenligning - Fordele vs. Ulemper
**Titel:** Sammenligning - Fordele vs. Ulemper

**Indhold:**
- **Tabel:**
  | Aspekt | Fordel | Ulempe |
  |--------|--------|--------|
  | Skalering | ✅ Let horizontal skalering | ⚠️ Orchestration kompleksitet |
  | Resource Efficiency | ✅ Lav overhead | ⚠️ Resource limits nødvendige |
  | Deployment Speed | ✅ Hurtig (sekunder) | ⚠️ Store images = langsom pull |
  | Consistency | ✅ Samme miljø overalt | ⚠️ Debugging kompleksitet |
  | Isolation | ✅ Process isolation | ⚠️ Mindre isolation end VMs |
  | Orchestration | ✅ Kubernetes support | ⚠️ Learning curve |
  | State Management | ✅ Stateless = let skalering | ⚠️ Stateful services komplekse |
  | Security | ✅ Container security | ⚠️ Kernel exploits risiko |

---

## SLIDE 29: Best Practices for Stor Skalerbar Applikation
**Titel:** Best Practices for Stor Skalerbar Applikation

**Indhold:**
- **1. Orchestration:** Brug Kubernetes for production, automatisk scaling, load balancing
- **2. Monitoring:** Prometheus for metrics, Centralized logging (Seq, ELK), Distributed tracing (Jaeger)
- **3. Security:** Non-root users, Security scanning, Network policies
- **4. Resource Management:** Set resource limits, Monitor resource usage, Auto-scaling
- **5. Image Optimization:** Multi-stage builds, Small base images, Layer caching

---

## SLIDE 30: Konklusion
**Titel:** Konklusion

**Indhold:**
- **Docker er godt til stor skalerbar applikation hvis:** ✅ Du har ekspertise i orchestration (Kubernetes), ✅ Du har monitoring og logging setup, ✅ Du kan håndtere kompleksitet
- **Docker er mindre godt hvis:** ❌ Du mangler ekspertise, ❌ Du har simple applikationer (overkill), ❌ Du har høje sikkerhedskrav (overvej VMs)
- **Anbefaling for Happy-Headlines:** ✅ Brug Docker (microservices, skalering, consistency), ✅ Kubernetes (for production orchestration), ✅ Monitoring (Prometheus, Grafana, Jaeger, Seq), ✅ Security (scanning, non-root users, network policies)
- **Resultat:** Skalering (let at skalerer op/ned), Consistency (samme miljø overalt), Deployment (hurtig - sekunder), Complexity (høj - kræver ekspertise)

---

## SLIDE 31: Spørgsmål?
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
4. **Diagrammer:**
   - Slide 3, 5: Tegn arkitektur-diagrammer eller brug tekst
5. **Kodeeksempler:**
   - Slide 11-15: Vis kode snippets eller referencer til filer
   - Brug monospace font for kode
6. **Tabel:**
   - Slide 7, 28: Tegn tabeller eller brug tekstbokse
7. **Screenshots forslag:**
   - Slide 11: Screenshot af `ArticleService/Dockerfile` (multi-stage build)
   - Slide 12: Screenshot af `docker-compose.yml` (services konfiguration)
   - Slide 13: Screenshot af `.github/workflows/ci-cd.yml` (CI/CD pipeline)
   - Slide 14: Screenshot af `docker-compose.yml` (environment variables)
   - Slide 15: Screenshot af `docker-compose.yml` (database configuration)
