# EKSAMEN - Spørgsmål 9: Organisatorisk Skalering
## Slide Outline & Speaking Notes

---

## SLIDE 1: Titel
**Speaking Notes:**
- "Jeg vil i dag præsentere Organisatorisk Skalering"
- "Jeg dækker tre dele: udfordringer ved team vækst, risiko-analyse for framework transition, og go/no-go beslutningsproces"
- "Jeg bruger Happy-Headlines microservices arkitektur som eksempel"

---

## PART A: BESKRIV UDFORDRINGER

---

## SLIDE 2: Udfordring 1 - Communication Overhead
**Speaking Notes:**
- "Første udfordring: Communication Overhead"
- "Lille gruppe: Informel kommunikation - standups, ad-hoc, alle kender alle, hurtige beslutninger"
- "Stor organisation: Formel kommunikation nødvendig - meetings, dokumentation"
- "Kommunikationskanaler vokser eksponentielt: n(n-1)/2"
- "Eksempel: 5 personer = 10 kanaler, 50 personer = 1,225 kanaler"
- "Problem: Information overload, misforståelser"

---

## SLIDE 3: Udfordring 2 - Coordination Complexity
**Speaking Notes:**
- "Anden udfordring: Coordination Complexity"
- "Lille gruppe: Simpel koordinering - face-to-face, alle ved hvad andre laver, let at aligne på mål"
- "Stor organisation: Kompleks koordinering - mellem teams, afdelinger, information silos, alignment kræver formelle processer"
- "Eksempel fra microservices: Teams ejer forskellige services - koordinering nødvendig for cross-service changes"
- "I Happy-Headlines: 6 services → 6 teams → Koordinering nødvendig"

---

## SLIDE 4: Udfordring 3 - Knowledge Silos
**Speaking Notes:**
- "Tredje udfordring: Knowledge Silos"
- "Lille gruppe: Delte viden - pair programming, code reviews, bus factor høj - flere ved hvordan systemet virker"
- "Stor organisation: Viden koncentreret i få personer, bus factor lav - hvis én person forlader, går viden tabt, onboarding tager længere tid"
- "Eksempel: Lille team - alle kan arbejde på ArticleService, stor organisation - kun ArticleService team kender ArticleService → Knowledge silo"

---

## SLIDE 5: Udfordring 4 - Process Overhead
**Speaking Notes:**
- "Fjerde udfordring: Process Overhead"
- "Lille gruppe: Letvægts processer - Scrum, Kanban, hurtig iteration, fleksibel"
- "Stor organisation: Tunge processer - SAFe, LeSS, langsommere iteration, mindre fleksibel"
- "Problem: Process overhead kan overstige værdien, innovation kan blive hæmmet"
- "Dette er en trade-off - skalering kræver processer, men processer reducerer fleksibilitet"

---

## SLIDE 6: Udfordring 5 - Dependency Management
**Speaking Notes:**
- "Femte udfordring: Dependency Management"
- "Lille gruppe: Få dependencies mellem teams, let at koordinere"
- "Stor organisation: Mange dependencies mellem teams, kompleks dependency graph, blocking issues - Team A venter på Team B"
- "Eksempel fra Happy-Headlines: PublisherService → ProfanityService (dependency), CommentService → ProfanityService (dependency)"
- "Problem: Hvis ProfanityService team er langsomt, blokerer det PublisherService og CommentService"
- "Dette viser kompleksiteten ved microservices med mange dependencies"

---

## SLIDE 7: Udfordring 6 - Culture Dilution
**Speaking Notes:**
- "Sjette udfordring: Culture Dilution"
- "Lille gruppe: Stærk kultur - samme værdier, normer, hurtig kultur-spredning"
- "Stor organisation: Kultur bliver fortyndet, sub-kulturer opstår - per team eller afdeling, kultur-spredning tager længere tid"
- "Dette kan skabe 'us vs. them' mentalitet mellem teams"

---

