# Spørgsmål 7: Specifikationer

## A. Forklar forskellen på formelle og uformelle specifikationer i forbindelse med outsourcing

### Formelle Specifikationer:

**Definition:**
Formelle specifikationer er præcise, matematisk eller strukturelt definerede beskrivelser af systemets opførsel, der kan verificeres formelt.

**Karakteristika:**
- **Præcision**: Matematisk eller strukturelt præcise
- **Verificerbarhed**: Kan verificeres formelt (test, proof)
- **Entydighed**: Ingen tvetydighed - én fortolkning
- **Komplethed**: Dækker alle edge cases og scenarier
- **Struktur**: Følger standardiseret format (fx Z, B, TLA+, eller struktureret format)

**Eksempler:**
- Pre-conditions og post-conditions
- Invariants
- Type definitions
- State machines
- API contracts (OpenAPI/Swagger)
- Test cases med expected outcomes

**Anvendelse i outsourcing:**
- Kritiske funktioner (sikkerhed, finans)
- Komplekse algoritmer
- Integrationer mellem systemer
- Compliance-kritiske funktioner

### Uformelle Specifikationer:

**Definition:**
Uformelle specifikationer er beskrivelser i naturligt sprog eller simple dokumenter, der forklarer hvad systemet skal gøre.

**Karakteristika:**
- **Fleksibilitet**: Kan fortolkes forskelligt
- **Læsbarhed**: Let at forstå for mennesker
- **Tvetydighed**: Kan have flere fortolkninger
- **Ufuldstændighed**: Kan mangle edge cases
- **Struktur**: Fri form (tekst, bullet points, skitser)

**Eksempler:**
- User stories
- Beskrivelser i naturligt sprog
- Skitser og mockups
- E-mails og chat-beskeder
- Verbale beskrivelser

**Anvendelse i outsourcing:**
- Simple funktioner
- Prototyper og MVP
- Hurtige projekter
- Når fleksibilitet er vigtigere end præcision

### Hovedforskelle:

| Aspekt | Formel Specifikation | Uformel Specifikation |
|--------|----------------------|----------------------|
| **Præcision** | Høj (matematisk) | Lav (naturligt sprog) |
| **Tvetydighed** | Ingen | Mulig |
| **Verificerbarhed** | Ja (automatisk) | Nej (manuel) |
| **Kost** | Høj (tager tid) | Lav (hurtig) |
| **Læsbarhed** | Lav (teknisk) | Høj (let at forstå) |
| **Komplethed** | Komplet | Kan være ufuldstændig |
| **Anvendelse** | Kritiske funktioner | Simple funktioner |

---

## B. Udarbejd et eksempel på en formel specifikation for en mindre funktion, der skal outsources

### Funktion: Profanity Check Service

**Kontekst:**
Happy-Headlines skal outsourcere udviklingen af en profanity check funktion. Funktionen skal tjekke om en given tekst indeholder upassende ord.

**Eksempel fra kodebasen:**
```csharp
// ProfanityService/Controllers/ProfanityController.cs (linje 14-28)
[HttpGet("check")]
public async Task<IActionResult> Check([FromQuery] string text, CancellationToken ct)
{
    var words = await _db.Words.AsNoTracking().Select(w => w.Word).ToListAsync(ct);
    var hits = new List<string>();
    if (!string.IsNullOrWhiteSpace(text))
    {
        var tokens = text.Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '\'', '"' },
                                StringSplitOptions.RemoveEmptyEntries);
        foreach (var t in tokens)
            if (words.Contains(t, StringComparer.OrdinalIgnoreCase)) hits.Add(t);
    }
    return Ok(new { contains = hits.Count > 0, words = hits });
}
```

### Formel Specifikation:

#### 1. **Function Signature**

```
Function: CheckProfanity
Input: text: String
Output: Result: { contains: Boolean, words: List<String> }
```

#### 2. **Pre-conditions**

```
Pre-Conditions:
  P1: text ∈ String ∪ {null, empty}
  P2: Database connection is available
  P3: Profanity word list is accessible
```

#### 3. **Post-conditions**

