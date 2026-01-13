# Spørgsmål 12: Virtualisering

## A. Forklar forskellen på virtualisering gennem virtuelle maskiner og containers

### Virtualisering - Definition:

**Virtualisering** er processen med at oprette en virtuel (software-baseret) version af noget, fx hardware, OS, eller applikationer. Dette gør det muligt at køre flere isolerede miljøer på samme fysiske hardware.

### Virtuelle Maskiner (VMs):

#### Arkitektur:

```
┌─────────────────────────────────────────┐
│         Host Operating System          │
│  (Windows, Linux, macOS)                │
├─────────────────────────────────────────┤
│         Hypervisor (VMM)                │
│  (VMware, VirtualBox, Hyper-V)         │
├─────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐     │
│  │   Guest OS   │  │   Guest OS   │     │
│  │   (Linux)    │  │  (Windows)   │     │
│  ├──────────────┤  ├──────────────┤     │
│  │  App 1       │  │  App 2       │     │
│  │  App 2       │  │  App 3       │     │
│  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────┘
```

#### Karakteristika:

1. **Full Virtualization:**
   - Hver VM har sit eget komplette OS (Guest OS)
   - Hypervisor virtualiserer hele hardwaren
   - Isolation på OS-niveau

2. **Resource Overhead:**
   - Høj overhead (hver VM kører fuldt OS)
   - Typisk 1-4 GB RAM per VM
   - Langsom start (boot OS)

3. **Isolation:**
   - Stærk isolation (komplet OS separation)
   - Sikkerhed på OS-niveau
   - Kan køre forskellige OS (Linux, Windows)

4. **Use Cases:**
   - Legacy applikationer (kræver specifikt OS)
   - Forskellige OS på samme hardware
   - Høj sikkerheds-isolation

### Containers:

#### Arkitektur:

```
┌─────────────────────────────────────────┐
│         Host Operating System           │
│  (Linux, Windows Server)               │
├─────────────────────────────────────────┤
│         Container Runtime               │
│  (Docker, containerd, CRI-O)            │
├─────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐    │
│  │  Container 1 │  │  Container 2 │    │
│  │  (App 1)     │  │  (App 2)     │    │
│  │  + Libs       │  │  + Libs      │    │
│  └──────────────┘  └──────────────┘    │
│         Shared OS Kernel                │
└─────────────────────────────────────────┘
```

#### Karakteristika:

1. **OS-Level Virtualization:**
   - Containers deler host OS kernel
   - Ingen Guest OS overhead
   - Isolation på process-niveau

2. **Resource Overhead:**
   - Lav overhead (kun app + dependencies)
   - Typisk 10-100 MB RAM per container
   - Hurtig start (sekunder)

3. **Isolation:**
   - Process isolation (namespaces, cgroups)
   - Mindre isolation end VMs
   - Alle containers deler samme OS

4. **Use Cases:**
   - Microservices
   - CI/CD pipelines
   - Cloud-native applikationer

### Sammenligning:

| Aspekt | Virtuelle Maskiner (VMs) | Containers |
|--------|-------------------------|------------|
| **Isolation** | OS-niveau (stærk) | Process-niveau (moderat) |
| **Overhead** | Høj (1-4 GB RAM) | Lav (10-100 MB RAM) |
| **Start Time** | Minutter (boot OS) | Sekunder |
| **OS** | Fuld Guest OS | Deler host OS kernel |
| **Security** | Høj (OS isolation) | Moderat (process isolation) |
| **Resource Usage** | Høj | Lav |
| **Portability** | Moderat (VM images) | Høj (container images) |
| **Use Case** | Legacy apps, forskellige OS | Microservices, cloud-native |
| **Scaling** | Langsom (tung) | Hurtig (let) |

### Konkrete Eksempler:

**VMs:**
- VMware vSphere: Enterprise virtualization
- VirtualBox: Development environments
- AWS EC2: Cloud VMs

**Containers:**
- Docker: Application containers
- Kubernetes: Container orchestration
- Docker Compose: Multi-container apps

---

## B. Demonstrer hvordan Docker kan bruges til at skabe et konsistent udviklings- og produktionsmiljø

### Problemstilling:

**Uden Docker:**
- **Development**: Windows 10, .NET 8.0, SQL Server 2019
- **Production**: Linux, .NET 8.0, SQL Server 2022
- **Problemer**: "Works on my machine", forskellige versions, setup kompleksitet