## SLIDE 8: Udfordring 7 - Decision Making Slowdown
**Speaking Notes:**
- "Syvende udfordring: Decision Making Slowdown"
- "Lille gruppe: Hurtige beslutninger - få personer involveret, konsensus let at opnå"
- "Stor organisation: Langsomme beslutninger - mange stakeholders, konsensus svær at opnå, hierarkier og approval gates"
- "Dette kan hæmme innovation og hurtig iteration"

---

## SLIDE 9: Udfordring 8 - Quality Consistency
**Speaking Notes:**
- "Ottende udfordring: Quality Consistency"
- "Lille gruppe: Konsistent kvalitet - samme standarder, code reviews let at koordinere"
- "Stor organisation: Inkonsistent kvalitet - forskellige standarder per team, code reviews komplekse - cross-team, standards svære at håndhæve"
- "Dette kan føre til teknisk gæld og inkonsistent codebase"

---

## SLIDE 10: Udfordring 9 - Innovation vs. Stability
**Speaking Notes:**
- "Niende udfordring: Innovation vs. Stability"
- "Lille gruppe: Innovation let - hurtig iteration, eksperimentering, risiko acceptabel"
- "Stor organisation: Innovation svær - processer, approval gates, risiko-avers - stabilitet vigtigere, 'Innovation tax' høj"
- "Dette er en klassisk trade-off - skalering kræver stabilitet, men stabilitet hæmmer innovation"

---

## SLIDE 11: Udfordring 10 - Resource Allocation
**Speaking Notes:**
- "Tiende udfordring: Resource Allocation"
- "Lille gruppe: Let at allokere ressourcer - få personer, prioritering simpel"
- "Stor organisation: Kompleks resource allocation - mange teams, projekter, prioritering kræver formelle processer, resource contention"
- "Dette kan føre til politiske kampe om ressourcer"

---

## SLIDE 12: Konkrete Eksempler fra Happy-Headlines
**Speaking Notes:**
- "Konkrete Eksempler fra Happy-Headlines:"
- "Happy-Headlines har 6 services: ArticleService, CommentService, ProfanityService, DraftService, PublisherService, NewsletterService"
- "Lille team (5 personer): Alle kan arbejde på alle services, ingen koordinering nødvendig, hurtig iteration"
- "Stor organisation (30+ personer): 6 teams - 1 per service, koordinering nødvendig, cross-service changes kræver multiple teams, slower iteration"
- "Dette viser hvordan microservices arkitektur skaber organisatoriske udfordringer ved skalering"

---

## PART B: RISIKO-ANALYSE

---

## SLIDE 13: Risiko-analyse Format (FMEA-baseret)
**Speaking Notes:**
- "Risiko-analyse Format:"
- "Vi bruger FMEA-baseret format - samme som i CI/CD risiko-analyse"
- "Format: ID, Risiko, Svarighed (S), Sandsynlighed (L), RPN (S×L), Mitigation, Prioritet"
- "Skala: S og L fra 1-10, RPN fra 1-100"
- "Dette giver os en struktureret måde at evaluere risici på"

---

## SLIDE 14: Top 5 Risici - Oversigt
**Speaking Notes:**
- "Top 5 Risici:"
- "R1: Resistance to Change - RPN 56, Kritisk - team members modsætter sig nyt framework"
- "R2: Process Overhead - RPN 56, Kritisk - nyt framework introducerer mere overhead"
- "R3: Loss of Team Autonomy - RPN 48, Høj - skaleret frameworks kan reducere team autonomy"
- "R4: Training Costs - RPN 48, Høj - omfattende training nødvendig"
- "R5: Productivity Drop - RPN 45, Høj - produktivitet falder under transition"
- "Dette viser de største risici ved framework transition"

---

