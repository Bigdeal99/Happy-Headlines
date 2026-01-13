# Google Slides Content - Spørgsmål 9: Organisatorisk Skalering
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Organisatorisk Skalering
**Undertitel:** Udfordringer ved team vækst og framework transition
**Footer:** Dit navn | Dato

---

## SLIDE 2: Udfordring 1 - Communication Overhead
**Titel:** Udfordring 1 - Communication Overhead

**Indhold:**
- **Lille gruppe (5-10 personer):** Informel kommunikation, alle kender alle, hurtige beslutninger
- **Stor organisation (50+ personer):** Formel kommunikation, kommunikationskanaler vokser eksponentielt
- **Eksempel:** 5 personer = 10 kanaler, 50 personer = 1,225 kanaler

---

## SLIDE 3: Udfordring 2 - Coordination Complexity
**Titel:** Udfordring 2 - Coordination Complexity

**Indhold:**
- **Lille gruppe:** Simpel koordinering (face-to-face), alle ved hvad andre laver
- **Stor organisation:** Kompleks koordinering (mellem teams, afdelinger), information silos
- **Eksempel fra microservices:** Teams ejer forskellige services → Koordinering nødvendig for cross-service changes

---

## SLIDE 4: Udfordring 3 - Knowledge Silos
**Titel:** Udfordring 3 - Knowledge Silos

**Indhold:**
- **Lille gruppe:** Delte viden (pair programming, code reviews), bus factor høj
- **Stor organisation:** Viden koncentreret i få personer, bus factor lav, onboarding tager længere tid
- **Eksempel:** Kun ArticleService team kender ArticleService → Knowledge silo

---

## SLIDE 5: Udfordring 4 - Process Overhead
**Titel:** Udfordring 4 - Process Overhead

**Indhold:**
- **Lille gruppe:** Letvægts processer (Scrum, Kanban), hurtig iteration, fleksibel
- **Stor organisation:** Tunge processer (SAFe, LeSS), langsommere iteration, mindre fleksibel
- **Problem:** Process overhead kan overstige værdien, innovation kan blive hæmmet

---

## SLIDE 6: Udfordring 5 - Dependency Management
**Titel:** Udfordring 5 - Dependency Management

**Indhold:**
- **Lille gruppe:** Få dependencies mellem teams, let at koordinere
- **Stor organisation:** Mange dependencies mellem teams, kompleks dependency graph, blocking issues
- **Eksempel fra Happy-Headlines:** PublisherService → ProfanityService, CommentService → ProfanityService

---

## SLIDE 7: Udfordring 6 - Culture Dilution
**Titel:** Udfordring 6 - Culture Dilution

**Indhold:**
- **Lille gruppe:** Stærk kultur (samme værdier, normer), hurtig kultur-spredning
- **Stor organisation:** Kultur bliver fortyndet, sub-kulturer opstår (per team/afdeling), kultur-spredning tager længere tid

---

## SLIDE 8: Udfordring 7 - Decision Making Slowdown
**Titel:** Udfordring 7 - Decision Making Slowdown

**Indhold:**
- **Lille gruppe:** Hurtige beslutninger (få personer involveret), konsensus let at opnå
- **Stor organisation:** Langsomme beslutninger (mange stakeholders), konsensus svær at opnå, hierarkier og approval gates

---

## SLIDE 9: Udfordring 8 - Quality Consistency
**Titel:** Udfordring 8 - Quality Consistency

**Indhold:**
- **Lille gruppe:** Konsistent kvalitet (samme standarder), code reviews let at koordinere
- **Stor organisation:** Inkonsistent kvalitet (forskellige standarder per team), code reviews komplekse (cross-team), standards svære at håndhæve

---

## SLIDE 10: Udfordring 9 - Innovation vs. Stability
**Titel:** Udfordring 9 - Innovation vs. Stability

**Indhold:**
- **Lille gruppe:** Innovation let (hurtig iteration, eksperimentering), risiko acceptabel
- **Stor organisation:** Innovation svær (processer, approval gates), risiko-avers (stabilitet vigtigere), "Innovation tax" høj

---

## SLIDE 11: Udfordring 10 - Resource Allocation
**Titel:** Udfordring 10 - Resource Allocation

**Indhold:**
- **Lille gruppe:** Let at allokere ressourcer (få personer), prioritering simpel
- **Stor organisation:** Kompleks resource allocation (mange teams, projekter), prioritering kræver formelle processer, resource contention

---

## SLIDE 12: Konkrete Eksempler fra Happy-Headlines
**Titel:** Konkrete Eksempler fra Happy-Headlines