**Med Docker:**
- **Development**: Docker container (samme som production)
- **Production**: Docker container (samme som development)
- **Resultat**: Konsistent miljø, "works everywhere"

### Demonstration: Docker i Happy-Headlines

#### Eksempel 1: Dockerfile (Konsistent Build)

**Kodeeksempel:**
```dockerfile
# ArticleService/Dockerfile (linje 1-14)

# Stage 1: Base image (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Stage 2: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Stage 3: Final image
FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "ArticleService.dll"]
```

**Konsistens:**
- **Development**: `docker build` → Samme image som production
- **Production**: `docker build` → Samme image som development
- **Resultat**: Identisk miljø

**Multi-stage Build:**
- **Build stage**: SDK image (større, til compilation)
- **Final stage**: Runtime image (mindre, kun runtime)
- **Fordel**: Mindre production image (hurtigere deployment)

#### Eksempel 2: Docker Compose (Konsistent Environment)

**Kodeeksempel:**
```yaml
# docker-compose.yml (linje 1-18)

services:
  article-service:
    image: article-service:latest
    build: ./ArticleService
    environment:
      - DB_CONNECTION=Server=article-db-global;Database=Articles;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True
      - OTLP_ENDPOINT=http://jaeger:4317
      - REDIS_CONNECTION=redis:6379
    ports:
      - "8080:8080"
    depends_on:
      - article-db-global
      - redis

  article-db-global:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "Your_password123"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
```

**Konsistens:**
- **Development**: `docker-compose up` → Alle services kører lokalt
- **Production**: `docker-compose up` → Samme services, samme konfiguration
- **Resultat**: Identisk miljø (services, databases, networking)

**Alle Services:**
```yaml
# docker-compose.yml - 6 microservices + dependencies
services:
  article-service: ...
  comment-service: ...
  profanity-service: ...
  draft-service: ...
  publisher-service: ...
  newsletter-service: ...
  redis: ...
  rabbitmq: ...
  prometheus: ...
  grafana: ...
  jaeger: ...
  seq: ...
```

**Konsistens:**
- **Development**: Alle services kører lokalt → Test hele systemet
- **Production**: Samme services → Samme opførsel

#### Eksempel 3: CI/CD Pipeline (Konsistent Build Process)

**Kodeeksempel:**
```yaml
# .github/workflows/ci-cd.yml (linje 29-50)

strategy:
  matrix:
    service:
      - name: article-service
        path: ./ArticleService
        dockerfile: ./ArticleService/Dockerfile
      - name: comment-service
        path: ./CommentService
        dockerfile: ./CommentService/Dockerfile
      # ... 6 services bygges parallel

steps:
  - name: Build and push Docker image
    uses: docker/build-push-action@v5
    with:
      context: ${{ matrix.service.path }}
      file: ${{ matrix.service.dockerfile }}
      tags: ${{ env.REGISTRY }}/${{ env.IMAGE_PREFIX }}/${{ matrix.service.name }}:latest
      cache-from: type=gha
      cache-to: type=gha,mode=max
```

**Konsistens:**
- **Development**: `docker build` → Lokal image
- **CI/CD**: `docker build` → Samme process → Push til registry
- **Production**: `docker pull` → Samme image fra registry
- **Resultat**: Identisk image fra development til production

#### Eksempel 4: Environment Variables (Konsistent Konfiguration)

**Kodeeksempel:**
```yaml
# docker-compose.yml (linje 12-15)
article-service:
  environment:
    - DB_CONNECTION=Server=article-db-global;Database=Articles;...
    - OTLP_ENDPOINT=http://jaeger:4317
    - REDIS_CONNECTION=redis:6379
```

**Konsistens:**
- **Development**: Environment variables i docker-compose.yml
- **Production**: Samme environment variables (via docker-compose eller Kubernetes)
- **Resultat**: Samme konfiguration, forskellige værdier (fx connection strings)

**Best Practice:**
```yaml
# Development
environment:
  - DB_CONNECTION=Server=article-db-global;...

# Production (via .env file eller Kubernetes secrets)
environment:
  - DB_CONNECTION=Server=prod-db;...
```

#### Eksempel 5: Database Consistency

**Kodeeksempel:**
```yaml
# docker-compose.yml (linje 21-27)
article-db-global:
  image: mcr.microsoft.com/mssql/server:2022-latest
  environment:
    SA_PASSWORD: "Your_password123"
    ACCEPT_EULA: "Y"
  ports:
    - "1433:1433"
```

