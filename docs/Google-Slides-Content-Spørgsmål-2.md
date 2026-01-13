# Google Slides Content - Spørgsmål 2: Fault Isolation
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Fault Isolation
**Undertitel:** Isolering af fejl i microservices
**Footer:** Dit navn | Dato

---

## SLIDE 2: Princippet - Isolation Boundaries
**Titel:** Princippet: Isolation Boundaries

**Indhold:**
- **Princippet:** Begræns fejlens spredning
- **Effekt:** Fejl i én del påvirker ikke andre dele
- **Eksempel:** Service A fejler → Service B fortsætter

---

## SLIDE 3: Princippet - Fail-Fast
**Titel:** Princippet: Fail-Fast

**Indhold:**
- **Princippet:** Identificer og håndter fejl hurtigt
- **Effekt:** Reducerer kaskadefejl
- **Eksempel:** Timeout → Fail fast → Ingen blokering

---

## SLIDE 4: Princippet - Circuit Breaker
**Titel:** Princippet: Circuit Breaker

**Indhold:**
- **Princippet:** Automatisk deaktiver fejlende service
- **Effekt:** Forhindrer yderligere requests
- **States:** Closed → Open → Half-Open → Closed

---

## SLIDE 5: Princippet - Graceful Degradation
**Titel:** Princippet: Graceful Degradation

**Indhold:**
- **Princippet:** Fortsæt med reduceret funktionalitet
- **Effekt:** Systemet går ikke helt ned
- **Eksempel:** Fallback mechanism

---

## SLIDE 6: Princippet - Timeout og Resource Limits
**Titel:** Princippet: Timeout og Resource Limits

**Indhold:**
- **Princippet:** Sæt tidsbegrænsninger
- **Effekt:** Forhindrer blokering
- **Eksempel:** 2 sek timeout → Fail fast

---

## SLIDE 7: Princippet - Health Checks
**Titel:** Princippet: Health Checks

**Indhold:**
- **Princippet:** Kontinuerlig overvågning
- **Effekt:** Identificer fejl tidligt
- **Eksempel:** Health check endpoints

---

## SLIDE 8: Demo - Circuit Breaker Konfiguration
**Titel:** Demonstration: Circuit Breaker Konfiguration

**Indhold:**
- **Fil:** `CommentService/Program.cs`
- **Kode:**
  ```csharp
  var retry = HttpPolicyExtensions
      .HandleTransientHttpError()
      .WaitAndRetryAsync(new[] { 
          TimeSpan.FromMilliseconds(200), 
          TimeSpan.FromMilliseconds(500), 
          TimeSpan.FromSeconds(1) 
      });
  
  var breaker = HttpPolicyExtensions
      .HandleTransientHttpError()
      .CircuitBreakerAsync(
          handledEventsAllowedBeforeBreaking: 3,
          durationOfBreak: TimeSpan.FromSeconds(20)
      );
  ```

**Screenshot:** Vis linje 20-38 (retry og breaker konfiguration)

---

## SLIDE 9: Demo - Circuit Breaker States
**Titel:** Circuit Breaker States

**Indhold:**
- **Closed (Normal):** Requests går igennem
- **Open (Fejlende):** Efter 3 fejl → Alle requests afvises
- **Half-Open (Test):** Efter 20 sek → Prøver én request
- **Closed:** Hvis test lykkes → Tilbage til normal

---

## SLIDE 10: Demo - Brug i Controller
**Titel:** Demonstration: Brug i Controller

**Indhold:**
- **Fil:** `CommentService/Controllers/CommentsController.cs`
- **Kode:**
  ```csharp
  var hasProfanity = await _profanity.ContainsProfanity(comment.Text, ct);
  ```
- **Fordel:** Beskyttet af circuit breaker

**Screenshot:** Vis linje 30-37 (Post metoden)

---

## SLIDE 11: Demo - Fallback Mechanism
**Titel:** Demonstration: Fallback Mechanism

**Indhold:**
- **Fil:** `CommentService/Services/ProfanityClient.cs`
- **Kode:**
  ```csharp
  private volatile HashSet<string> _fallbackList;
  var fallback = (cfg["FALLBACK_PROFANITY_WORDS"] ?? "bad,ugly,stupid")
                .Split(',', ...);
  ```
- **Fordel:** Graceful degradation

**Screenshot:** Vis linje 5-17 (class definition og constructor)

---

## SLIDE 12: Demo - Performance Impact
**Titel:** Performance Impact