**Indhold:**
- **Happy-Headlines har 6 services:** ArticleService, CommentService, ProfanityService, DraftService, PublisherService, NewsletterService
- **Lille team (5 personer):** Alle kan arbejde på alle services, ingen koordinering nødvendig, hurtig iteration
- **Stor organisation (30+ personer):** 6 teams (1 per service) → Koordinering nødvendig, cross-service changes kræver multiple teams, slower iteration

---

## SLIDE 13: Risiko-analyse Format (FMEA-baseret)
**Titel:** Risiko-analyse Format (FMEA-baseret)

**Indhold:**
- **Format:** ID, Risiko, Svarighed (S), Sandsynlighed (L), RPN (S×L), Mitigation, Prioritet
- **Eksempel fra FMEA:** `docs/FMEA-CICD-Risk-Analysis.md` bruger samme format
- **Skala:** S og L fra 1-10, RPN fra 1-100

---

## SLIDE 14: Top 5 Risici - Oversigt
**Titel:** Top 5 Risici - Oversigt

**Indhold:**
| ID | Risiko | S | L | RPN | Prioritet |
|----|--------|---|---|-----|-----------|
| R1 | Resistance to Change | 8 | 7 | 56 | Kritisk |
| R2 | Process Overhead | 7 | 8 | 56 | Kritisk |
| R3 | Loss of Team Autonomy | 8 | 6 | 48 | Høj |
| R4 | Training Costs | 6 | 8 | 48 | Høj |
| R5 | Productivity Drop | 9 | 5 | 45 | Høj |

---

## SLIDE 15: R1 - Resistance to Change (Kritisk)
**Titel:** R1 - Resistance to Change (Kritisk)

**Indhold:**
- **Svarighed:** 8 (kan stoppe hele transitionen, kan skabe toxic kultur)
- **Sandsynlighed:** 7 (høj - mange teams modsætter sig typisk)
- **RPN:** 56 (Kritisk)
- **Mitigation:** Change management, training, communication
- **Residual Risk:** 2 (Lav efter mitigation)

---

## SLIDE 16: R2 - Process Overhead (Kritisk)
**Titel:** R2 - Process Overhead (Kritisk)

**Indhold:**
- **Svarighed:** 7 (reducerer produktivitet, kan skabe frustration)
- **Sandsynlighed:** 8 (meget høj - skaleret frameworks har typisk mere overhead)
- **RPN:** 56 (Kritisk)
- **Mitigation:** Start small, adapt process, measure overhead
- **Residual Risk:** 2 (Lav efter mitigation)

---

## SLIDE 17: R3 - Loss of Team Autonomy (Høj)
**Titel:** R3 - Loss of Team Autonomy (Høj)

**Indhold:**
- **Svarighed:** 8 (reducerer motivation, kan føre til talent loss)
- **Sandsynlighed:** 6 (medium-høj - afhænger af framework implementation)
- **RPN:** 48 (Høj)
- **Mitigation:** Preserve team autonomy, empower teams, framework selection
- **Residual Risk:** 2 (Lav efter mitigation)

---

## SLIDE 18: Framework-specifikke Risici
**Titel:** Framework-specifikke Risici

**Indhold:**
- **SAFe:** Top-down Control (RPN: 56), PI Planning Overhead (RPN: 56), Role Complexity (RPN: 42)
- **LeSS:** Requires Scrum Maturity (RPN: 42), Coordination Complexity (RPN: 42), Feature Team Transition (RPN: 35)
- **Spotify:** Requires Strong Culture (RPN: 48), Informal Structure (RPN: 30), Scaling Challenges (RPN: 28)

---

## SLIDE 19: Step 1 - Risk Assessment
**Titel:** Step 1 - Risk Assessment

**Indhold:**
- **Total RPN:** 56 + 56 + 48 + 48 + 45 + 35 + 35 + 30 + 24 + 20 = 391
- **Kategoriser Risici:**
  - Kritiske (RPN ≥ 50): R1, R2 → 2 risici
  - Høje (RPN 30-49): R3, R4, R5, R6, R7 → 5 risici
  - Medium (RPN 20-29): R8, R9 → 2 risici
  - Lave (RPN < 20): R10 → 1 risiko

---

## SLIDE 20: Step 2 - Mitigation Assessment
**Titel:** Step 2 - Mitigation Assessment

**Indhold:**
- **Kritiske risici:** Alle har mitigation → Residual risk: 2 (lav)
- **Høje risici:** Alle har mitigation → Residual risk: 2 (lav)
- **Total Residual Risk:** 10 × 2 = 20 (meget lav)
- **Mitigation Cost:** Training 100,000 DKK + Consultants 200,000 DKK + Tooling 50,000 DKK = 350,000 DKK

---

## SLIDE 21: Step 3 - Cost-Benefit Analysis
**Titel:** Step 3 - Cost-Benefit Analysis