**Konsistens:**
- **Development**: SQL Server 2022 i container → Test mod samme version
- **Production**: SQL Server 2022 i container → Samme version
- **Resultat**: Ingen "works on my machine" problemer

**Alle Databases:**
```yaml
# docker-compose.yml - 8 databases (global + 7 kontinenter)
article-db-global: ...
article-db-europe: ...
article-db-asia: ...
# ... alle bruger samme image
```

### Workflow: Development → Production

#### Step 1: Development

```bash
# Developer lokalt
cd Happy-Headlines
docker-compose up

# Alle services kører:
# - article-service (localhost:8080)
# - comment-service (localhost:8081)
# - profanity-service (localhost:8082)
# - ... alle services + databases
```

**Konsistens:**
- Samme Docker images som production
- Samme docker-compose.yml konfiguration
- Samme environment variables (development værdier)

#### Step 2: CI/CD Build

```yaml
# .github/workflows/ci-cd.yml
# Automatisk build når code pushes
- name: Build and push Docker image
  uses: docker/build-push-action@v5
  # Build samme Dockerfile som development
  # Push til registry
```

**Konsistens:**
- Samme Dockerfile som development
- Samme build process
- Image tagget med version (SHA, latest)

#### Step 3: Production Deployment

```bash
# Production server
docker pull ghcr.io/owner/happy-headlines/article-service:latest
docker-compose -f docker-compose.prod.yml up

# Eller Kubernetes
kubectl set image deployment/article-service article-service=ghcr.io/owner/happy-headlines/article-service:latest
```

**Konsistens:**
- Samme image som development
- Samme konfiguration (forskellige environment variables)
- Samme opførsel

### Fordele ved Konsistent Miljø:

**1. "Works on my machine" → "Works everywhere":**
- Development = Production miljø
- Ingen surprises i production

**2. Hurtigere Onboarding:**
- Ny developer: `docker-compose up` → Alt kører
- Ingen kompleks setup

**3. Reproducerbarhed:**
- Samme image = Samme opførsel
- Test i development = Test i production

**4. Version Control:**
- Dockerfile i Git → Versioneret
- Docker images tagget → Kan rollback

**5. Isolation:**
- Hver service i sin egen container
- Ingen konflikter mellem services

---

## C. Diskuter fordele og ulemper ved at bruge Docker i en stor skalerbar applikation

### Fordele ved Docker i Stor Skalerbar Applikation:

#### 1. **Horizontal Skalering (X-akse)**

**Fordel:**
- Let at skalerer op/ned (flere container instances)
- Hurtig deployment af nye instances

**Eksempel fra kodebasen:**
```yaml
# docker-compose.yml (linje 8-9)
deploy:
  replicas: 1  # Kan øges til 3, 5, 10...
  restart_policy:
    condition: on-failure
```

**I Production (Kubernetes):**
```yaml
# Kubernetes deployment
replicas: 10  # 10 instances af article-service
```

**Fordel:**
- **1 instance**: 500 req/s
- **10 instances**: 5000 req/s (10x skalering)
- **Auto-scaling**: Kan skalerer baseret på trafik

#### 2. **Microservices Isolation (Y-akse)**

**Fordel:**
- Hver service i sin egen container
- Independent deployment og skalering

**Eksempel fra kodebasen:**
```yaml
# docker-compose.yml - 6 separate services
article-service: ...
comment-service: ...
profanity-service: ...
# ... hver service isoleret
```

**Fordel:**
- **ArticleService skalering**: Deploy kun article-service containers
- **CommentService skalering**: Deploy kun comment-service containers
- **Independent**: Ingen impact på andre services

#### 3. **Resource Efficiency**

**Fordel:**
- Lav overhead (10-100 MB per container)
- Kan køre mange containers på samme server

**Eksempel:**
```
Server med 16 GB RAM:
- VMs: 4 VMs × 4 GB = 16 GB (max 4 VMs)
- Containers: 100 containers × 100 MB = 10 GB (kan køre 100+ containers)
```

**Fordel:**
- **Højere density**: Flere applikationer per server
- **Lavere cost**: Færre servere nødvendige

#### 4. **Fast Deployment**

**Fordel:**
- Hurtig container start (sekunder)
- Hurtig skalering (start flere containers)

