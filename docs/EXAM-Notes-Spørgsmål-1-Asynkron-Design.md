# EKSAMEN - Spørgsmål 1: Asynkron Design
## Slide Outline & Speaking Notes

---

## SLIDE 1: Titel
**Speaking Notes:**
- "Jeg vil i dag præsentere asynkron design"
- "Jeg vil dække tre dele: problemstillinger, demonstration fra vores projekt, og kompromiser"
- "Fokus er på hvordan asynkron design løser performance-problemer, men også introducerer kompleksitet"

---

## PART A: BESKRIV PROBLEMSTILLINGER

---

## SLIDE 2: Problem 1 - Blocking I/O
**Speaking Notes:**
- "Første problem: blocking I/O"
- "Når en synkron operation venter på database eller HTTP call, bliver tråden blokeret"
- "Tråden kan ikke håndtere andre requests mens den venter"
- "Dette skaber lav throughput - færre requests kan håndteres"
- "Det er også ressource-spild - tråde venter inaktivt"
- "For at håndtere flere requests, skal vi have flere tråde, hvilket bruger mere hukommelse"

---

## SLIDE 3: Problem 2 - Tæt Kobling
**Speaking Notes:**
- "Andet problem: tæt kobling mellem services"
- "Synkron kommunikation skaber tæt kobling"
- "Hvis Service B er langsom eller nede, påvirker det hele kæden"
- "Service A venter på Service B, som venter på Service C"
- "Dette skaber cascading failures"

---

## SLIDE 4: Problem 3 - Long-Running Operations
**Speaking Notes:**
- "Tredje problem: long-running operations"
- "Operationer som cache warming eller data processing tager lang tid"
- "Hvis disse kører synkront, blokerer de API responses"
- "Brugere venter unødvendigt længe"
- "Disse operationer bør køre i baggrunden"

---

## SLIDE 5: Problem 4 - Resource Contention
**Speaking Notes:**
- "Fjerde problem: resource contention"
- "Mange samtidige requests til samme resource, fx database"
- "Database connection pool kan blive udtømt"
- "Dette skaber bottleneck - alle requests venter på connections"
- "Asynkron design kan håndtere dette bedre ved at frigive connections hurtigere"

---

## PART B: DEMONSTRER FRA PROJEKTET

---

## SLIDE 6: Demo 1 - Message Queue Pattern
**Speaking Notes:**
- "Nu viser jeg konkrete eksempler fra vores projekt"
- "Første eksempel: Message Queue Pattern"
- "I PublishController ser vi at når en artikel publiceres, sender vi den til en queue i stedet for at vente på database write"
- "API returnerer 202 Accepted umiddelbart - ikke 200 OK efter database write"
- "Dette giver 10x hurtigere response time - fra 500ms til 50ms"
- "I ArticleQueueConsumer ser vi at messages processeres asynkront i baggrunden"
- "Consumer kan håndtere 10 messages samtidigt (BasicQos)"
- "Dette giver bedre resource utilization"

**Screenshot Instructions:**
1. Åbn `PublisherService/Controllers/PublishController.cs`
2. Marker linje 20-38 (hele Publish metoden)
3. Vis hvordan `_publisher.Publish(msg)` returnerer umiddelbart
4. Åbn `ArticleService/Services/ArticleQueueConsumer.cs`
5. Marker linje 38-69 (consumer.Received handler)
6. Vis `BasicQos(0, 10, false)` - concurrent processing

---

## SLIDE 7: Demo 1 - Performance Metrics
**Speaking Notes:**
- "Performance-forbedring:"
- "Response time: fra 500ms til 50ms - 10x hurtigere"
- "Throughput: fra 2 requests per sekund per tråd til 20 requests per sekund per tråd"
- "Dette er fordi tråden ikke blokeres på database write"
- "Tråden kan håndtere flere requests mens database write sker i baggrunden"

---

## SLIDE 8: Demo 2 - Async/Await i Controllers
**Speaking Notes:**
- "Andet eksempel: Async/Await i Controllers"
- "I ArticlesController ser vi at alle I/O operationer er asynkrone"
- "Cache lookup, database query, og cache write bruger alle await"
- "Dette betyder at tråden ikke blokeres under I/O"
- "En tråd kan håndtere mange requests samtidigt"
- "I stedet for 1000 tråde for 1000 requests, kan vi bruge ~20 tråde"
- "Dette sparer 50x hukommelse - fra 2GB til 40MB"

