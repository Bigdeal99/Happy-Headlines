# EKSAMEN - Spørgsmål 2: Fault Isolation
## Slide Outline & Speaking Notes

---

## SLIDE 1: Titel
**Speaking Notes:**
- "Jeg vil i dag præsentere fault isolation"
- "Jeg vil dække tre dele: principper, demonstration af circuit breakers, og sammenligning af resilience patterns"
- "Fokus er på hvordan vi isolerer fejl så de ikke spreder sig til hele systemet"

---

## PART A: BESKRIV PRINCIPPER

---

## SLIDE 2: Princippet - Isolation Boundaries
**Speaking Notes:**
- "Første princip: Isolation Boundaries"
- "Fault isolation handler om at begrænse fejlens spredning til en specifik komponent eller service"
- "Princippet er at fejl i én del af systemet ikke skal påvirke andre dele"
- "Eksempel: Hvis ProfanityService fejler, skal det ikke påvirke CommentService eller ArticleService"
- "Dette forhindrer cascading failures"

---

## SLIDE 3: Princippet - Fail-Fast
**Speaking Notes:**
- "Andet princip: Fail-Fast"
- "Systemet skal hurtigt identificere og håndtere fejl"
- "I stedet for at lade dem sprede sig"
- "Dette reducerer kaskadefejl"
- "Eksempel: Hvis en service er langsom, timeout hurtigt i stedet for at vente"

---

## SLIDE 4: Princippet - Circuit Breaker
**Speaking Notes:**
- "Tredje princip: Circuit Breaker Pattern"
- "Automatisk deaktiverer en fejlende service efter et tærskelværdi af fejl"
- "Forhindrer yderligere requests og giver systemet tid til at genoprette sig"
- "Circuit breaker har tre states: Closed (normal), Open (fejlende), Half-Open (test)"
- "Dette er hovedprincippet vi demonstrerer i del B"

---

## SLIDE 5: Princippet - Graceful Degradation
**Speaking Notes:**
- "Fjerde princip: Graceful Degradation"
- "Systemet skal kunne fortsætte med reduceret funktionalitet når en komponent fejler"
- "I stedet for at gå helt ned"
- "Eksempel: Hvis ProfanityService fejler, bruger vi fallback word list"
- "Systemet fortsætter, men med reduceret funktionalitet"

---

## SLIDE 6: Princippet - Timeout og Resource Limits
**Speaking Notes:**
- "Femte princip: Timeout og Resource Limits"
- "Sætter tidsbegrænsninger og ressourcegrænser"
- "Forhindrer at en fejlende service blokerer andre operationer"
- "Eksempel: 2 sek timeout på HTTP calls"
- "Hvis service ikke svarer inden 2 sek, fail fast"

---

## SLIDE 7: Princippet - Health Checks
**Speaking Notes:**
- "Sjette princip: Health Checks og Monitoring"
- "Kontinuerlig overvågning af service health"
- "Identificer fejl tidligt og isoler dem"
- "Eksempel: Health check endpoints der returnerer service status"
- "Monitoring kan trigger alerts når service fejler"

---

## PART B: DEMONSTRER CIRCUIT BREAKERS

---

## SLIDE 8: Demo - Circuit Breaker Konfiguration
**Speaking Notes:**
- "Nu viser jeg konkrete eksempler fra vores projekt"
- "Første eksempel: Circuit Breaker Konfiguration"
- "I CommentService/Program.cs ser vi hvordan vi konfigurerer circuit breaker"
- "Retry policy prøver igen ved transient fejl - 3 forsøg med exponential backoff"
- "Circuit breaker åbner efter 3 fejl og holder åben i 20 sekunder"
- "Timeout er sat til 2 sekunder for at undgå lange ventetider"

**Screenshot Instructions:**
1. Åbn `CommentService/Program.cs`
2. Marker linje 23-30 (retry og breaker konfiguration)
3. Marker linje 32-38 (HttpClient registration med policies)
4. Vis hvordan retry og breaker kombineres

---

## SLIDE 9: Demo - Circuit Breaker States
**Speaking Notes:**
- "Circuit breaker states:"
- "Closed: Normal tilstand - requests går igennem"
- "Efter 3 fejl: Circuit breaker åbner"
- "Open: Alle requests bliver afvist umiddelbart"
- "Efter 20 sekunder: Circuit breaker går til Half-Open"
- "Half-Open: Prøver én request for at teste om service er genoprettet"
- "Hvis request lykkes: Luk circuit breaker (Closed)"
- "Hvis request fejler: Åbn igen (Open)"