**Eksempel:**
```
VM deployment:
- Start VM: 2-5 minutter
- Deploy app: 5 minutter
- Total: 7-10 minutter

Container deployment:
- Start container: 5-10 sekunder
- Deploy app: 0 sekunder (i image)
- Total: 5-10 sekunder
```

**Fordel:**
- **Hurtigere skalering**: Kan reagere hurtigt på trafik-spikes
- **Hurtigere rollback**: Start gammel container version

#### 5. **Consistency Across Environments**

**Fordel:**
- Samme image i development, staging, production
- Ingen "works on my machine" problemer

**Eksempel:**
```
Development: docker-compose up
Staging: docker-compose -f docker-compose.staging.yml up
Production: Kubernetes (samme images)
```

**Fordel:**
- **Færre bugs**: Test i development = Test i production
- **Hurtigere deployment**: Ingen surprises

#### 6. **Orchestration Support**

**Fordel:**
- Docker works med Kubernetes, Docker Swarm, etc.
- Automatisk load balancing, service discovery

**Eksempel:**
```yaml
# Kubernetes deployment
apiVersion: apps/v1
kind: Deployment
metadata:
  name: article-service
spec:
  replicas: 10
  selector:
    matchLabels:
      app: article-service
  template:
    spec:
      containers:
      - name: article-service
        image: ghcr.io/owner/happy-headlines/article-service:latest
```

**Fordel:**
- **Auto-scaling**: Kubernetes skalerer automatisk
- **Load balancing**: Automatisk request distribution
- **Service discovery**: Automatisk service location

#### 7. **Version Control og Rollback**

**Fordel:**
- Docker images tagget med versions
- Let at rollback til tidligere version

**Eksempel:**
```yaml
# CI/CD pipeline
tags: 
  - article-service:latest
  - article-service:abc123d  # SHA tag
  - article-service:v1.2.3    # Semantic version
```

**Fordel:**
- **Rollback**: `kubectl set image ... article-service:v1.2.2`
- **Version tracking**: Kan se hvilken version kører

### Ulemper ved Docker i Stor Skalerbar Applikation:

#### 1. **Orchestration Complexity**

**Ulempe:**
- Kubernetes, Docker Swarm er komplekse
- Kræver ekspertise at operere

**Eksempel:**
```
Simple deployment:
docker-compose up  # Simpelt

Production deployment:
- Kubernetes cluster setup
- Service mesh (Istio, Linkerd)
- Monitoring (Prometheus, Grafana)
- Logging (ELK, Loki)
- Security (RBAC, network policies)
```

**Ulempe:**
- **Learning curve**: Kræver ekspertise
- **Operational overhead**: Kompleks at vedligeholde

#### 2. **Networking Complexity**

**Ulempe:**
- Container networking kan være kompleks
- Service discovery, load balancing

**Eksempel:**
```
docker-compose.yml:
- article-service → article-db-global (internal network)
- article-service → redis (internal network)
- article-service → jaeger (internal network)
```

**Ulempe:**
- **Network debugging**: Svært at debug container networking
- **Service discovery**: Kræver DNS eller service registry

#### 3. **State Management**

**Ulempe:**
- Containers er stateless (best practice)
- Stateful services (databases) kræver ekstra setup

**Eksempel:**
```
Stateless service (article-service):
- Let at skalerer (flere instances)
- Ingen state i container

Stateful service (article-db-global):
- Svært at skalerer (database replication)
- Data persistence (volumes)
```

**Ulempe:**
- **Database skalering**: Kræver database clustering/replication
- **Data persistence**: Kræver volumes, backup strategies

#### 4. **Security Concerns**

**Ulempe:**
- Containers deler host OS kernel
- Mindre isolation end VMs

**Eksempel:**
```
VM:
- Hvis en VM kompromitteres → Kun den VM påvirkes
- Stærk isolation

Container:
- Hvis en container kompromitteres → Kan påvirke host OS
- Mindre isolation
```

**Ulempe:**
- **Kernel exploits**: Kan påvirke alle containers
- **Privileged containers**: Kan få root access til host

**Mitigation:**
- Read-only filesystems
- Non-root users
- Security scanning (Trivy, Snyk)
- Network policies

#### 5. **Image Size**

**Ulempe:**
- Store images = Langsomere deployment
- Storage overhead

**Eksempel:**
```
Large image (500 MB):
- Pull time: 30 sekunder
- Storage: 500 MB × 10 instances = 5 GB

Small image (50 MB):
- Pull time: 3 sekunder
- Storage: 50 MB × 10 instances = 500 MB
```