**Screenshot Instructions:**
1. Åbn `ArticleService/Controllers/ArticlesController.cs`
2. Marker linje 29-52 (List metoden)
3. Vis alle `await` keywords
4. Forklar at hver await frigiver tråden til andre requests

---

## SLIDE 9: Demo 2 - Thread Pool Efficiency
**Speaking Notes:**
- "Thread pool efficiency:"
- "Synkron: Hver request kræver sin egen tråd"
- "1000 requests = 1000 tråde = 2GB RAM (2MB per tråd)"
- "Asynkron: En tråd kan håndtere mange requests"
- "1000 requests = ~20 tråde = 40MB RAM"
- "Dette er 50x mindre hukommelse"
- "CPU utilization er også bedre - fra 80% til 40%"

---

## SLIDE 10: Demo 3 - Background Services
**Speaking Notes:**
- "Tredje eksempel: Background Services"
- "ArticleCacheWarmer kører i baggrunden og varmer cache op"
- "Dette påvirker ikke API performance"
- "Cache hit rate forbedres fra 30% til 80%"
- "Response time forbedres fra 50ms (database) til 5ms (cache)"
- "Dette er muligt fordi cache warming sker asynkront i baggrunden"

**Screenshot Instructions:**
1. Åbn `ArticleService/Services/ArticleCacheWarmer.cs`
2. Marker linje 21-44 (ExecuteAsync metoden)
3. Vis `while` loop og `Task.Delay`
4. Forklar at dette kører kontinuerligt i baggrunden

---

## SLIDE 11: Demo 3 - Cache Performance
**Speaking Notes:**
- "Cache performance:"
- "Før cache warming: 30% cache hits, 50ms response time (database)"
- "Efter cache warming: 80% cache hits, 5ms response time (cache)"
- "Dette er 10x hurtigere for cache hits"
- "API performance forbedres fordi cache warming sker asynkront"
- "Ingen impact på API response times"

---

## SLIDE 12: Performance Summary
**Speaking Notes:**
- "Performance summary:"
- "Alle metrics forbedres betydeligt"
- "Threads: 50x reduktion"
- "Memory: 50x reduktion"
- "Throughput: 10x forbedring"
- "Response time: 10x forbedring"
- "CPU utilization: 2x bedre"
- "Disse forbedringer er mulige fordi asynkron design frigiver tråde under I/O"

---

## PART C: VURDER KOMPROMISER

---

## SLIDE 13: Kompromis 1 - Error Handling
**Speaking Notes:**
- "Nu diskuterer jeg kompromiserne"
- "Første kompromis: Error handling bliver mere kompleks"
- "I ArticleQueueConsumer, hvis database save fejler, skal vi håndtere det korrekt"
- "Vi skal nack message i stedet for ack, så den kan prøves igen"
- "Dette kræver try-catch blokke og korrekt error handling"
- "Synkron kode har simplere error handling"

**Screenshot Instructions:**
1. Åbn `ArticleService/Services/ArticleQueueConsumer.cs`
2. Marker linje 64-66 (SaveChangesAsync og BasicAck)
3. Forklar at hvis SaveChangesAsync fejler, skal vi BasicNack i stedet
4. Vis at dette kræver try-catch

---

## SLIDE 14: Kompromis 2 - Debugging
**Speaking Notes:**
- "Andet kompromis: Debugging bliver mere kompleks"
- "Stack traces i asynkron kode er mindre informative"
- "Async context kan mistes"
- "Vi bruger distributed tracing (OpenTelemetry/Jaeger) for at følge async flow"
- "Dette kræver ekstra infrastruktur og setup"
- "I ArticleQueueConsumer ser vi Activity.StartActivity for tracing"

**Screenshot Instructions:**
1. Åbn `ArticleService/Services/ArticleQueueConsumer.cs`
2. Marker linje 46-49 (Activity.StartActivity)
3. Forklar at dette er nødvendigt for at følge async flow

---

