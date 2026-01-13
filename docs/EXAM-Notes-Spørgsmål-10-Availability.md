# EKSAMEN - Spørgsmål 10: Availability
## Slide Outline & Speaking Notes

---

## SLIDE 1: Titel
**Speaking Notes:**
- "Jeg vil i dag præsentere Availability"
- "Jeg dækker tre dele: definition og relation til skaleringsprincipper, mekanismer til forbedring ved stigende trafik, og negative indvirkninger af arkitektoniske valg"
- "Jeg bruger Happy-Headlines microservices arkitektur som eksempel"

---

## PART A: FORKLAR AVAILABILITY

---

## SLIDE 2: Availability - Definition
**Speaking Notes:**
- "Availability er procentdelen af tid hvor systemet er operationelt og kan håndtere requests"
- "Formel: Availability = (Total Time - Downtime) / Total Time × 100%"
- "Eksempler: 99% = 3.65 dages downtime/år, 99.9% = 8.76 timer downtime/år, 99.99% = 52.56 minutter downtime/år, 99.999% = 5.26 minutter downtime/år"
- "Højere availability = mindre downtime = bedre brugeroplevelse"

---

## SLIDE 3: Relation til Skaleringsprincipper - Performance
**Speaking Notes:**
- "Relation til Skaleringsprincipper: Performance Skalering → Availability"
- "Princippet: Højere performance = Bedre availability - systemet kan håndtere mere trafik uden at gå ned"
- "Eksempel: Lav performance - system går ned ved 1000 req/s → 0% availability, Høj performance - system håndterer 5000 req/s → 99.9% availability"
- "Dette viser at performance direkte påvirker availability"

---

## SLIDE 4: Relation til Skaleringsprincipper - Cost
**Speaking Notes:**
- "Relation til Skaleringsprincipper: Cost Skalering → Availability"
- "Princippet: Invester i redundancy = Højere availability - backup systems, failover mechanisms"
- "Eksempel: Lav cost - 1 server, hvis server fejler = 0% availability, Høj cost - 3 servers (redundancy), hvis 1 fejler = 99%+ availability"
- "Dette viser at investering i redundancy forbedrer availability"

---

## SLIDE 5: Relation til Skaleringsprincipper - People
**Speaking Notes:**
- "Relation til Skaleringsprincipper: People Skalering → Availability"
- "Princippet: Automatisering = Konsistent availability - færre menneskelige fejl = højere availability"
- "Eksempel: Manuel deployment - fejl-risiko → 95% availability, Automatiseret deployment - konsistent → 99.9% availability"
- "Dette viser at automatisering reducerer fejl og forbedrer availability"

---

## SLIDE 6: Relation til Skaleringskuben - X-akse
**Speaking Notes:**
- "Relation til Skaleringskuben: X-akse (Horizontal Duplication) → Availability"
- "Princippet: Flere instanser = Redundancy - hvis én instance fejler, kan andre fortsætte"
- "Availability Impact: 1 instance - Availability = 99% (1% downtime), 3 instances - Availability = 99.999% (hvis 1 fejler, 2 andre fortsætter)"
- "Eksempel fra kodebasen: docker-compose.yml - deploy.replicas: 1 (kun 1 replica for Prometheus), med flere replicas: Højere availability"
- "Forbedring: 1 replica - hvis container fejler → 0% availability, 3 replicas - hvis 1 container fejler → 66% availability (2/3 fortsætter)"

---

## SLIDE 7: Relation til Skaleringskuben - Y-akse
**Speaking Notes:**
- "Relation til Skaleringskuben: Y-akse (Functional Decomposition) → Availability"
- "Princippet: Microservices = Isolation - fejl i én service påvirker ikke andre"
- "Availability Impact: Monolitisk - hvis én del fejler → Hele systemet nede → 0% availability, Microservices - hvis ArticleService fejler → Kommentarer fortsætter → 80% availability"
- "Eksempel fra kodebasen: Happy-Headlines (6 services) - ArticleService fejler → Kommentarer, Newsletter fortsætter, ProfanityService fejler → Fallback aktiveres → Kommentarer fortsætter"
- "Forbedring: Monolitisk - 1 fejl = 0% availability, Microservices - 1 fejl = 80-90% availability (andre services fortsætter)"

---