---

## SLIDE 10: Demo - Brug i Controller
**Speaking Notes:**
- "Andet eksempel: Brug af Circuit Breaker i Controller"
- "I CommentsController ser vi at profanity check er beskyttet af circuit breaker"
- "Hvis ProfanityService fejler, kaster circuit breaker exception"
- "CommentService bliver ikke blokeret"
- "Systemet kan fortsætte med andre operationer"

**Screenshot Instructions:**
1. Åbn `CommentService/Controllers/CommentsController.cs`
2. Marker linje 30-37 (Post metoden)
3. Vis `await _profanity.ContainsProfanity()` kald
4. Forklar at dette er beskyttet af circuit breaker fra Program.cs

---

## SLIDE 11: Demo - Fallback Mechanism
**Speaking Notes:**
- "Tredje eksempel: Fallback Mechanism"
- "ProfanityClient har en fallback word list"
- "Hvis circuit breaker er åben, kan systemet bruge fallback"
- "Fallback er konfigureret via environment variable"
- "Dette giver graceful degradation - systemet fortsætter med reduceret funktionalitet"

**Screenshot Instructions:**
1. Åbn `CommentService/Services/ProfanityClient.cs`
2. Marker linje 5-17 (class definition og constructor)
3. Vis `_fallbackList` og hvordan det initialiseres
4. Forklar at dette er klar til brug når circuit breaker er åben

---

## SLIDE 12: Demo - Performance Impact
**Speaking Notes:**
- "Performance Impact:"
- "Uden circuit breaker: Hvis ProfanityService er nede, venter hver request 2 sek på timeout"
- "100 requests = 200 sekunder total ventetid"
- "CommentService bliver blokeret, database connections holdes åbne, thread pool udtømmes"
- "Med circuit breaker: Efter 3 fejl (6 sek), åbner circuit breaker"
- "Resten af requests bliver afvist umiddelbart (0ms)"
- "CommentService fortsætter, ressourcer frigives hurtigt"
- "Response time: 2000ms → 0ms (efter circuit åbner)"

---

## SLIDE 13: Demo - Isolation i Praksis
**Speaking Notes:**
- "Isolation i praksis:"
- "Før circuit breaker: Hvis ProfanityService er nede, bliver CommentService blokeret"
- "Alle kommentar-requests påvirkes, database connections holdes åbne"
- "Systemet kan gå ned (cascading failure)"
- "Efter circuit breaker: Circuit breaker åbner efter 3 fejl"
- "CommentService bruger fallback, kommentar-funktionalitet fortsætter (reduceret)"
- "Andre services påvirkes ikke, systemet forbliver tilgængeligt"

---

## PART C: SAMMENLIGN RESILIENCE PATTERNS

---

## SLIDE 14: Pattern 1 - Retry
**Speaking Notes:**
- "Nu sammenligner jeg forskellige resilience patterns"
- "Første pattern: Retry"
- "Prøver operation igen ved fejl"
- "Forbedrer availability ved at håndtere transient fejl"
- "Eksempel: Network glitch - retry kan løse problemet"
- "Trade-off: Hvis fejl er permanent, forbruger retry ressourcer og forlænger fejl-tiden"
- "I vores projekt: Retry policy prøver 3 gange med exponential backoff"

---

## SLIDE 15: Pattern 2 - Circuit Breaker
**Speaking Notes:**
- "Andet pattern: Circuit Breaker"
- "Stopper requests til fejlende service"
- "Forbedrer availability ved at forhindre resource exhaustion"
- "Use case: Fejlende eller langsom service"
- "Trade-off: Reducerer funktionalitet når circuit breaker er åben"
- "Men forbedrer overall system availability"
- "I vores projekt: Circuit breaker åbner efter 3 fejl"

---

## SLIDE 16: Pattern 3 - Fallback
**Speaking Notes:**
- "Tredje pattern: Fallback"
- "Alternativ handling når primær service fejler"
- "Forbedrer availability ved graceful degradation"
- "Use case: Når reduceret funktionalitet er acceptabelt"
- "Trade-off: Kan returnere mindre præcise resultater"
- "I vores projekt: Fallback word list når ProfanityService fejler"

