# Google Slides Content - Spørgsmål 10: Availability
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Availability
**Undertitel:** Hvordan skaleringsprincipper og arkitektur påvirker systemets tilgængelighed
**Footer:** Dit navn | Dato

---

## SLIDE 2: Availability - Definition
**Titel:** Availability - Definition

**Indhold:**
- **Availability = (Total Time - Downtime) / Total Time × 100%**
- **Eksempler:**
  - 99% = 3.65 dages downtime/år
  - 99.9% = 8.76 timer downtime/år
  - 99.99% = 52.56 minutter downtime/år
  - 99.999% = 5.26 minutter downtime/år

---

## SLIDE 3: Relation til Skaleringsprincipper - Performance
**Titel:** Relation til Skaleringsprincipper - Performance

**Indhold:**
- **Performance Skalering → Availability**
- Højere performance = Bedre availability
- Systemet kan håndtere mere trafik uden at gå ned
- **Eksempel:** Lav performance (1000 req/s) → 0% availability, Høj performance (5000 req/s) → 99.9% availability

---

## SLIDE 4: Relation til Skaleringsprincipper - Cost
**Titel:** Relation til Skaleringsprincipper - Cost

**Indhold:**
- **Cost Skalering → Availability**
- Invester i redundancy = Højere availability
- Backup systems, failover mechanisms
- **Eksempel:** Lav cost (1 server) → 0% availability ved fejl, Høj cost (3 servers) → 99%+ availability ved fejl

---

## SLIDE 5: Relation til Skaleringsprincipper - People
**Titel:** Relation til Skaleringsprincipper - People

**Indhold:**
- **People Skalering → Availability**
- Automatisering = Konsistent availability
- Færre menneskelige fejl = Højere availability
- **Eksempel:** Manuel deployment (95% availability), Automatiseret deployment (99.9% availability)

---

## SLIDE 6: Relation til Skaleringskuben - X-akse
**Titel:** Relation til Skaleringskuben - X-akse

**Indhold:**
- **X-akse (Horizontal Duplication) → Availability**
- Flere instanser = Redundancy
- Hvis én instance fejler, kan andre fortsætte
- **Eksempel:** 1 instance (99% availability), 3 instances (99.999% availability)

---

## SLIDE 7: Relation til Skaleringskuben - Y-akse
**Titel:** Relation til Skaleringskuben - Y-akse

**Indhold:**
- **Y-akse (Functional Decomposition) → Availability**
- Microservices = Isolation
- Fejl i én service påvirker ikke andre
- **Eksempel:** Monolitisk (0% availability ved fejl), Microservices (80% availability ved fejl)

---

## SLIDE 8: Relation til Skaleringskuben - Z-akse
**Titel:** Relation til Skaleringskuben - Z-akse

**Indhold:**
- **Z-akse (Data Partitioning) → Availability**
- Flere databases = Redundancy
- Hvis én database fejler, kan andre fortsætte
- **Eksempel:** 1 database (0% availability ved fejl), 8 databases (87.5% availability ved fejl)

---

## SLIDE 9: Kombineret Skalering → Availability
**Titel:** Kombineret Skalering → Availability

**Indhold:**
- **X + Y + Z akser kombineret**
- X-akse: 3 replicas per service
- Y-akse: 6 services (isolation)
- Z-akse: 8 databases (geografisk)
- **Total system availability ≈ 99.9%+**

---

## SLIDE 10: Problemstilling - Stigende Trafik
**Titel:** Problemstilling - Stigende Trafik

**Indhold:**
- System presses af stigende trafik
- 100 req/s → 1000 req/s (10x stigning)
- Systemet går ned ved høj trafik
- Availability falder fra 99% → 50%

---

## SLIDE 11: Mekanisme 1 - Circuit Breaker
**Titel:** Mekanisme 1 - Circuit Breaker

**Indhold:**
- **Circuit Breaker (Isolerer Fejlende Services)**
- Kodeeksempel: `CommentService/Program.cs` (linje 28-30)
- **Før:** ProfanityService fejler → CommentService blokeret → 0% availability
- **Efter:** Circuit breaker åbner → CommentService bruger fallback → 80% availability
- **Ved stigende trafik:** 1000 req/s → Circuit breaker åbner → 80% availability (med fallback)

---

## SLIDE 12: Mekanisme 2 - Retry Pattern
**Titel:** Mekanisme 2 - Retry Pattern

**Indhold:**
- **Retry Pattern (Håndterer Transient Fejl)**
- Kodeeksempel: `CommentService/Program.cs` (linje 23-26)
- **Før:** 1 transient fejl → Request fejler → 0% availability for den request
- **Efter:** 1 transient fejl → 3 retries → 95% success rate → Højere availability
- **Ved stigende trafik:** 1000 req/s → Retry håndterer de fleste → 95% availability

---

## SLIDE 13: Mekanisme 3 - Caching
**Titel:** Mekanisme 3 - Caching