## SLIDE 8: Relation til Skaleringskuben - Z-akse
**Speaking Notes:**
- "Relation til Skaleringskuben: Z-akse (Data Partitioning) → Availability"
- "Princippet: Flere databases = Redundancy - hvis én database fejler, kan andre fortsætte"
- "Availability Impact: 1 database - hvis database fejler → 0% availability, 8 databases (Z-akse) - hvis 1 database fejler → 87.5% availability (7/8 fortsætter)"
- "Eksempel fra kodebasen: docker-compose.yml - 8 databases (global + 7 kontinenter) - article-db-global, article-db-europe, article-db-asia, osv."
- "Forbedring: 1 database - 1 fejl = 0% availability, 8 databases - 1 fejl = 87.5% availability (andre regions fortsætter)"

---

## SLIDE 9: Kombineret Skalering → Availability
**Speaking Notes:**
- "Kombineret Skalering → Availability:"
- "X + Y + Z akser kombineret:"
- "X-akse: 3 replicas per service - redundancy"
- "Y-akse: 6 services (isolation) - fejl i én service påvirker ikke andre"
- "Z-akse: 8 databases (geografisk) - fejl i én database påvirker ikke andre"
- "Availability beregning: Service availability = 99% (per service), With 3 replicas (X-akse): 99.999% (redundancy), With 6 services (Y-akse): Isolation → 1 service fejl = 83% availability, With 8 databases (Z-akse): 1 DB fejl = 87.5% availability"
- "Total system availability ≈ 99.9%+"

---

## PART B: MEKANISMER TIL FORBEDRING

---

## SLIDE 10: Problemstilling - Stigende Trafik
**Speaking Notes:**
- "Problemstilling: System presses af stigende trafik"
- "100 req/s → 1000 req/s (10x stigning)"
- "Systemet går ned ved høj trafik"
- "Availability falder fra 99% → 50%"
- "Vi skal implementere mekanismer til forbedring af availability"

---

## SLIDE 11: Mekanisme 1 - Circuit Breaker
**Speaking Notes:**
- "Mekanisme 1: Circuit Breaker (Isolerer Fejlende Services)"
- "Kodeeksempel: CommentService/Program.cs (linje 28-30) - CircuitBreakerAsync med 3 handledEventsAllowedBeforeBreaking og 20 sekunder durationOfBreak"
- "Availability forbedring: Før - ProfanityService fejler → CommentService blokeret → 0% availability, Efter - Circuit breaker åbner → CommentService bruger fallback → 80% availability"
- "Ved stigende trafik: 100 req/s - System håndterer det → 99% availability, 1000 req/s - ProfanityService overbelastet → Circuit breaker åbner → 80% availability (med fallback)"
- "Dette viser hvordan circuit breaker isolerer fejlende services og forbedrer availability"

---

## SLIDE 12: Mekanisme 2 - Retry Pattern
**Speaking Notes:**
- "Mekanisme 2: Retry Pattern (Håndterer Transient Fejl)"
- "Kodeeksempel: CommentService/Program.cs (linje 23-26) - WaitAndRetryAsync med 200ms, 500ms, 1 sekund delays"
- "Availability forbedring: Før - 1 transient fejl → Request fejler → 0% availability for den request, Efter - 1 transient fejl → 3 retries → 95% success rate → Højere availability"
- "Ved stigende trafik: 100 req/s - Få transient fejl → Retry håndterer det → 99% availability, 1000 req/s - Flere transient fejl → Retry håndterer de fleste → 95% availability"
- "Dette viser hvordan retry håndterer transient fejl og forbedrer availability"

---

## SLIDE 13: Mekanisme 3 - Caching
**Speaking Notes:**
- "Mekanisme 3: Caching (Reducerer Load)"
- "Kodeeksempel: ArticleService/Controllers/ArticlesController.cs (linje 32-39) - Redis cache lookup, cache hit returnerer 5ms response"
- "Availability forbedring: Før - Hver request → Database query → Database overbelastet → 0% availability, Efter - 80% cache hits → 80% færre database queries → Database ikke overbelastet → 99% availability"
- "Ved stigende trafik: 100 req/s - 20 DB queries/s → Database håndterer det → 99% availability, 1000 req/s - 200 DB queries/s (80% cache hits) → Database håndterer det → 99% availability, Uden cache - 1000 DB queries/s → Database overbelastet → 0% availability"
- "Dette viser hvordan caching reducerer load og forbedrer availability"

---