```
Post-Conditions:
  Q1: Result.contains = true ⟺ ∃ word ∈ ProfanityWords : word matches text (case-insensitive)
  Q2: Result.words = { w | w ∈ ProfanityWords ∧ w matches text (case-insensitive) }
  Q3: Result.words ⊆ ProfanityWords
  Q4: If text is null or empty: Result.contains = false ∧ Result.words = []
  Q5: Response time ≤ 2 seconds
  Q6: HTTP status code = 200 (OK)
```

#### 4. **Invariants**

```
Invariants:
  I1: ∀ word ∈ Result.words : word ∈ ProfanityWords
  I2: Result.contains = true ⟺ |Result.words| > 0
  I3: Case-insensitive matching: "Bad" matches "bad"
```

#### 5. **Tokenization Rules**

```
Tokenization:
  T1: Split text by delimiters: [' ', '.', ',', '!', '?', ';', ':', ''', '"']
  T2: Remove empty tokens
  T3: Each token is compared against ProfanityWords
  T4: Matching is case-insensitive (StringComparer.OrdinalIgnoreCase)
```

#### 6. **API Contract (OpenAPI/Swagger)**

```yaml
/api/profanity/check:
  get:
    summary: Check if text contains profanity
    parameters:
      - name: text
        in: query
        required: true
        schema:
          type: string
          example: "hello world"
    responses:
      200:
        description: Success
        content:
          application/json:
            schema:
              type: object
              properties:
                contains:
                  type: boolean
                  example: false
                words:
                  type: array
                  items:
                    type: string
                  example: []
      400:
        description: Bad request (missing text parameter)
      500:
        description: Internal server error
    performance:
      maxResponseTime: 2000ms
```

#### 7. **Test Cases (Formel Definition)**

```
Test Case 1:
  Input: text = "hello world"
  Expected: { contains: false, words: [] }
  Pre-condition: "hello", "world" ∉ ProfanityWords

Test Case 2:
  Input: text = "this is bad"
  Expected: { contains: true, words: ["bad"] }
  Pre-condition: "bad" ∈ ProfanityWords

Test Case 3:
  Input: text = "This is BAD"
  Expected: { contains: true, words: ["bad"] }
  Pre-condition: "bad" ∈ ProfanityWords (case-insensitive)

Test Case 4:
  Input: text = null
  Expected: { contains: false, words: [] }

Test Case 5:
  Input: text = ""
  Expected: { contains: false, words: [] }

Test Case 6:
  Input: text = "bad,ugly,stupid"
  Expected: { contains: true, words: ["bad", "ugly", "stupid"] }
  Pre-condition: "bad", "ugly", "stupid" ∈ ProfanityWords

Test Case 7:
  Input: text = "bad word"
  Expected: { contains: true, words: ["bad"] }
  Pre-condition: "bad" ∈ ProfanityWords, "word" ∉ ProfanityWords
```

#### 8. **Error Handling**

```
Error Conditions:
  E1: Database unavailable
    → HTTP 500, Error message: "Database connection failed"
  
  E2: text parameter missing
    → HTTP 400, Error message: "text parameter is required"
  
  E3: Timeout (> 2 seconds)
    → HTTP 504, Error message: "Request timeout"
```

#### 9. **Performance Requirements**

```
Performance:
  P1: Response time (p95) ≤ 2 seconds
  P2: Throughput ≥ 100 requests/second
  P3: Database query: Single query to fetch all profanity words
  P4: Memory: O(n) where n = number of profanity words
```

#### 10. **Integration Requirements**

```
Integration:
  I1: HTTP endpoint: GET /api/profanity/check?text={text}
  I2: Response format: JSON
  I3: Content-Type: application/json
  I4: Must support circuit breaker pattern (timeout: 2 seconds)
  I5: Must support distributed tracing (OpenTelemetry)
```

#### 11. **Data Model**

```
Data Model:
  ProfanityWord:
    - Word: String (primary key, case-insensitive)
    - CreatedAt: DateTime
    - UpdatedAt: DateTime

  CheckResult:
    - contains: Boolean
    - words: List<String>
```

#### 12. **State Machine (Hvis relevant)**