## SLIDE 15: R1 - Resistance to Change (Kritisk)
**Speaking Notes:**
- "R1: Resistance to Change - Kritisk"
- "Svarighed: 8 - kan stoppe hele transitionen, kan skabe toxic kultur, kan føre til talent loss"
- "Sandsynlighed: 7 - høj, mange teams modsætter sig typisk"
- "RPN: 56 - Kritisk"
- "Mitigation: Change management - involver teams i valg af framework, kommunikér 'why' ikke kun 'what', adresser concerns proaktivt"
- "Training: Omfattende training før rollout, internal champions per team, continuous support"
- "Communication: Transparent kommunikation om forandringer, regular feedback sessions, celebrate wins"
- "Residual Risk: 2 - lav efter mitigation"

---

## SLIDE 16: R2 - Process Overhead (Kritisk)
**Speaking Notes:**
- "R2: Process Overhead - Kritisk"
- "Svarighed: 7 - reducerer produktivitet, kan skabe frustration, kan føre til process rejection"
- "Sandsynlighed: 8 - meget høj, skaleret frameworks har typisk mere overhead"
- "RPN: 56 - Kritisk"
- "Mitigation: Start Small - pilot med 1-2 teams først, mål overhead (time tracking), adapt process baseret på feedback"
- "Measure Overhead: Track time spent i meetings, compare før/efter, optimize kontinuerligt"
- "Adapt Framework: Tag kun det der giver værdi, drop unødvendige ceremonies, customize til organisation"
- "Residual Risk: 2 - lav efter mitigation"

---

## SLIDE 17: R3 - Loss of Team Autonomy (Høj)
**Speaking Notes:**
- "R3: Loss of Team Autonomy - Høj"
- "Svarighed: 8 - reducerer motivation, kan føre til talent loss, kan skabe 'us vs. them' kultur"
- "Sandsynlighed: 6 - medium-høj, afhænger af framework implementation"
- "RPN: 48 - Høj"
- "Mitigation: Preserve Autonomy - delegate beslutninger til teams, teams ejer deres backlog, minimal central control"
- "Empower Teams: Teams kan adapt process, teams kan vælge tools, teams kan prioritere work"
- "Framework Selection: Vælg framework der bevarer autonomy - fx Spotify model, undgå top-down frameworks - fx SAFe"
- "Residual Risk: 2 - lav efter mitigation"

---

## SLIDE 18: Framework-specifikke Risici
**Speaking Notes:**
- "Framework-specifikke Risici:"
- "SAFe: Top-down Control - RPN 56, PI Planning Overhead - RPN 56, Role Complexity - RPN 42"
- "LeSS: Requires Scrum Maturity - RPN 42, Coordination Complexity - RPN 42, Feature Team Transition - RPN 35"
- "Spotify: Requires Strong Culture - RPN 48, Informal Structure - RPN 30, Scaling Challenges - RPN 28"
- "Dette viser at forskellige frameworks har forskellige risici - vælg framework der passer til organisationen"

---

## PART C: GO/NO-GO BESLUTNING

---

## SLIDE 19: Step 1 - Risk Assessment
**Speaking Notes:**
- "Step 1: Risk Assessment"
- "Beregn Total Risk Score: Sum af alle RPN værdier = 391"
- "Kategoriser Risici:"
- "Kritiske (RPN ≥ 50): R1, R2 → 2 risici"
- "Høje (RPN 30-49): R3, R4, R5, R6, R7 → 5 risici"
- "Medium (RPN 20-29): R8, R9 → 2 risici"
- "Lave (RPN < 20): R10 → 1 risiko"
- "Dette giver os et overblik over risiko-niveauet"

---

## SLIDE 20: Step 2 - Mitigation Assessment
**Speaking Notes:**
- "Step 2: Mitigation Assessment"
- "Evaluer Mitigation Effektivitet:"
- "Kritiske risici: Alle har mitigation → Residual risk: 2 (lav)"
- "Høje risici: Alle har mitigation → Residual risk: 2 (lav)"
- "Total Residual Risk: 10 × 2 = 20 (meget lav)"
- "Mitigation Cost: Training 100,000 DKK, Consultants 200,000 DKK, Tooling 50,000 DKK, Total 350,000 DKK"
- "Dette viser at risici kan mitigere, men til en kost"

