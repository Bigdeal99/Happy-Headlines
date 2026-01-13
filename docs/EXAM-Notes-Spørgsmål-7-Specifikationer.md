# EKSAMEN - Spørgsmål 7: Specifikationer
## Slide Outline & Speaking Notes

---

## SLIDE 1: Titel
**Speaking Notes:**
- "Jeg vil i dag præsentere Specifikationer i forbindelse med outsourcing"
- "Jeg dækker tre dele: forskellen på formelle og uformelle specifikationer, et eksempel på formel specifikation, og sammenligning med fordele og ulemper"
- "Jeg bruger ProfanityService fra Happy-Headlines som eksempel"

---

## PART A: FORKLAR FORSKELLEN

---

## SLIDE 2: Formelle Specifikationer - Definition
**Speaking Notes:**
- "Første type: Formelle Specifikationer"
- "Definition: Præcise, matematisk eller strukturelt definerede beskrivelser af systemets opførsel"
- "Kan verificeres formelt - test, proof"
- "Ingen tvetydighed - én fortolkning"
- "Dækker alle edge cases og scenarier"

---

## SLIDE 3: Formelle Specifikationer - Karakteristika
**Speaking Notes:**
- "Karakteristika for formelle specifikationer:"
- "Præcision: Matematisk eller strukturelt præcise"
- "Verificerbarhed: Kan verificeres formelt"
- "Entydighed: Ingen tvetydighed"
- "Komplethed: Dækker alle edge cases"
- "Struktur: Følger standardiseret format - fx Z, B, TLA+, eller OpenAPI/Swagger"

---

## SLIDE 4: Formelle Specifikationer - Eksempler
**Speaking Notes:**
- "Eksempler på formelle specifikationer:"
- "Pre-conditions og post-conditions"
- "Invariants"
- "Type definitions"
- "State machines"
- "API contracts - OpenAPI/Swagger"
- "Test cases med expected outcomes"

---

## SLIDE 5: Formelle Specifikationer - Anvendelse
**Speaking Notes:**
- "Anvendelse i outsourcing:"
- "Kritiske funktioner - sikkerhed, finans"
- "Komplekse algoritmer"
- "Integrationer mellem systemer"
- "Compliance-kritiske funktioner"
- "Dette er vigtigt når man outsourcer - reducerer misforståelser"

---

## SLIDE 6: Uformelle Specifikationer - Definition
**Speaking Notes:**
- "Anden type: Uformelle Specifikationer"
- "Definition: Beskrivelser i naturligt sprog eller simple dokumenter"
- "Forklarer hvad systemet skal gøre"
- "Kan fortolkes forskelligt - tvetydighed mulig"
- "Kan mangle edge cases"

---

## SLIDE 7: Uformelle Specifikationer - Karakteristika
**Speaking Notes:**
- "Karakteristika for uformelle specifikationer:"
- "Fleksibilitet: Kan fortolkes forskelligt"
- "Læsbarhed: Let at forstå for mennesker"
- "Tvetydighed: Kan have flere fortolkninger"
- "Ufuldstændighed: Kan mangle edge cases"
- "Struktur: Fri form - tekst, bullet points, skitser"

---

## SLIDE 8: Uformelle Specifikationer - Eksempler
**Speaking Notes:**
- "Eksempler på uformelle specifikationer:"
- "User stories"
- "Beskrivelser i naturligt sprog"
- "Skitser og mockups"
- "E-mails og chat-beskeder"
- "Verbale beskrivelser"

---

## SLIDE 9: Uformelle Specifikationer - Anvendelse
**Speaking Notes:**
- "Anvendelse i outsourcing:"
- "Simple funktioner"
- "Prototyper og MVP"
- "Hurtige projekter"
- "Når fleksibilitet er vigtigere end præcision"
- "Dette er fint når man har tæt samarbejde med udvikleren"

---

## SLIDE 10: Hovedforskelle - Tabel
**Speaking Notes:**
- "Hovedforskelle:"
- "Præcision: Formel er høj - matematisk, uformel er lav - naturligt sprog"
- "Tvetydighed: Formel har ingen, uformel kan have flere fortolkninger"
- "Verificerbarhed: Formel kan verificeres automatisk, uformel kræver manuel fortolkning"
- "Kost: Formel tager lang tid, uformel er hurtig"
- "Læsbarhed: Formel er teknisk, uformel er let at forstå"
- "Komplethed: Formel er komplet, uformel kan være ufuldstændig"

---

## PART B: EKSEMPEL PÅ FORMEL SPECIFIKATION

---

## SLIDE 11: Funktion - Profanity Check Service
**Speaking Notes:**
- "Nu viser jeg et eksempel på formel specifikation"
- "Funktion: Profanity Check Service"
- "Kontekst: Happy-Headlines skal outsourcere udviklingen af en profanity check funktion"
- "Funktionen skal tjekke om en given tekst indeholder upassende ord"
- "Dette er en mindre funktion, men kritisk for systemet"