```
States:
  - Idle: Waiting for request
  - Processing: Checking text against word list
  - Success: Found result, returning response
  - Error: Error occurred, returning error response

Transitions:
  Idle → Processing: Request received
  Processing → Success: Check completed successfully
  Processing → Error: Error occurred
  Success → Idle: Response sent
  Error → Idle: Error response sent
```

---

## C. Sammenlign din formelle specifikation med en uformel version, og vurder fordele og ulemper

### Uformel Specifikation (Samme Funktion):

**Beskrivelse:**
"Vi skal have en funktion der tjekker om en tekst indeholder upassende ord. Funktionen skal tage en tekst som input og returnere om teksten indeholder upassende ord, og hvilke ord der blev fundet. Funktionen skal være hurtig og håndtere edge cases som tom tekst."

**User Story:**
"As a system administrator, I want to check if user comments contain profanity, so that I can filter out inappropriate content."

**Acceptance Criteria:**
- Funktionen skal tjekke tekst mod en liste af upassende ord
- Funktionen skal returnere om teksten indeholder upassende ord
- Funktionen skal returnere hvilke ord der blev fundet
- Funktionen skal være case-insensitive
- Funktionen skal håndtere tom tekst

### Sammenligning:

| Aspekt | Formel Specifikation | Uformel Specifikation |
|--------|----------------------|----------------------|
| **Længde** | ~200 linjer (detaljeret) | ~10 linjer (kort) |
| **Præcision** | Høj (matematisk præcis) | Lav (fortolkning nødvendig) |
| **Tvetydighed** | Ingen | Høj (mange spørgsmål) |
| **Komplethed** | Komplet (alle edge cases) | Ufuldstændig (mangler detaljer) |
| **Verificerbarhed** | Ja (automatisk tests) | Nej (manuel fortolkning) |
| **Læsbarhed** | Lav (teknisk) | Høj (let at forstå) |
| **Kost** | Høj (tager tid at skrive) | Lav (hurtig at skrive) |

### Fordele ved Formel Specifikation:

#### 1. **Præcision og Entydighed**
- ✅ Ingen tvetydighed - udvikleren ved præcist hvad der forventes
- ✅ Alle edge cases er defineret
- ✅ Matematisk præcis → Færre misforståelser

**Eksempel:**
- **Formel**: "Result.contains = true ⟺ ∃ word ∈ ProfanityWords : word matches text (case-insensitive)"
- **Uformel**: "Tjek om teksten indeholder upassende ord" → Hvad betyder "indeholder"? Hvad er "upassende ord"?

#### 2. **Verificerbarhed**
- ✅ Kan automatisk verificeres med tests
- ✅ Test cases er præcist defineret
- ✅ Kan bevise korrekthed (formel verification)

**Eksempel:**
- **Formel**: 7 præcise test cases med expected outcomes
- **Uformel**: "Test funktionen" → Hvad skal testes? Hvad er forventet resultat?

#### 3. **Komplethed**
- ✅ Alle scenarier er dækket (null, empty, multiple words, etc.)
- ✅ Error handling er defineret
- ✅ Performance requirements er specificeret

**Eksempel:**
- **Formel**: Test Case 4-7 dækker null, empty, multiple words, partial matches
- **Uformel**: "Håndter edge cases" → Hvilke edge cases?

#### 4. **Contract-based Development**
- ✅ API contract er præcist defineret (OpenAPI)
- ✅ Integration requirements er klare
- ✅ Kan generere client code automatisk

**Eksempel:**
- **Formel**: OpenAPI spec → Kan generere client SDK automatisk
- **Uformel**: "HTTP endpoint" → Hvilken metode? Hvilke parametre? Hvilket format?

#### 5. **Reducerer Rework**
- ✅ Færre iterationer (udvikleren ved præcist hvad der skal laves)
- ✅ Færre misforståelser → Færre bugs
- ✅ Hurtigere development (mindre spørgsmål)

### Ulemper ved Formel Specifikation:

#### 1. **Høj Kost**
- ❌ Tager lang tid at skrive (dage/uger)
- ❌ Kræver ekspertise i formel specifikation
- ❌ Kan være overkill for simple funktioner