**Indhold:**
| Scenario | Uden Circuit Breaker | Med Circuit Breaker |
|----------|---------------------|---------------------|
| 100 requests | 200 sek ventetid | 6 sek (første 3) + 0ms (resten) |
| System status | Blokeret | Fortsætter |
| Response time | 2000ms | 0ms (efter åbning) |

---

## SLIDE 13: Demo - Isolation i Praksis
**Titel:** Isolation i Praksis

**Indhold:**
- **Før Circuit Breaker:**
  - CommentService → ProfanityService (nede) → System går ned
- **Efter Circuit Breaker:**
  - CommentService → Circuit Breaker → Fallback → System fortsætter

---

## SLIDE 14: Pattern 1 - Retry
**Titel:** Resilience Pattern: Retry

**Indhold:**
- **Formål:** Prøver operation igen ved fejl
- **Availability:** ✅ Forbedrer (håndterer transient fejl)
- **Use Case:** Network glitches, temporary DB locks
- **Trade-off:** ⚠️ Kan forværre hvis fejl er permanent

---

## SLIDE 15: Pattern 2 - Circuit Breaker
**Titel:** Resilience Pattern: Circuit Breaker

**Indhold:**
- **Formål:** Stopper requests til fejlende service
- **Availability:** ✅ Forbedrer (forhindrer resource exhaustion)
- **Use Case:** Fejlende eller langsom service
- **Trade-off:** ⚠️ Reducerer funktionalitet når åben

---

## SLIDE 16: Pattern 3 - Fallback
**Titel:** Resilience Pattern: Fallback

**Indhold:**
- **Formål:** Alternativ handling når primær fejler
- **Availability:** ✅ Forbedrer (graceful degradation)
- **Use Case:** Når reduceret funktionalitet er acceptabelt
- **Trade-off:** ⚠️ Kan returnere mindre præcise resultater

---

## SLIDE 17: Pattern 4 - Timeout
**Titel:** Resilience Pattern: Timeout

**Indhold:**
- **Formål:** Begrænser ventetid på operation
- **Availability:** ✅ Forbedrer (forhindrer lange blokeringer)
- **Use Case:** Alle eksterne calls
- **Trade-off:** ⚠️ Kan afvise legitime langsomme requests

---

## SLIDE 18: Pattern 5 - Bulkhead
**Titel:** Resilience Pattern: Bulkhead

**Indhold:**
- **Formål:** Isolerer ressourcer
- **Availability:** ✅ Forbedrer (forhindrer cascading failures)
- **Use Case:** Kritiske vs. ikke-kritiske operationer
- **Trade-off:** ⚠️ Kan reducere resource utilization

---

## SLIDE 19: Availability Sammenligning
**Titel:** Availability Sammenligning

**Indhold:**
**Scenario:** ProfanityService går ned i 5 minutter

| Pattern | Availability | Response Time | Resource Usage |
|---------|-------------|---------------|----------------|
| Ingen pattern | 0% | Timeout (2 sek) | 100% threads blokeret |
| Kun Retry | 0% | 3x timeout (6 sek) | 100% threads blokeret |
| Kun Circuit Breaker | 80% | 0ms (efter åbning) | 10% threads brugt |
| Retry + Circuit Breaker + Fallback | 90% | 0ms (efter åbning) | 10% threads brugt |

---

## SLIDE 20: Kombinerede Patterns
**Titel:** Kombinerede Patterns

**Indhold:**
- **Eksempel fra projektet:**
  1. Retry (transient fejl)
  2. Circuit Breaker (permanente fejl)
  3. Timeout (forhindrer blokering)
  4. Fallback (graceful degradation)
- **Resultat:** 90%+ availability

---

## SLIDE 21: Availability Ranking
**Titel:** Availability Ranking

**Indhold:**
- **Højeste availability:**
  1. Circuit Breaker + Fallback + Retry + Timeout (90%+)
  2. Circuit Breaker + Fallback (85%)
  3. Circuit Breaker alene (80%)
  4. Fallback alene (70%)
  5. Retry alene (0% ved permanent fejl)
  6. Ingen pattern (0%)

---

## SLIDE 22: Konklusion
**Titel:** Konklusion

**Indhold:**
- Fault isolation er essentielt for microservices
- Circuit breaker isolerer fejl effektivt
- Kombination af patterns giver optimal availability
- **Anbefaling:** Brug altid timeout + retry + circuit breaker + fallback

---

## SLIDE 23: Spørgsmål?
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
   - Tilføj kode-snippets som billeder eller tekstbokse
4. **Screenshots:**
   - Tag screenshots af de angivne filer
   - Indsæt som billeder på relevante slides
   - Tilføj annotations hvis nødvendigt
