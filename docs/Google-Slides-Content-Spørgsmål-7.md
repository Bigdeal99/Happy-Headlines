# Google Slides Content - Spørgsmål 7: Specifikationer
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Specifikationer i Outsourcing
**Undertitel:** Formelle vs uformelle specifikationer
**Footer:** Dit navn | Dato

---

## SLIDE 2: Formelle Specifikationer - Definition
**Titel:** Formelle Specifikationer - Definition

**Indhold:**
- **Definition:** Præcise, matematisk eller strukturelt definerede beskrivelser
- Kan verificeres formelt (test, proof)
- Ingen tvetydighed - én fortolkning

---

## SLIDE 3: Formelle Specifikationer - Karakteristika
**Titel:** Formelle Specifikationer - Karakteristika

**Indhold:**
- **Præcision:** Matematisk eller strukturelt præcise
- **Verificerbarhed:** Kan verificeres formelt
- **Entydighed:** Ingen tvetydighed
- **Komplethed:** Dækker alle edge cases
- **Struktur:** Følger standardiseret format (Z, B, TLA+, OpenAPI)

---

## SLIDE 4: Formelle Specifikationer - Eksempler
**Titel:** Formelle Specifikationer - Eksempler

**Indhold:**
- Pre-conditions og post-conditions
- Invariants
- Type definitions
- State machines
- API contracts (OpenAPI/Swagger)
- Test cases med expected outcomes

---

## SLIDE 5: Formelle Specifikationer - Anvendelse
**Titel:** Formelle Specifikationer - Anvendelse

**Indhold:**
- Kritiske funktioner (sikkerhed, finans)
- Komplekse algoritmer
- Integrationer mellem systemer
- Compliance-kritiske funktioner

---

## SLIDE 6: Uformelle Specifikationer - Definition
**Titel:** Uformelle Specifikationer - Definition

**Indhold:**
- **Definition:** Beskrivelser i naturligt sprog eller simple dokumenter
- Forklarer hvad systemet skal gøre
- Kan fortolkes forskelligt

---

## SLIDE 7: Uformelle Specifikationer - Karakteristika
**Titel:** Uformelle Specifikationer - Karakteristika

**Indhold:**
- **Fleksibilitet:** Kan fortolkes forskelligt
- **Læsbarhed:** Let at forstå for mennesker
- **Tvetydighed:** Kan have flere fortolkninger
- **Ufuldstændighed:** Kan mangle edge cases
- **Struktur:** Fri form (tekst, bullet points, skitser)

---

## SLIDE 8: Uformelle Specifikationer - Eksempler
**Titel:** Uformelle Specifikationer - Eksempler

**Indhold:**
- User stories
- Beskrivelser i naturligt sprog
- Skitser og mockups
- E-mails og chat-beskeder
- Verbale beskrivelser

---

## SLIDE 9: Uformelle Specifikationer - Anvendelse
**Titel:** Uformelle Specifikationer - Anvendelse

**Indhold:**
- Simple funktioner
- Prototyper og MVP
- Hurtige projekter
- Når fleksibilitet er vigtigere end præcision

---

## SLIDE 10: Hovedforskelle - Tabel
**Titel:** Hovedforskelle

**Indhold:**
| Aspekt | Formel Specifikation | Uformel Specifikation |
|--------|----------------------|----------------------|
| Præcision | Høj (matematisk) | Lav (naturligt sprog) |
| Tvetydighed | Ingen | Mulig |
| Verificerbarhed | Ja (automatisk) | Nej (manuel) |
| Kost | Høj (tager tid) | Lav (hurtig) |
| Læsbarhed | Lav (teknisk) | Høj (let at forstå) |
| Komplethed | Komplet | Kan være ufuldstændig |

---

## SLIDE 11: Funktion - Profanity Check Service
**Titel:** Funktion - Profanity Check Service

**Indhold:**
- **Funktion:** Profanity Check Service
- **Kontekst:** Happy-Headlines skal outsourcere udviklingen
- **Fil:** `ProfanityService/Controllers/ProfanityController.cs`
- **Formål:** Tjekke om tekst indeholder upassende ord

**Screenshot:** Vis `ProfanityService/Controllers/ProfanityController.cs` Check metoden

---

## SLIDE 12: Formel Specifikation - Function Signature
**Titel:** Formel Specifikation - Function Signature

**Indhold:**
- **Function:** CheckProfanity
- **Input:** text: String
- **Output:** Result: { contains: Boolean, words: List<String> }

---

## SLIDE 13: Formel Specifikation - Pre-conditions
**Titel:** Formel Specifikation - Pre-conditions

**Indhold:**
- **Pre-Conditions:**
  - P1: text ∈ String ∪ {null, empty}
  - P2: Database connection is available
  - P3: Profanity word list is accessible

---

## SLIDE 14: Formel Specifikation - Post-conditions
**Titel:** Formel Specifikation - Post-conditions

**Indhold:**
- **Post-Conditions:**
  - Q1: Result.contains = true ⟺ ∃ word ∈ ProfanityWords : word matches text (case-insensitive)
  - Q2: Result.words = { w | w ∈ ProfanityWords ∧ w matches text }
  - Q3: If text is null or empty: Result.contains = false ∧ Result.words = []
  - Q4: Response time ≤ 2 seconds
  - Q5: HTTP status code = 200 (OK)

---

## SLIDE 15: Formel Specifikation - API Contract (OpenAPI)
**Titel:** Formel Specifikation - API Contract (OpenAPI)

**Indhold:**
- **Endpoint:** GET /api/profanity/check?text={text}
- **Response:** { contains: boolean, words: string[] }
- **Performance:** maxResponseTime: 2000ms
- **Error Codes:** 400 (Bad Request), 500 (Internal Server Error), 504 (Timeout)