## SLIDE 14: Mekanisme 4 - Timeout
**Speaking Notes:**
- "Mekanisme 4: Timeout (Forhindrer Blokering)"
- "Kodeeksempel: CommentService/Program.cs (linje 35) - Timeout = TimeSpan.FromSeconds(2)"
- "Availability forbedring: Før - Langsom service blokerer alle requests → Thread pool udtømt → 0% availability, Efter - Timeout efter 2 sek → Thread frigives → System fortsætter → 99% availability"
- "Ved stigende trafik: 100 req/s - Få langsomme requests → Timeout håndterer det → 99% availability, 1000 req/s - Mange langsomme requests → Timeout forhindrer blokering → 95% availability"
- "Dette viser hvordan timeout forhindrer blokering og forbedrer availability"

---

## SLIDE 15: Mekanisme 5 - Fallback
**Speaking Notes:**
- "Mekanisme 5: Fallback (Graceful Degradation)"
- "Kodeeksempel: CommentService/Services/ProfanityClient.cs (linje 14-16) - FALLBACK_PROFANITY_WORDS environment variable, fallback word list"
- "Availability forbedring: Før - ProfanityService nede → Kommentarer ikke mulige → 0% availability for feature, Efter - Fallback aktiveres → Kommentarer mulige (reduceret funktionalitet) → 80% availability"
- "Ved stigende trafik: 100 req/s - ProfanityService håndterer det → 100% availability, 1000 req/s - ProfanityService overbelastet → Fallback aktiveres → 80% availability"
- "Dette viser hvordan fallback giver graceful degradation og forbedrer availability"

---

## SLIDE 16: Mekanisme 6 - X-akse Skalering
**Speaking Notes:**
- "Mekanisme 6: X-akse Skalering (Load Distribution)"
- "Kodeeksempel: docker-compose.yml - Kan skaleres til flere replicas, deploy.replicas: 3 for load distribution"
- "Availability forbedring: Før - 1 instance → Hvis instance fejler → 0% availability, Efter - 3 instances → Hvis 1 fejler → 66% availability (2/3 fortsætter)"
- "Ved stigende trafik: 100 req/s - 1 instance håndterer det → 99% availability, 1000 req/s - 3 instances → Load fordelt → 99% availability, Uden skalering - 1 instance overbelastet → 0% availability"
- "Dette viser hvordan X-akse skalering distribuerer load og forbedrer availability"

---

## SLIDE 17: Mekanisme 7 - Asynkron Processing
**Speaking Notes:**
- "Mekanisme 7: Asynkron Processing (Decoupling)"
- "Kodeeksempel: PublisherService/Controllers/PublishController.cs (linje 34-37) - RabbitMQ queue, returnerer 202 Accepted umiddelbart"
- "Availability forbedring: Før - Synkron processing → Hvis ArticleService langsom → PublisherService blokeret → 0% availability, Efter - Asynkron queue → PublisherService returnerer umiddelbart → 99% availability"
- "Ved stigende trafik: 100 req/s - Synkron → ArticleService kan håndtere det → 99% availability, 1000 req/s - Synkron → ArticleService overbelastet → PublisherService blokeret → 0% availability, Asynkron - ArticleService processerer i baggrunden → PublisherService fortsætter → 99% availability"
- "Dette viser hvordan asynkron processing decoupler services og forbedrer availability"

---

## SLIDE 18: Kombineret Approach - Optimal Availability
**Speaking Notes:**
- "Kombineret Approach (Optimal Availability):"
- "Alle mekanismer kombineret: CommentService - Retry transient fejl, Circuit breaker for permanente fejl, Timeout (2 sek), Fallback (local cache), Caching (Redis), X-akse skalering (3 replicas)"
- "Availability ved stigende trafik:"
- "100 req/s: Uden Mekanismer 99%, Med Alle Mekanismer 99.9%, Forbedring +0.9%"
- "500 req/s: Uden Mekanismer 50%, Med Alle Mekanismer 99%, Forbedring +49%"
- "1000 req/s: Uden Mekanismer 0% (system ned), Med Alle Mekanismer 95%, Forbedring +95%"
- "2000 req/s: Uden Mekanismer 0%, Med Alle Mekanismer 90%, Forbedring +90%"
- "Dette viser at kombinationen af alle mekanismer giver optimal availability"

---

## PART C: NEGATIVE INDRIVNINGER

---