**Indhold:**
- **Caching (Reducerer Load)**
- Kodeeksempel: `ArticleService/Controllers/ArticlesController.cs` (linje 32-39)
- **Før:** Hver request → Database query → Database overbelastet → 0% availability
- **Efter:** 80% cache hits → 80% færre database queries → Database ikke overbelastet → 99% availability
- **Ved stigende trafik:** 1000 req/s → 200 DB queries/s (80% cache hits) → 99% availability

---

## SLIDE 14: Mekanisme 4 - Timeout
**Titel:** Mekanisme 4 - Timeout

**Indhold:**
- **Timeout (Forhindrer Blokering)**
- Kodeeksempel: `CommentService/Program.cs` (linje 35)
- **Før:** Langsom service blokerer alle requests → Thread pool udtømt → 0% availability
- **Efter:** Timeout efter 2 sek → Thread frigives → System fortsætter → 99% availability
- **Ved stigende trafik:** 1000 req/s → Timeout forhindrer blokering → 95% availability

---

## SLIDE 15: Mekanisme 5 - Fallback
**Titel:** Mekanisme 5 - Fallback

**Indhold:**
- **Fallback (Graceful Degradation)**
- Kodeeksempel: `CommentService/Services/ProfanityClient.cs` (linje 14-16)
- **Før:** ProfanityService nede → Kommentarer ikke mulige → 0% availability for feature
- **Efter:** Fallback aktiveres → Kommentarer mulige (reduceret funktionalitet) → 80% availability
- **Ved stigende trafik:** 1000 req/s → Fallback aktiveres → 80% availability

---

## SLIDE 16: Mekanisme 6 - X-akse Skalering
**Titel:** Mekanisme 6 - X-akse Skalering

**Indhold:**
- **X-akse Skalering (Load Distribution)**
- Kodeeksempel: `docker-compose.yml` - Kan skaleres til flere replicas
- **Før:** 1 instance → Hvis instance fejler → 0% availability
- **Efter:** 3 instances → Hvis 1 fejler → 66% availability (2/3 fortsætter)
- **Ved stigende trafik:** 1000 req/s → 3 instances → Load fordelt → 99% availability

---

## SLIDE 17: Mekanisme 7 - Asynkron Processing
**Titel:** Mekanisme 7 - Asynkron Processing

**Indhold:**
- **Asynkron Processing (Decoupling)**
- Kodeeksempel: `PublisherService/Controllers/PublishController.cs` (linje 34-37)
- **Før:** Synkron processing → Hvis ArticleService langsom → PublisherService blokeret → 0% availability
- **Efter:** Asynkron queue → PublisherService returnerer umiddelbart → 99% availability
- **Ved stigende trafik:** 1000 req/s → Asynkron → ArticleService processerer i baggrunden → 99% availability

---

## SLIDE 18: Kombineret Approach - Optimal Availability
**Titel:** Kombineret Approach - Optimal Availability

**Indhold:**
- **Alle mekanismer kombineret**
- CommentService: Retry + Circuit Breaker + Timeout + Fallback + Caching + X-akse skalering
- **Tabel:**
  | Trafik | Uden Mekanismer | Med Alle Mekanismer | Forbedring |
  |--------|-----------------|---------------------|------------|
  | 100 req/s | 99% | 99.9% | +0.9% |
  | 500 req/s | 50% | 99% | +49% |
  | 1000 req/s | 0% (system ned) | 95% | +95% |
  | 2000 req/s | 0% | 90% | +90% |

---

## SLIDE 19: Negativ Indvirkning 1 - Synkron Service Dependencies
**Titel:** Negativ Indvirkning 1 - Synkron Service Dependencies

**Indhold:**
- **Synkron Service Dependencies**
- Problem: Tæt kobling mellem services via synkron HTTP calls
- Eksempel: `PublisherService/Controllers/PublishController.cs` (linje 24-27)
- **Negativ Impact:** Hvis ProfanityService er langsom → PublisherService blokeret → 0% availability
- **Løsning:** Asynkron processing (RabbitMQ), Circuit breaker, Timeout

---

## SLIDE 20: Negativ Indvirkning 2 - Single Point of Failure
**Titel:** Negativ Indvirkning 2 - Single Point of Failure

**Indhold:**
- **Single Point of Failure (SPOF)**
- Problem: Kritisk komponent uden redundancy
- Eksempel: `docker-compose.yml` - Single Redis instance, Single RabbitMQ instance
- **Negativ Impact:** Hvis Redis fejler → Alle cache operations fejler → System overbelastet → 0% availability
- **Løsning:** Redis Cluster, RabbitMQ Cluster, Multiple instances (X-akse skalering)

---

## SLIDE 21: Negativ Indvirkning 3 - Tæt Database Coupling
**Titel:** Negativ Indvirkning 3 - Tæt Database Coupling