**Note:** Vis OpenAPI/Swagger specifikation hvis tilgængelig

---

## SLIDE 16: Formel Specifikation - Test Cases
**Titel:** Formel Specifikation - Test Cases

**Indhold:**
- **Test Case 1:** text = "hello world" → { contains: false, words: [] }
- **Test Case 2:** text = "this is bad" → { contains: true, words: ["bad"] }
- **Test Case 3:** text = "This is BAD" → { contains: true, words: ["bad"] } (case-insensitive)
- **Test Case 4:** text = null → { contains: false, words: [] }
- **Test Case 5:** text = "" → { contains: false, words: [] }

---

## SLIDE 17: Formel Specifikation - Error Handling
**Titel:** Formel Specifikation - Error Handling

**Indhold:**
- **Error Conditions:**
  - E1: Database unavailable → HTTP 500
  - E2: text parameter missing → HTTP 400
  - E3: Timeout (> 2 seconds) → HTTP 504

---

## SLIDE 18: Uformel Specifikation - Eksempel
**Titel:** Uformel Specifikation - Eksempel

**Indhold:**
- **Beskrivelse:** "Vi skal have en funktion der tjekker om en tekst indeholder upassende ord. Funktionen skal tage en tekst som input og returnere om teksten indeholder upassende ord, og hvilke ord der blev fundet. Funktionen skal være hurtig og håndtere edge cases som tom tekst."
- **User Story:** "As a system administrator, I want to check if user comments contain profanity, so that I can filter out inappropriate content."

---

## SLIDE 19: Sammenligning - Tabel
**Titel:** Sammenligning - Tabel

**Indhold:**
| Aspekt | Formel Specifikation | Uformel Specifikation |
|--------|----------------------|----------------------|
| Længde | ~200 linjer | ~10 linjer |
| Præcision | Høj (matematisk) | Lav (fortolkning) |
| Tvetydighed | Ingen | Høj (mange spørgsmål) |
| Komplethed | Komplet (alle edge cases) | Ufuldstændig (mangler detaljer) |
| Verificerbarhed | Ja (automatisk tests) | Nej (manuel fortolkning) |
| Kost | Høj (tager tid) | Lav (hurtig) |

---

## SLIDE 20: Fordele ved Formel Specifikation
**Titel:** Fordele ved Formel Specifikation

**Indhold:**
- **Præcision og Entydighed:** Ingen tvetydighed, alle edge cases defineret
- **Verificerbarhed:** Kan automatisk verificeres med tests
- **Komplethed:** Alle scenarier dækket (null, empty, multiple words)
- **Contract-based Development:** API contract præcist defineret
- **Reducerer Rework:** Færre iterationer, færre bugs

---

## SLIDE 21: Ulemper ved Formel Specifikation
**Titel:** Ulemper ved Formel Specifikation

**Indhold:**
- **Høj Kost:** Tager lang tid at skrive (dage/uger)
- **Lav Læsbarhed:** Teknisk og svær at forstå for ikke-tekniske stakeholders
- **Rigiditet:** Svær at ændre, kan begrænse kreativitet
- **Overhead:** Kan være for detaljeret for simple funktioner

---

## SLIDE 22: Fordele ved Uformel Specifikation
**Titel:** Fordele ved Uformel Specifikation

**Indhold:**
- **Hurtig og Fleksibel:** Hurtig at skrive (minutter), let at ændre
- **Læsbarhed:** Let at forstå for alle, business stakeholders kan læse det
- **Lav Kost:** Billig at producere, ingen ekspertise nødvendig

---

## SLIDE 23: Ulemper ved Uformel Specifikation
**Titel:** Ulemper ved Uformel Specifikation

**Indhold:**
- **Tvetydighed:** Mange spørgsmål fra udvikleren, forskellige fortolkninger
- **Ufuldstændighed:** Mangler edge cases, mangler error handling
- **Høj Rework Risiko:** Første implementation kan være forkert, mange iterationer
- **Svær Verificering:** Kan ikke automatisk verificeres, test cases skal fortolkes

---

## SLIDE 24: Anbefaling - Hybrid Approach
**Titel:** Anbefaling - Hybrid Approach

**Indhold:**
- **Brug formel specifikation når:**
  - Funktionen er kritisk (sikkerhed, finans)
  - Outsourcing til ekstern udvikler
  - Kompleks funktionalitet
  - Integration mellem systemer
- **Brug uformel specifikation når:**
  - Simpel funktionalitet
  - Intern udvikling (samme team)
  - Prototyper og MVP
  - Hurtige projekter

---

## SLIDE 25: Konklusion
**Titel:** Konklusion

**Indhold:**
- **For outsourcing:** Anbefaling - Formel specifikation
- **Årsag:** Reducerer misforståelser, færre iterationer, bedre kvalitet
- **Trade-off:** Højere initial kost, men lavere total kost (mindre rework)
- **For intern udvikling:** Anbefaling - Uformel specifikation (eller hybrid)

---

## SLIDE 26: Spørgsmål?
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
   - Tilføj matematiske formler som billeder eller LaTeX hvis muligt
4. **Screenshots:**
   - Tag screenshots af `ProfanityService/Controllers/ProfanityController.cs`
   - Indsæt som billeder på relevante slides
   - Tilføj annotations hvis nødvendigt
5. **Matematiske formler:**
   - Slide 14: Brug LaTeX eller billeder for matematiske symboler (⟺, ∃, ∈, etc.)
   - Slide 13: Brug matematiske symboler (∪, ∈, etc.)