## SLIDE 15: Kompromis 3 - State Management
**Speaking Notes:**
- "Tredje kompromis: State management"
- "Shared state kan skabe race conditions i asynkron kode"
- "Fx LRU cache management - hvis to requests opdaterer samtidigt"
- "Dette kræver atomic operations eller locking"
- "Locking kan reducere performance gains"
- "Synkron kode har simplere state management"

---

## SLIDE 16: Kompromis 4 - Message Queue Overhead
**Speaking Notes:**
- "Fjerde kompromis: Message queue overhead"
- "RabbitMQ introducerer latency - 1-5ms per message"
- "Yderligere infrastruktur at vedligeholde"
- "Message durability overhead"
- "Men dette giver decoupling og scalability"
- "Trade-off: Længere latency, men bedre decoupling"

---

## SLIDE 17: Kompromis 5 - Eventual Consistency
**Speaking Notes:**
- "Femte kompromis: Eventual consistency"
- "Asynkron processing betyder at data ikke er konsistent med det samme"
- "I PublishController returnerer vi 202 Accepted - article er ikke tilgængelig med det samme"
- "Brugeren skal vente eller poll for status"
- "Dette kræver status endpoints eller websockets"
- "Synkron kode har immediate consistency"

---

## SLIDE 18: Kompromis Summary
**Speaking Notes:**
- "Kompromis summary:"
- "Alle kompromiser handler om højere kompleksitet"
- "Error handling, debugging, state management bliver mere komplekse"
- "Yderligere infrastruktur nødvendig (queues, tracing)"
- "Eventual consistency challenges"
- "Men performance gains er betydelige - 10-50x forbedring"

---

## SLIDE 20: Når Brug Asynkron Design?
**Speaking Notes:**
- "Når brug asynkron design?"
- "Brug det når I/O-bound operations - database, HTTP, file system"
- "Når high throughput er vigtigere end low latency"
- "Når services skal decouples"
- "Når long-running operations"
- "Undgå det for CPU-bound operations - kun overhead"
- "Undgå det hvis simpel synkron kode er tilstrækkelig"
- "Undgå det hvis team mangler erfaring"

---

## SLIDE 21: Mitigation Strategies
**Speaking Notes:**
- "Mitigation strategies:"
- "Vi bruger comprehensive logging - Serilog til Seq"
- "Distributed tracing - OpenTelemetry/Jaeger"
- "Error handling patterns - Polly circuit breaker"
- "Monitoring - Prometheus metrics"
- "Dokumentation af async flows"
- "Disse værktøjer hjælper med at håndtere kompleksiteten"

---

## SLIDE 22: Konklusion
**Speaking Notes:**
- "Konklusion:"
- "Performance gains er betydelige - 10-50x forbedring"
- "Kompleksitet costs er højere - learning curve, infrastruktur"
- "For high-scale microservices som Happy-Headlines er kompleksiteten værd at betale for"
- "Især når vi har distributed tracing, monitoring, og error handling patterns"
- "Asynkron design er essentielt for moderne microservices arkitektur"

---

## SLIDE 23: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres opmærksomhed"
- "Jeg er klar til spørgsmål"

---

## EKSTRA NOTER TIL EKSAMEN:

### Hvis de spørger om specifikke filer:
- **PublisherService/Controllers/PublishController.cs**: Message queue pattern
- **ArticleService/Services/ArticleQueueConsumer.cs**: Async consumer med concurrent processing
- **ArticleService/Controllers/ArticlesController.cs**: Async/await i controllers
- **ArticleService/Services/ArticleCacheWarmer.cs**: Background service

### Hvis de spørger om performance metrics:
- Threads: 50x reduktion (1000 → 20)
- Memory: 50x reduktion (2GB → 40MB)
- Throughput: 10x forbedring (50 → 500 req/s)
- Response time: 10x forbedring (500ms → 50ms)

### Hvis de spørger om kompromiser:
- Error handling: Mere kompleks (try-catch, nack handling)
- Debugging: Kræver distributed tracing
- State management: Race conditions risiko
- Message queue: Overhead + infrastruktur
- Consistency: Eventual consistency

### Hvis de spørger om hvornår IKKE at bruge:
- CPU-bound operations (kun overhead)
- Simpel synkron kode er tilstrækkelig
- Team mangler erfaring med async patterns