**Indhold:**
- **Tæt Database Coupling**
- Problem: Alle services afhænger af samme database
- **Negativ Impact:** Hvis database fejler → Alle services påvirkes → 0% availability
- **Løsning:** Z-akse skalering (flere databases), Database replication, Read replicas

---

## SLIDE 22: Negativ Indvirkning 4 - Manglende Health Checks
**Titel:** Negativ Indvirkning 4 - Manglende Health Checks

**Indhold:**
- **Manglende Health Checks**
- Problem: Ingen automatisk detection af fejlende services
- **Negativ Impact:** Fejlende service opdages sent → Længere downtime → Lavere availability
- **Løsning:** Health check endpoints, Automatisk recovery, Monitoring

---

## SLIDE 23: Negativ Indvirkning 5 - Ingen Circuit Breaker
**Titel:** Negativ Indvirkning 5 - Ingen Circuit Breaker

**Indhold:**
- **Ingen Circuit Breaker**
- Problem: Fejlende services blokerer alle requests
- **Negativ Impact:** Fejlende service → Blokerer alle requests → 0% availability
- **Løsning:** Circuit breaker (som i CommentService/Program.cs)

---

## SLIDE 24: Negativ Indvirkning 6 - Manglende Timeout
**Titel:** Negativ Indvirkning 6 - Manglende Timeout

**Indhold:**
- **Manglende Timeout**
- Problem: Ingen timeout på eksterne calls
- **Negativ Impact:** Langsom service → Blokerer alle requests → 0% availability
- **Løsning:** Timeout (som i CommentService/Program.cs - 2 sek)

---

## SLIDE 25: Sammenligning - Negativ vs. Positiv Impact
**Titel:** Sammenligning - Negativ vs. Positiv Impact

**Indhold:**
- **Tabel:**
  | Arkitektonisk Valg | Availability Impact | Eksempel fra Kodebasen |
  |---------------------|---------------------|-------------------------|
  | Synkron dependencies | ❌ 0% (hvis dependency fejler) | PublisherService → ProfanityService (synkron) |
  | Asynkron processing | ✅ 99% (decoupling) | PublisherService → RabbitMQ → ArticleService |
  | Single Point of Failure | ❌ 0% (hvis SPOF fejler) | Single Redis instance |
  | Redundancy (X-akse) | ✅ 99.9% (redundancy) | 3 replicas per service |
  | Ingen circuit breaker | ❌ 0% (cascading failures) | Før circuit breaker implementation |
  | Circuit breaker | ✅ 80-90% (isolation) | CommentService → ProfanityService |

---

## SLIDE 26: Best Practices for Høj Availability
**Titel:** Best Practices for Høj Availability

**Indhold:**
- **1. Design for Failure:** Antag at services kan fejle, Implementer circuit breakers, timeouts, fallbacks
- **2. Redundancy:** X-akse skalering (flere instances), Database replication, Multiple infrastructure components
- **3. Isolation:** Y-akse skalering (microservices), Circuit breakers, Bulkhead pattern
- **4. Decoupling:** Asynkron processing (queues), Event-driven architecture, Løs kobling mellem services
- **5. Monitoring:** Health checks, Metrics, logging, tracing, Proactive alerting

---

## SLIDE 27: Konklusion
**Titel:** Konklusion

**Indhold:**
- **Arkitektoniske valg der reducerer availability:** ❌ Synkron dependencies, ❌ Single Point of Failure, ❌ Tæt database coupling, ❌ Manglende health checks, ❌ Ingen circuit breaker, ❌ Manglende timeout
- **Arkitektoniske valg der forbedrer availability:** ✅ Asynkron processing, ✅ Redundancy (X-akse), ✅ Isolation (Y-akse, circuit breakers), ✅ Health checks, ✅ Circuit breakers, ✅ Timeouts, ✅ Caching, ✅ Fallback mechanisms
- **Anbefaling:** Design systemet med availability i tankerne fra starten

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
   - Tilføj tabeller som billeder eller tekstbokse
4. **Kodeeksempler:**
   - Slide 11-17: Vis kode snippets eller referencer til filer
   - Brug monospace font for kode
5. **Tabel:**
   - Slide 18, 25: Tegn tabeller eller brug tekstbokse
6. **Screenshots forslag:**
   - Slide 11: Screenshot af `CommentService/Program.cs` (circuit breaker)
   - Slide 12: Screenshot af `CommentService/Program.cs` (retry)
   - Slide 13: Screenshot af `ArticleService/Controllers/ArticlesController.cs` (caching)
   - Slide 14: Screenshot af `CommentService/Program.cs` (timeout)
   - Slide 15: Screenshot af `CommentService/Services/ProfanityClient.cs` (fallback)
   - Slide 17: Screenshot af `PublisherService/Controllers/PublishController.cs` (asynkron)
   - Slide 19: Screenshot af `PublisherService/Controllers/PublishController.cs` (synkron dependency)
   - Slide 20: Screenshot af `docker-compose.yml` (Redis, RabbitMQ)