---

## SLIDE 21: Step 3 - Cost-Benefit Analysis
**Speaking Notes:**
- "Step 3: Cost-Benefit Analysis"
- "Costs: Direct Costs 350,000 DKK (mitigation), Productivity Loss 20% i 6 måneder = 500,000 DKK, Total 850,000 DKK"
- "Benefits: Better Coordination - 30% reduktion i blocking issues, Faster Delivery - 20% forbedring i time-to-market, Scalability - kan håndtere 2x team growth, Estimated Value 2,000,000 DKK/år"
- "ROI: (2,000,000 - 850,000) / 850,000 × 100% = 135%"
- "Payback Period: 850,000 / 2,000,000 = 0.425 år - ca. 5 måneder"
- "Dette viser at investeringen er værdifuld"

---

## SLIDE 22: Step 4 - Go/No-Go Decision Matrix
**Speaking Notes:**
- "Step 4: Go/No-Go Decision Matrix"
- "Kritiske Risici: Threshold ≤ 3, Actual 2 → ✅ PASS"
- "Residual Risk: Threshold ≤ 30, Actual 20 → ✅ PASS"
- "ROI: Threshold ≥ 100%, Actual 135% → ✅ PASS"
- "Payback Period: Threshold ≤ 12 måneder, Actual 5 måneder → ✅ PASS"
- "Team Readiness: Threshold ≥ 70%, Actual 75% → ✅ PASS"
- "Management Support: Threshold Ja, Actual Ja → ✅ PASS"
- "Decision: ✅ GO - Alle kriterier opfyldt"

---

## SLIDE 23: Conditional Go Decision
**Speaking Notes:**
- "Conditional Go Decision:"
- "Hvis nogle kriterier ikke opfyldes:"
- "Scenario 1: Høj Resistance (R1 ikke mitigerede) → Condition: Address resistance først, Action: 3 måneders change management program, Re-evaluate: Efter change management"
- "Scenario 2: Lav ROI (< 100%) → Condition: Reduce costs eller increase benefits, Action: Start smaller (pilot), reduce training costs, extend timeline, Re-evaluate: Efter cost reduction"
- "Scenario 3: Team Readiness < 70% → Condition: Build readiness først, Action: Scrum maturity program, training først, gradual introduction, Re-evaluate: Efter readiness improvement"
- "Dette viser at vi kan gå videre med conditions"

---

## SLIDE 24: No-Go Scenarier
**Speaking Notes:**
- "No-Go Scenarier:"
- "❌ NO-GO hvis:"
- "1. Kritiske risici ikke kan mitigere - RPN > 50 efter mitigation"
- "2. ROI < 50% - ikke værd at investere"
- "3. Team readiness < 50% - ikke klar til forandring"
- "4. Management support mangler - vil fejle uden support"
- "5. Culture mismatch - framework passer ikke til organisation"
- "Eksempel: Organisation har meget flat struktur, SAFe kræver hierarki → Culture mismatch, Decision: ❌ NO-GO → Vælg andet framework (fx Spotify model)"

---

## SLIDE 25: Beslutningsproces Flow
**Speaking Notes:**
- "Beslutningsproces Flow:"
- "Start: Risiko-analyse"
- "Step 1: Risk Assessment - beregn total RPN, kategoriser risici"
- "Step 2: Mitigation Assessment - evaluer mitigation effektivitet, beregn residual risk"
- "Step 3: Cost-Benefit Analysis - beregn costs, benefits, ROI, payback period"
- "Step 4: Decision Matrix - evaluer alle kriterier"
- "All criteria met? → YES: ✅ GO, NO: Conditional GO → Address gaps → Re-evaluate → ✅ GO eller ❌ NO-GO"
- "Dette er en struktureret beslutningsproces"

---