**Screenshot Instructions:**
1. Åbn `ProfanityService/Controllers/ProfanityController.cs`
2. Marker Check metoden
3. Forklar at dette er funktionen vi skal specificere

---

## SLIDE 12: Formel Specifikation - Function Signature
**Speaking Notes:**
- "Første del: Function Signature"
- "Function: CheckProfanity"
- "Input: text - String"
- "Output: Result - objekt med contains (Boolean) og words (List<String>)"
- "Dette er præcist defineret - ingen tvetydighed"

---

## SLIDE 13: Formel Specifikation - Pre-conditions
**Speaking Notes:**
- "Anden del: Pre-conditions"
- "P1: text kan være String, null eller empty"
- "P2: Database connection skal være tilgængelig"
- "P3: Profanity word list skal være tilgængelig"
- "Dette definerer hvad der skal være opfyldt før funktionen kører"

---

## SLIDE 14: Formel Specifikation - Post-conditions
**Speaking Notes:**
- "Tredje del: Post-conditions"
- "Q1: Result.contains er true hvis og kun hvis der findes et ord i ProfanityWords der matcher teksten - case-insensitive"
- "Q2: Result.words er mængden af alle ord fra ProfanityWords der matcher teksten"
- "Q3: Hvis text er null eller empty, skal contains være false og words være tom"
- "Q4: Response time skal være ≤ 2 sekunder"
- "Q5: HTTP status code skal være 200"
- "Dette er matematisk præcist - ingen tvetydighed"

---

## SLIDE 15: Formel Specifikation - API Contract (OpenAPI)
**Speaking Notes:**
- "Fjerde del: API Contract (OpenAPI)"
- "Dette er en struktureret specifikation"
- "Endpoint: GET /api/profanity/check?text={text}"
- "Response: JSON objekt med contains (boolean) og words (array af strings)"
- "Performance: maxResponseTime 2000ms"
- "Dette kan generere client code automatisk"

**Screenshot Instructions:**
1. Vis OpenAPI specifikation fra dokumentation
2. Forklar at dette er struktureret format
3. Vis at det kan generere client SDK automatisk

---

## SLIDE 16: Formel Specifikation - Test Cases
**Speaking Notes:**
- "Femte del: Test Cases"
- "Test Case 1: 'hello world' - ingen profanity → contains false, words tom"
- "Test Case 2: 'this is bad' - profanity fundet → contains true, words ['bad']"
- "Test Case 3: 'This is BAD' - case-insensitive matching → contains true, words ['bad']"
- "Test Case 4: null input → contains false, words tom"
- "Test Case 5: empty string → contains false, words tom"
- "Alle test cases er præcist defineret med expected outcomes"

---

## SLIDE 17: Formel Specifikation - Error Handling
**Speaking Notes:**
- "Sjette del: Error Handling"
- "E1: Database unavailable → HTTP 500"
- "E2: text parameter missing → HTTP 400"
- "E3: Timeout (> 2 seconds) → HTTP 504"
- "Alle error conditions er defineret - ingen gætteri"

---

## PART C: SAMMENLIGNING

---

## SLIDE 18: Uformel Specifikation - Eksempel
**Speaking Notes:**
- "Nu sammenligner jeg med uformel specifikation"
- "Beskrivelse: 'Vi skal have en funktion der tjekker om en tekst indeholder upassende ord'"
- "User Story: 'As a system administrator, I want to check if user comments contain profanity'"
- "Dette er let at forstå, men har mange spørgsmål:"
- "Hvad betyder 'indeholder'? Helt ord eller delord?"
- "Hvad er 'upassende ord'? Hvor kommer listen fra?"
- "Hvad betyder 'hurtig'? 1 sekund? 5 sekunder?"
- "Hvilke edge cases? Kun tom tekst?"

---

## SLIDE 19: Sammenligning - Tabel
**Speaking Notes:**
- "Sammenligning:"
- "Længde: Formel er ~200 linjer, uformel er ~10 linjer"
- "Præcision: Formel er høj - matematisk, uformel er lav - kræver fortolkning"
- "Tvetydighed: Formel har ingen, uformel har høj - mange spørgsmål"
- "Komplethed: Formel er komplet - alle edge cases, uformel er ufuldstændig - mangler detaljer"
- "Verificerbarhed: Formel kan verificeres automatisk, uformel kræver manuel fortolkning"
- "Kost: Formel tager lang tid, uformel er hurtig"

---