**Ulempe:**
- **Deployment time**: Store images tager længere tid at pull
- **Storage cost**: Store images bruger mere disk

**Mitigation:**
- Multi-stage builds (mindre final image)
- Image optimization
- Layer caching

#### 6. **Debugging Complexity**

**Ulempe:**
- Svært at debug containerized apps
- Logs spredt over flere containers

**Eksempel:**
```
Debugging:
- Container logs: docker logs article-service
- Multiple containers: 6 services × 10 instances = 60 containers
- Distributed tracing: OpenTelemetry, Jaeger
```

**Ulempe:**
- **Log aggregation**: Kræver centraliseret logging (Seq, ELK)
- **Debugging tools**: Kræver container-aware tools

**Mitigation:**
- Centralized logging (Seq, ELK)
- Distributed tracing (Jaeger, Zipkin)
- Monitoring (Prometheus, Grafana)

#### 7. **Resource Limits**

**Ulempe:**
- Containers kan forbruge for mange ressourcer
- Kan påvirke andre containers

**Eksempel:**
```yaml
# docker-compose.yml
article-service:
  deploy:
    resources:
      limits:
        cpus: '2'
        memory: 2G
```

**Ulempe:**
- **Resource contention**: Containers konkurrerer om ressourcer
- **OOM kills**: Containers kan blive killed hvis de overforbruger

**Mitigation:**
- Resource limits
- Monitoring
- Auto-scaling

#### 8. **Vendor Lock-in**

**Ulempe:**
- Docker-specifik features kan skabe lock-in
- Migration til andet platform kan være svært

**Eksempel:**
```
Docker-specific:
- docker-compose.yml format
- Docker networking
- Docker volumes
```

**Ulempe:**
- **Migration**: Svært at migrere til andet container platform
- **Flexibility**: Bundet til Docker ecosystem

**Mitigation:**
- Brug standard APIs (OCI, CNCF)
- Undgå Docker-specifik features

### Sammenligning: Fordele vs. Ulemper

| Aspekt | Fordel | Ulempe |
|--------|--------|--------|
| **Skalering** | ✅ Let horizontal skalering | ⚠️ Orchestration kompleksitet |
| **Resource Efficiency** | ✅ Lav overhead | ⚠️ Resource limits nødvendige |
| **Deployment Speed** | ✅ Hurtig (sekunder) | ⚠️ Store images = langsom pull |
| **Consistency** | ✅ Samme miljø overalt | ⚠️ Debugging kompleksitet |
| **Isolation** | ✅ Process isolation | ⚠️ Mindre isolation end VMs |
| **Orchestration** | ✅ Kubernetes support | ⚠️ Learning curve |
| **State Management** | ✅ Stateless = let skalering | ⚠️ Stateful services komplekse |
| **Security** | ✅ Container security | ⚠️ Kernel exploits risiko |

### Best Practices for Stor Skalerbar Applikation:

**1. Orchestration:**
- Brug Kubernetes for production
- Automatisk scaling, load balancing

**2. Monitoring:**
- Prometheus for metrics
- Centralized logging (Seq, ELK)
- Distributed tracing (Jaeger)

**3. Security:**
- Non-root users
- Security scanning
- Network policies

**4. Resource Management:**
- Set resource limits
- Monitor resource usage
- Auto-scaling

**5. Image Optimization:**
- Multi-stage builds
- Small base images
- Layer caching

### Konklusion:

**Docker er godt til stor skalerbar applikation hvis:**
- ✅ Du har ekspertise i orchestration (Kubernetes)
- ✅ Du har monitoring og logging setup
- ✅ Du kan håndtere kompleksitet

**Docker er mindre godt hvis:**
- ❌ Du mangler ekspertise
- ❌ Du har simple applikationer (overkill)
- ❌ Du har høje sikkerhedskrav (overvej VMs)

**Anbefaling:**
For Happy-Headlines (6 microservices, skalering behov):
- ✅ **Brug Docker**: Microservices, skalering, consistency
- ✅ **Kubernetes**: For production orchestration
- ✅ **Monitoring**: Prometheus, Grafana, Jaeger, Seq
- ✅ **Security**: Scanning, non-root users, network policies

**Resultat:**
- **Skalering**: Let at skalerer op/ned
- **Consistency**: Samme miljø overalt
- **Deployment**: Hurtig (sekunder)
- **Complexity**: Høj (kræver ekspertise)