---

## SLIDE 17: Pattern 4 - Timeout
**Speaking Notes:**
- "Fjerde pattern: Timeout"
- "Begrænser ventetid på operation"
- "Forbedrer availability ved at forhindre lange blokeringer"
- "Use case: Alle eksterne calls"
- "Trade-off: Kan afvise legitime langsomme requests"
- "I vores projekt: 2 sek timeout på HTTP calls til ProfanityService"

---

## SLIDE 18: Pattern 5 - Bulkhead
**Speaking Notes:**
- "Femte pattern: Bulkhead"
- "Isolerer ressourcer så fejl i én del ikke kan forbruge alle ressourcer"
- "Forbedrer availability ved at forhindre cascading failures"
- "Use case: Kritiske vs. ikke-kritiske operationer"
- "Trade-off: Kan reducere resource utilization"
- "Eksempel: Separate thread pools for kritiske og ikke-kritiske operationer"

---

## SLIDE 19: Availability Sammenligning
**Speaking Notes:**
- "Availability sammenligning:"
- "Scenario: ProfanityService går ned i 5 minutter"
- "Ingen pattern: 0% availability - system går ned"
- "Kun Retry: 0% availability - retry forbruger ressourcer"
- "Kun Circuit Breaker: 80% availability - system fortsætter"
- "Retry + Circuit Breaker + Fallback: 90% availability - optimal"
- "Kombination af patterns giver højeste availability"

---

## SLIDE 20: Kombinerede Patterns
**Speaking Notes:**
- "Kombinerede patterns:"
- "I vores projekt kombinerer vi flere patterns"
- "Retry håndterer transient fejl - 99%+ availability"
- "Circuit breaker isolerer permanente fejl - 80-90% availability"
- "Timeout forhindrer blokering - højere availability"
- "Fallback giver graceful degradation - 70-80% availability"
- "Kombineret: 90%+ availability selv når dependencies fejler"

---

## SLIDE 21: Availability Ranking
**Speaking Notes:**
- "Availability ranking:"
- "Højeste availability: Kombination af alle patterns - 90%+"
- "Circuit Breaker + Fallback: 85%"
- "Circuit Breaker alene: 80%"
- "Fallback alene: 70%"
- "Retry alene: 0% ved permanent fejl"
- "Ingen pattern: 0%"
- "Anbefaling: Kombiner altid timeout, retry, circuit breaker, og fallback"

---

## SLIDE 22: Konklusion
**Speaking Notes:**
- "Konklusion:"
- "Fault isolation er essentielt for microservices arkitektur"
- "Circuit breaker isolerer fejl effektivt - forhindrer cascading failures"
- "Kombination af patterns giver optimal availability - 90%+"
- "Anbefaling: Brug altid timeout, retry, circuit breaker, og fallback"
- "Dette giver robuste microservices der kan håndtere fejl gracefully"

---

## SLIDE 23: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres opmærksomhed"
- "Jeg er klar til spørgsmål"

---

## EKSTRA NOTER TIL EKSAMEN:

### Hvis de spørger om specifikke filer:
- **CommentService/Program.cs**: Circuit breaker konfiguration (linje 20-38)
- **CommentService/Controllers/CommentsController.cs**: Brug af circuit breaker (linje 30-37)
- **CommentService/Services/ProfanityClient.cs**: Fallback mechanism (linje 5-17)

### Hvis de spørger om circuit breaker states:
- **Closed**: Normal tilstand - requests går igennem
- **Open**: Efter 3 fejl - alle requests afvises umiddelbart
- **Half-Open**: Efter 20 sek - prøver én request for at teste
- **Closed**: Hvis test lykkes - tilbage til normal

### Hvis de spørger om performance:
- **Uden circuit breaker**: 100 requests = 200 sek ventetid
- **Med circuit breaker**: 100 requests = 6 sek (første 3) + 0ms (resten)
- **Response time**: 2000ms → 0ms (efter circuit åbner)

### Hvis de spørger om availability:
- **Ingen pattern**: 0% availability
- **Circuit Breaker alene**: 80% availability
- **Kombineret patterns**: 90%+ availability

### Hvis de spørger om hvornår IKKE at bruge:
- Simple applikationer uden dependencies
- Hvor immediate consistency er kritisk
- Hvor team mangler erfaring med resilience patterns