## SLIDE 26: Implementation Roadmap (Hvis GO)
**Speaking Notes:**
- "Implementation Roadmap (Hvis GO):"
- "Phase 1: Preparation (Måned 1-2) - Address critical risks (R1, R2), build team readiness, select framework, train champions"
- "Phase 2: Pilot (Måned 3-4) - Run pilot med 1-2 teams, measure overhead og productivity, gather feedback, adapt process"
- "Phase 3: Rollout (Måned 5-8) - Rollout til alle teams, continuous support, measure success metrics, optimize process"
- "Phase 4: Optimization (Måned 9-12) - Reduce overhead, improve efficiency, scale best practices, continuous improvement"
- "Dette er en phased approach - start small, scale gradually"

---

## SLIDE 27: Success Metrics
**Speaking Notes:**
- "Success Metrics:"
- "Track efter implementation:"
- "1. Productivity: Velocity før/efter - måler om produktivitet forbedres"
- "2. Coordination: Blocking issues reduktion - måler om koordinering forbedres"
- "3. Time-to-market: Feature delivery time - måler om delivery forbedres"
- "4. Team Satisfaction: Survey scores - måler om teams er tilfredse"
- "5. Process Overhead: Time spent i meetings - måler om overhead er acceptabel"
- "Go/No-Go Re-evaluation: Hvis metrics ikke forbedres efter 6 måneder → Re-evaluate, hvis overhead > 30% → Adapt process, hvis team satisfaction < 60% → Address concerns"

---

## SLIDE 28: Konklusion
**Speaking Notes:**
- "Konklusion:"
- "Risikoanalysen bruges til:"
- "1. Identificere risici før implementation - proaktiv risikostyring"
- "2. Evaluer mitigation muligheder - find løsninger til risici"
- "3. Beregn ROI og payback period - data-driven beslutning"
- "4. Træffe data-driven beslutning - go/no-go baseret på fakta"
- "5. Set conditions for successful implementation - address gaps først"
- "6. Track success efter implementation - kontinuerlig forbedring"
- "Decision Framework: ✅ GO - alle kriterier opfyldt → Proceed, ✅ Conditional GO - nogle kriterier mangler → Address først, ❌ NO-GO - kritiske risici ikke håndterbare → Don't proceed"

---

## SLIDE 29: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres opmærksomhed"
- "Jeg er klar til spørgsmål"

---

## EKSTRA NOTER TIL EKSAMEN:

### Hvis de spørger om specifikke filer:
- **`docs/FMEA-CICD-Risk-Analysis.md`**: Eksempel på FMEA-baseret risiko-analyse format
- **`docs/Spørgsmål-9-Organisatorisk-Skalering.md`**: Fuld risiko-analyse med alle detaljer

### Hvis de spørger om udfordringer:
- Communication Overhead: n(n-1)/2 kanaler
- Coordination Complexity: Information silos
- Knowledge Silos: Bus factor lav
- Process Overhead: Tunge processer
- Dependency Management: Blocking issues
- Culture Dilution: Sub-kulturer
- Decision Making Slowdown: Hierarkier
- Quality Consistency: Inkonsistent standarder
- Innovation vs. Stability: Innovation tax
- Resource Allocation: Resource contention

### Hvis de spørger om risiko-analyse:
- Format: FMEA-baseret (S, L, RPN)
- Top 5 risici: R1 (56), R2 (56), R3 (48), R4 (48), R5 (45)
- Mitigation: Alle risici har mitigation strategier
- Residual Risk: 2 (lav) efter mitigation

### Hvis de spørger om go/no-go:
- Step 1: Risk Assessment (Total RPN = 391)
- Step 2: Mitigation Assessment (Residual Risk = 20)
- Step 3: Cost-Benefit Analysis (ROI = 135%, Payback = 5 måneder)
- Step 4: Decision Matrix (Alle kriterier opfyldt → ✅ GO)
- Conditional GO: Address gaps først
- NO-GO: Kritiske risici ikke håndterbare, ROI < 50%, culture mismatch