## SLIDE 20: Fordele ved Formel Specifikation
**Speaking Notes:**
- "Fordele ved Formel Specifikation:"
- "Præcision og Entydighed: Ingen tvetydighed - udvikleren ved præcist hvad der forventes, alle edge cases er defineret"
- "Verificerbarhed: Kan automatisk verificeres med tests, test cases er præcist defineret"
- "Komplethed: Alle scenarier er dækket - null, empty, multiple words, error handling"
- "Contract-based Development: API contract er præcist defineret, kan generere client code automatisk"
- "Reducerer Rework: Færre iterationer, færre misforståelser, færre bugs, hurtigere development"

---

## SLIDE 21: Ulemper ved Formel Specifikation
**Speaking Notes:**
- "Ulemper ved Formel Specifikation:"
- "Høj Kost: Tager lang tid at skrive - dage eller uger, kræver ekspertise, kan være overkill for simple funktioner"
- "Lav Læsbarhed: Teknisk og svær at forstå for ikke-tekniske stakeholders, kræver matematisk/logisk baggrund"
- "Rigiditet: Svær at ændre - kræver opdatering af hele specifikationen, kan begrænse kreativitet"
- "Overhead: Kan være for detaljeret for simple funktioner, tager tid væk fra faktisk development"

---

## SLIDE 22: Fordele ved Uformel Specifikation
**Speaking Notes:**
- "Fordele ved Uformel Specifikation:"
- "Hurtig og Fleksibel: Hurtig at skrive - minutter, let at ændre, giver udvikleren kreativitet"
- "Læsbarhed: Let at forstå for alle, business stakeholders kan læse det, god til kommunikation"
- "Lav Kost: Billig at producere, ingen ekspertise nødvendig, god til prototyper og MVP"

---

## SLIDE 23: Ulemper ved Uformel Specifikation
**Speaking Notes:**
- "Ulemper ved Uformel Specifikation:"
- "Tvetydighed: Mange spørgsmål fra udvikleren, forskellige fortolkninger, kan føre til forkert implementation"
- "Ufuldstændighed: Mangler edge cases, mangler error handling, mangler performance requirements"
- "Høj Rework Risiko: Første implementation kan være forkert, mange iterationer nødvendige, kan føre til scope creep"
- "Svær Verificering: Kan ikke automatisk verificeres, test cases skal fortolkes, kan ikke bevise korrekthed"

---

## SLIDE 24: Anbefaling - Hybrid Approach
**Speaking Notes:**
- "Anbefaling: Hybrid Approach"
- "Brug formel specifikation når: Funktionen er kritisk - sikkerhed, finans, outsourcing til ekstern udvikler, kompleks funktionalitet, integration mellem systemer"
- "Brug uformel specifikation når: Simpel funktionalitet, intern udvikling - samme team, prototyper og MVP, hurtige projekter"
- "Best Practice: Start med uformel specifikation - hurtig, udvid til formel specifikation for kritiske dele, brug formel specifikation for API contracts og test cases"

---

## SLIDE 25: Konklusion
**Speaking Notes:**
- "Konklusion:"
- "For outsourcing (som i eksemplet): Anbefaling - Formel specifikation"
- "Årsag: Reducerer misforståelser, færre iterationer, bedre kvalitet"
- "Trade-off: Højere initial kost, men lavere total kost - mindre rework"
- "For intern udvikling: Anbefaling - Uformel specifikation eller hybrid"
- "Årsag: Hurtigere, mere fleksibelt, samme team kan diskutere"
- "Trade-off: Lavere initial kost, men højere risiko for misforståelser"

---

## SLIDE 26: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres opmærksomhed"
- "Jeg er klar til spørgsmål"

---

## EKSTRA NOTER TIL EKSAMEN:

### Hvis de spørger om specifikke filer:
- **`ProfanityService/Controllers/ProfanityController.cs`**: Eksempel funktion til specifikation
- **`docs/Spørgsmål-7-Specifikationer.md`**: Fuld specifikation med alle detaljer

### Hvis de spørger om formel specifikation:
- Function Signature: CheckProfanity(text: String) → Result
- Pre-conditions: text kan være null/empty, database tilgængelig
- Post-conditions: Result.contains = true ⟺ ∃ word ∈ ProfanityWords : word matches text
- API Contract: OpenAPI/Swagger specifikation
- Test Cases: 7 præcise test cases med expected outcomes
- Error Handling: 3 error conditions defineret

### Hvis de spørger om uformel specifikation:
- Beskrivelse: "Vi skal have en funktion der tjekker om en tekst indeholder upassende ord"
- User Story: "As a system administrator, I want to check if user comments contain profanity"
- Acceptance Criteria: 5 korte kriterier

### Hvis de spørger om sammenligning:
- Formel: Høj præcision, ingen tvetydighed, verificerbar, komplet, høj kost
- Uformel: Lav præcision, tvetydig, ikke verificerbar, ufuldstændig, lav kost
- Anbefaling: Formel for outsourcing/kritiske funktioner, uformel for simple/interne funktioner