**Indhold:**
- **Costs:** Direct Costs 350,000 DKK + Productivity Loss 500,000 DKK = 850,000 DKK
- **Benefits:** Better Coordination 30% reduktion, Faster Delivery 20% forbedring, Scalability 2x team growth → Estimated Value 2,000,000 DKK/år
- **ROI:** (2,000,000 - 850,000) / 850,000 × 100% = 135%
- **Payback Period:** 850,000 / 2,000,000 = 0.425 år (~5 måneder)

---

## SLIDE 22: Step 4 - Go/No-Go Decision Matrix
**Titel:** Step 4 - Go/No-Go Decision Matrix

**Indhold:**
| Kriterium | Threshold | Actual | Status |
|-----------|-----------|--------|--------|
| Kritiske Risici | ≤ 3 | 2 | ✅ PASS |
| Residual Risk | ≤ 30 | 20 | ✅ PASS |
| ROI | ≥ 100% | 135% | ✅ PASS |
| Payback Period | ≤ 12 måneder | 5 måneder | ✅ PASS |
| Team Readiness | ≥ 70% | 75% | ✅ PASS |
| Management Support | Ja | Ja | ✅ PASS |
- **Decision:** ✅ GO (Alle kriterier opfyldt)

---

## SLIDE 23: Conditional Go Decision
**Titel:** Conditional Go Decision

**Indhold:**
- **Scenario 1:** Høj Resistance (R1 ikke mitigerede) → Condition: Address resistance først, Action: 3 måneders change management program
- **Scenario 2:** Lav ROI (< 100%) → Condition: Reduce costs eller increase benefits, Action: Start smaller (pilot), reduce training costs
- **Scenario 3:** Team Readiness < 70% → Condition: Build readiness først, Action: Scrum maturity program, training først

---

## SLIDE 24: No-Go Scenarier
**Titel:** No-Go Scenarier

**Indhold:**
- **❌ NO-GO hvis:**
  1. Kritiske risici ikke kan mitigere (RPN > 50 efter mitigation)
  2. ROI < 50% (ikke værd at investere)
  3. Team readiness < 50% (ikke klar til forandring)
  4. Management support mangler (vil fejle uden support)
  5. Culture mismatch (framework passer ikke til organisation)

---

## SLIDE 25: Beslutningsproces Flow
**Titel:** Beslutningsproces Flow

**Indhold:**
```
Start: Risiko-analyse
    ↓
Step 1: Risk Assessment
    ↓
Step 2: Mitigation Assessment
    ↓
Step 3: Cost-Benefit Analysis
    ↓
Step 4: Decision Matrix
    ↓
All criteria met?
    ↓
YES → ✅ GO
NO → Conditional GO → Address gaps → Re-evaluate → ✅ GO / ❌ NO-GO
```

---

## SLIDE 26: Implementation Roadmap (Hvis GO)
**Titel:** Implementation Roadmap (Hvis GO)

**Indhold:**
- **Phase 1: Preparation (Måned 1-2)** - Address critical risks, build team readiness, select framework, train champions
- **Phase 2: Pilot (Måned 3-4)** - Run pilot med 1-2 teams, measure overhead, gather feedback, adapt process
- **Phase 3: Rollout (Måned 5-8)** - Rollout til alle teams, continuous support, measure success metrics, optimize process
- **Phase 4: Optimization (Måned 9-12)** - Reduce overhead, improve efficiency, scale best practices, continuous improvement

---

## SLIDE 27: Success Metrics
**Titel:** Success Metrics

**Indhold:**
- **Track efter implementation:**
  1. Productivity: Velocity før/efter
  2. Coordination: Blocking issues reduktion
  3. Time-to-market: Feature delivery time
  4. Team Satisfaction: Survey scores
  5. Process Overhead: Time spent i meetings
- **Go/No-Go Re-evaluation:** Hvis metrics ikke forbedres efter 6 måneder → Re-evaluate

---

## SLIDE 28: Konklusion
**Titel:** Konklusion

**Indhold:**
- **Risikoanalysen bruges til:**
  1. Identificere risici før implementation
  2. Evaluer mitigation muligheder
  3. Beregn ROI og payback period
  4. Træffe data-driven beslutning (go/no-go)
  5. Set conditions for successful implementation
  6. Track success efter implementation
- **Decision Framework:** ✅ GO (alle kriterier opfyldt), ✅ Conditional GO (nogle kriterier mangler), ❌ NO-GO (kritiske risici ikke håndterbare)

---

## SLIDE 29: Spørgsmål?
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
4. **Flow diagram:**
   - Slide 25: Tegn flow diagram eller brug tekst
5. **Tal og beregninger:**
   - Slide 19-21: Vis beregninger tydeligt
   - Brug monospace font for formler