## SLIDE 19: Negativ Indvirkning 1 - Synkron Service Dependencies
**Speaking Notes:**
- "Negativ Indvirkning 1: Synkron Service Dependencies"
- "Problem: Tæt kobling mellem services via synkron HTTP calls"
- "Eksempel fra kodebasen: PublisherService/Controllers/PublishController.cs (linje 24-27) - Synkron HTTP call til ProfanityService"
- "Negativ Impact: Hvis ProfanityService er langsom → PublisherService blokeret → 0% availability, Hvis ProfanityService fejler → PublisherService fejler → 0% availability, Cascading failures - fejl spreder sig til alle afhængige services"
- "Løsning: Asynkron processing (RabbitMQ) → Decoupling, Circuit breaker → Isolation, Timeout → Forhindrer blokering"
- "Dette viser hvordan synkron dependencies reducerer availability"

---

## SLIDE 20: Negativ Indvirkning 2 - Single Point of Failure
**Speaking Notes:**
- "Negativ Indvirkning 2: Single Point of Failure (SPOF)"
- "Problem: Kritisk komponent uden redundancy"
- "Eksempler fra kodebasen: docker-compose.yml - Single Redis instance, Single RabbitMQ instance, Ingen replication → SPOF"
- "Negativ Impact: Hvis Redis fejler → Alle cache operations fejler → System overbelastet → 0% availability, Hvis RabbitMQ fejler → Ingen message processing → 0% availability for publishing"
- "Løsning: Redis Cluster (replication), RabbitMQ Cluster (high availability), Multiple instances (X-akse skalering)"
- "Dette viser hvordan SPOF reducerer availability"

---

## SLIDE 21: Negativ Indvirkning 3 - Tæt Database Coupling
**Speaking Notes:**
- "Negativ Indvirkning 3: Tæt Database Coupling"
- "Problem: Alle services afhænger af samme database"
- "Eksempel: Før Z-akse skalering - alle services bruger samme database"
- "Negativ Impact: Hvis database fejler → Alle services påvirkes → 0% availability, Hvis database er langsom → Alle services langsomme → 0% availability, Database bottleneck - kan ikke skaleres uafhængigt"
- "Løsning: Z-akse skalering (flere databases), Database replication, Read replicas"
- "Dette viser hvordan tæt database coupling reducerer availability"

---

## SLIDE 22: Negativ Indvirkning 4 - Manglende Health Checks
**Speaking Notes:**
- "Negativ Indvirkning 4: Manglende Health Checks"
- "Problem: Ingen automatisk detection af fejlende services"
- "Negativ Impact: Fejlende service opdages sent → Længere downtime → Lavere availability, Ingen automatisk recovery → Manual intervention nødvendig → Længere downtime"
- "Løsning: Health check endpoints - path: /health, interval: 30s, timeout: 10s, retries: 3, Automatisk recovery, Monitoring"
- "Dette viser hvordan manglende health checks reducerer availability"

---

## SLIDE 23: Negativ Indvirkning 5 - Ingen Circuit Breaker
**Speaking Notes:**
- "Negativ Indvirkning 5: Ingen Circuit Breaker"
- "Problem: Fejlende services blokerer alle requests"
- "Eksempel (Uden Circuit Breaker): FØR - Ingen circuit breaker, hvis ProfanityService fejler → Blokerer alle requests"
- "Negativ Impact: Fejlende service → Blokerer alle requests → 0% availability, Resource exhaustion → Thread pool udtømt → 0% availability, Cascading failures → Fejl spreder sig"
- "Løsning: Circuit breaker (som i CommentService/Program.cs) - Isolerer fejlende service"
- "Dette viser hvordan manglende circuit breaker reducerer availability"

---

## SLIDE 24: Negativ Indvirkning 6 - Manglende Timeout
**Speaking Notes:**
- "Negativ Indvirkning 6: Manglende Timeout"
- "Problem: Ingen timeout på eksterne calls"
- "Negativ Impact: Langsom service → Blokerer alle requests → 0% availability, Thread pool udtømt → Alle threads venter → 0% availability"
- "Løsning: Timeout (som i CommentService/Program.cs - 2 sek) → Timeout efter 2 sek"
- "Dette viser hvordan manglende timeout reducerer availability"

---