**Eksempel:**
- **Formel**: 200 linjer specifikation for en simpel funktion
- **Uformel**: 10 linjer → 20x hurtigere at skrive

#### 2. **Lav Læsbarhed**
- ❌ Teknisk og svær at forstå for ikke-tekniske stakeholders
- ❌ Kræver matematisk/logisk baggrund
- ❌ Kan skræmme business stakeholders væk

**Eksempel:**
- **Formel**: "Result.contains = true ⟺ ∃ word ∈ ProfanityWords"
- **Uformel**: "Returner true hvis teksten indeholder upassende ord" → Alle forstår det

#### 3. **Rigiditet**
- ❌ Svær at ændre (kræver opdatering af hele specifikationen)
- ❌ Kan begrænse kreativitet i løsningen
- ❌ Kan være for restriktiv

#### 4. **Overhead**
- ❌ Kan være for detaljeret for simple funktioner
- ❌ Tager tid væk fra faktisk development
- ❌ Kan skabe "analysis paralysis"

### Fordele ved Uformel Specifikation:

#### 1. **Hurtig og Fleksibel**
- ✅ Hurtig at skrive (minutter)
- ✅ Let at ændre
- ✅ Giver udvikleren kreativitet

#### 2. **Læsbarhed**
- ✅ Let at forstå for alle
- ✅ Business stakeholders kan læse det
- ✅ God til kommunikation

#### 3. **Lav Kost**
- ✅ Billig at producere
- ✅ Ingen ekspertise nødvendig
- ✅ God til prototyper og MVP

### Ulemper ved Uformel Specifikation:

#### 1. **Tvetydighed**
- ❌ Mange spørgsmål fra udvikleren
- ❌ Forskellige fortolkninger
- ❌ Kan føre til forkert implementation

**Eksempel:**
- **Uformel**: "Tjek om teksten indeholder upassende ord"
  - Spørgsmål: Hvad er "indeholder"? Helt ord eller delord?
  - Spørgsmål: Hvad er "upassende ord"? Hvor kommer listen fra?
  - Spørgsmål: Case-sensitive eller ikke?

#### 2. **Ufuldstændighed**
- ❌ Mangler edge cases
- ❌ Mangler error handling
- ❌ Mangler performance requirements

**Eksempel:**
- **Uformel**: "Håndter edge cases" → Hvilke?
- **Formel**: Test Case 4-7 dækker alle edge cases

#### 3. **Høj Rework Risiko**
- ❌ Første implementation kan være forkert
- ❌ Mange iterationer nødvendige
- ❌ Kan føre til scope creep

#### 4. **Svær Verificering**
- ❌ Kan ikke automatisk verificeres
- ❌ Test cases skal fortolkes
- ❌ Kan ikke bevise korrekthed

### Anbefaling: Hybrid Approach

**Brug formel specifikation når:**
- ✅ Funktionen er kritisk (sikkerhed, finans)
- ✅ Outsourcing til ekstern udvikler
- ✅ Kompleks funktionalitet
- ✅ Integration mellem systemer
- ✅ Compliance-kritiske funktioner

**Brug uformel specifikation når:**
- ✅ Simpel funktionalitet
- ✅ Intern udvikling (samme team)
- ✅ Prototyper og MVP
- ✅ Hurtige projekter
- ✅ Når fleksibilitet er vigtigere end præcision

**Best Practice:**
1. Start med uformel specifikation (hurtig)
2. Udvid til formel specifikation for kritiske dele
3. Brug formel specifikation for API contracts og test cases
4. Brug uformel specifikation for business logic beskrivelser

### Konklusion:

**For outsourcing (som i eksemplet):**
- **Anbefaling**: Formel specifikation
- **Årsag**: Reducerer misforståelser, færre iterationer, bedre kvalitet
- **Trade-off**: Højere initial kost, men lavere total kost (mindre rework)

**For intern udvikling:**
- **Anbefaling**: Uformel specifikation (eller hybrid)
- **Årsag**: Hurtigere, mere fleksibelt, samme team kan diskutere
- **Trade-off**: Lavere initial kost, men højere risiko for misforståelser