## SLIDE 25: Sammenligning - Negativ vs. Positiv Impact
**Speaking Notes:**
- "Sammenligning: Negativ vs. Positiv Impact"
- "Synkron dependencies: ❌ 0% (hvis dependency fejler) - PublisherService → ProfanityService (synkron)"
- "Asynkron processing: ✅ 99% (decoupling) - PublisherService → RabbitMQ → ArticleService"
- "Single Point of Failure: ❌ 0% (hvis SPOF fejler) - Single Redis instance"
- "Redundancy (X-akse): ✅ 99.9% (redundancy) - 3 replicas per service"
- "Ingen circuit breaker: ❌ 0% (cascading failures) - Før circuit breaker implementation"
- "Circuit breaker: ✅ 80-90% (isolation) - CommentService → ProfanityService"
- "Dette viser tydeligt forskellen mellem negative og positive arkitektoniske valg"

---

## SLIDE 26: Best Practices for Høj Availability
**Speaking Notes:**
- "Best Practices for Høj Availability:"
- "1. Design for Failure: Antag at services kan fejle, Implementer circuit breakers, timeouts, fallbacks"
- "2. Redundancy: X-akse skalering (flere instances), Database replication, Multiple infrastructure components"
- "3. Isolation: Y-akse skalering (microservices), Circuit breakers, Bulkhead pattern"
- "4. Decoupling: Asynkron processing (queues), Event-driven architecture, Løs kobling mellem services"
- "5. Monitoring: Health checks, Metrics, logging, tracing, Proactive alerting"
- "Dette er de vigtigste principper for høj availability"

---

## SLIDE 27: Konklusion
**Speaking Notes:**
- "Konklusion:"
- "Arkitektoniske valg der reducerer availability: ❌ Synkron dependencies, ❌ Single Point of Failure, ❌ Tæt database coupling, ❌ Manglende health checks, ❌ Ingen circuit breaker, ❌ Manglende timeout, ❌ Synchronous operations, ❌ Ingen fallback, ❌ Manglende monitoring, ❌ Tight coupling"
- "Arkitektoniske valg der forbedrer availability: ✅ Asynkron processing, ✅ Redundancy (X-akse), ✅ Isolation (Y-akse, circuit breakers), ✅ Health checks, ✅ Circuit breakers, ✅ Timeouts, ✅ Caching, ✅ Fallback mechanisms, ✅ Monitoring, ✅ Løs kobling"
- "Anbefaling: Design systemet med availability i tankerne fra starten - Implementer circuit breakers, timeouts, fallbacks, Brug asynkron processing hvor muligt, Skaler på X, Y, Z akser for redundancy og isolation, Monitor kontinuerligt for proactive detection"

---

## SLIDE 28: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres opmærksomhed"
- "Jeg er klar til spørgsmål"

---

## EKSTRA NOTER TIL EKSAMEN:

### Hvis de spørger om specifikke filer:
- **`CommentService/Program.cs`**: Circuit breaker, retry, timeout implementation
- **`ArticleService/Controllers/ArticlesController.cs`**: Caching implementation
- **`PublisherService/Controllers/PublishController.cs`**: Synkron dependency eksempel
- **`docker-compose.yml`**: X-akse, Y-akse, Z-akse skalering eksempler
- **`CommentService/Services/ProfanityClient.cs`**: Fallback implementation

### Hvis de spørger om availability beregninger:
- Availability = (Total Time - Downtime) / Total Time × 100%
- 99% = 3.65 dages downtime/år
- 99.9% = 8.76 timer downtime/år
- 99.99% = 52.56 minutter downtime/år
- 99.999% = 5.26 minutter downtime/år

### Hvis de spørger om skaleringskuben:
- X-akse: Horizontal Duplication → Redundancy → Højere availability
- Y-akse: Functional Decomposition → Isolation → Højere availability
- Z-akse: Data Partitioning → Redundancy → Højere availability
- Kombineret: X + Y + Z → 99.9%+ availability

### Hvis de spørger om mekanismer:
- Circuit Breaker: Isolerer fejlende services → 80% availability
- Retry: Håndterer transient fejl → 95% availability
- Caching: Reducerer load → 99% availability
- Timeout: Forhindrer blokering → 99% availability
- Fallback: Graceful degradation → 80% availability
- X-akse skalering: Load distribution → 99.9% availability
- Asynkron processing: Decoupling → 99% availability

### Hvis de spørger om negative indvirkninger:
- Synkron dependencies: 0% availability (hvis dependency fejler)
- Single Point of Failure: 0% availability (hvis SPOF fejler)
- Tæt database coupling: 0% availability (hvis database fejler)
- Manglende health checks: Længere downtime → Lavere availability
- Ingen circuit breaker: 0% availability (cascading failures)
- Manglende timeout: 0% availability (thread pool udtømt)
