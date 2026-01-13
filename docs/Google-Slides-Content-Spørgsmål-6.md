# Google Slides Content - Spørgsmål 6: Recovery
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Recovery i CI/CD og microservices  
**Undertitel:** Feature flags, rollback og recovery patterns  
**Footer:** Dit navn | Dato

---

## SLIDE 2: Feature Flags – Hvad og Hvorfor?
**Titel:** Feature Flags – Hvad og hvorfor?

**Indhold:**
- **Definition:** Slå funktionalitet til/fra i runtime uden redeploy
- **Formål:** Styre risiko ved nye features i production
- **Brug:** Gradual rollout, hurtig deaktivering, A/B‑tests, user‑segmenter

---

## SLIDE 3: Mulighed 1 – Gradual Rollout (Canary)
**Titel:** Gradual Rollout (Canary)

**Indhold:**
- Aktivér feature for en lille procentdel brugere (fx 5–10 %)
- Overvåg metrics (latency, fejlrate, brugerfeedback)
- Skaler gradvist op til 100 %, hvis alt ser godt ud
- Hvis problemer: sluk flag → instant recovery

---

## SLIDE 4: Mulighed 2 – Instant Rollback uden Redeploy
**Titel:** Instant rollback uden redeploy

**Indhold:**
- Sluk feature med ét flag i stedet for at redeploye
- Recovery‑tid: sekunder i stedet for minutter
- Påvirker kun den ene feature, resten af systemet kører videre
- Særligt nyttigt for ikke‑kritiske, nye features

---

## SLIDE 5: Andre Muligheder med Feature Flags
**Titel:** Andre muligheder med feature flags

**Indhold:**
- **A/B‑testing:** To implementationer bag hvert sit flag → data‑drevet valg
- **Environment‑specifik:** Aktivér kun i staging først, derefter production
- **User‑segmenter:** Kun ON for beta/premium brugere
- **Emergency kill switch:** Sluk risikabel feature ved kritiske fejl

---

## SLIDE 6: Feature Flags vs. Deployment
**Titel:** Feature flags vs. deployment

**Indhold:**
- **Deployment:** Koden lægges ud
- **Release:** Feature flag tændes
- Separation:
  - Deploy ofte
  - Release kontrolleret med feature flags

---

## SLIDE 7: Rollback‑strategi i CI/CD
**Titel:** Rollback‑strategi i CI/CD

**Indhold:**
- **Principper (fra docs/Spørgsmål-6-Recovery.md):**
  1. Version tagging (SHA) → nem rollback
  2. Health checks → auto‑rollback ved fejl
  3. Monitoring (Prometheus) → rollback på høj error‑rate
  4. Database‑backup før migration → DB‑rollback muligt

---

## SLIDE 8: Version Tagging i Pipeline (Implementeret)
**Titel:** Version tagging i CI/CD‑pipeline

**Indhold:**
- **Fil:** `.github/workflows/ci-cd.yml`
- Images tagges med:
  - Fuldt SHA‑tag (`:${{ github.sha }}`)
  - Kort SHA‑tag (første 7 tegn)
- Gør det muligt at pege tilbage på “seneste kendt gode” version

**Screenshot:**  
Vis `Generate version tags`‑steppet (ca. linje 65–76) i `.github/workflows/ci-cd.yml`.

---

## SLIDE 9: Health Check Barrier (fra FMEA)
**Titel:** Health check som barrier

**Indhold:**
- **Fil:** `docs/FMEA-CICD-Risk-Analysis.md`
- Step: "Wait for Service Health"
- Loop kalder `/health` op til fx 30 gange
- Hvis service ikke bliver healthy → deployment fejler

**Pointe:**  
Health check er en **barriere**, som forhindrer en dårlig release i at blive accepteret.

---

## SLIDE 10: Automatisk Rollback på Health‑fejl (Konceptuelt)
**Titel:** Automatisk rollback på health‑fejl

**Indhold:**
- Idé fra `docs/Spørgsmål-6-Recovery.md`:
  - Step 1: `Health check validation`
  - Step 2: `Rollback on health check failure`
- Handling:
  - Hvis `healthy == false` → `kubectl rollout undo deployment/...`

**Budskab:**  
Health‑check og version tagging kombineres til automatisk rollback.

---

## SLIDE 11: Monitoring‑baseret Rollback (Prometheus)
**Titel:** Rollback baseret på error‑rate

**Indhold:**
- Query (fra Recovery‑noten):
  - `rate(http_requests_total{status=~"5.."}[5m])`
- Tærskel: fx 5 % 5xx‑fejl
- Hvis error‑rate > 5 %:
  - Sæt `error_rate_high=true`
  - Trig rollback (fx `rollout undo`)

---

## SLIDE 12: Database‑Backup og Migration‑Rollback
**Titel:** Database‑backup og rollback

**Indhold:**
- Før migration:
  - Tag backup af databasen
- Kør migration
- Ved fejl:
  - Restore backup
- Formål:
  - Beskytte mod data‑korruption ved mislykket migration

---

## SLIDE 13: Samlet Rollback‑Flow
**Titel:** Samlet rollback‑flow

**Indhold (tekstdiagram):**
```text
Deploy ny version
    ↓
Vent på deployment
    ↓
Health check OK?
    ├─ Nej → Rollback (undo deployment)
    └─ Ja  → Monitorér error-rate (Prometheus)
                 ↓
          Error < 5 %?
            ├─ Nej → Rollback
            └─ Ja  → Deployment godkendt
```

---

## SLIDE 14: Sammenligning – Design to be disabled vs Design for rollback
**Titel:** Sammenligning af principper

**Indhold (tabel):**
| Aspekt        | Design to be disabled (feature flags) | Design for rollback (deployment) |
|---------------|----------------------------------------|-----------------------------------|
| Formål        | Sluk funktionalitet i runtime         | Gå tilbage til tidligere version |
| Granularitet  | Feature‑niveau                        | System/version‑niveau            |
| Hastighed     | Sekunder (ingen redeploy)             | Minutter (ny rollout)            |
| Scope         | Én feature                            | Hele deployment                  |
| Data‑impact   | Ingen (feature OFF)                   | Kan påvirke data (migration)     |

---

## SLIDE 15: Design to be disabled – Hvornår?
**Titel:** Design to be disabled – anvendelse

**Indhold:**
- **Typiske scenarier:**
  - En specifik feature fejler
  - A/B‑testing af implementationer
  - Gradvis udrulning (canary)
  - Emergency kill switch for risikabel feature
- **Nøgleide:**  
  Byg features, så de kan slås FRA uden at stoppe resten af systemet.

---

## SLIDE 16: Eksempelidé – Profanity‑check Feature
**Titel:** Eksempelidé: Profanity‑check

**Indhold:**
- **Fil:** `CommentService/Services/ProfanityClient.cs`
- Idé:
  - Profanity‑check er en separat service
  - Kunne ligge bag et feature flag
  - Hvis `ProfanityService` er ustabil:
    - Sluk flag → kommentarfunktionen fortsætter uden check

**Note:**  
Her bruges en eksisterende komponent som et naturligt sted at anvende *design to be disabled*.

---

## SLIDE 17: Design for rollback – Hvornår?
**Titel:** Design for rollback – anvendelse

**Indhold:**
- **Typiske scenarier:**
  - System‑wide fejl (hele versionen ustabil)
  - Fejlede database migrationer
  - Breaking API‑ændringer
  - Konfigurations‑/infrastruktur‑fejl
- **Nøgleide:**  
  Kunne rulle hele systemet tilbage til “senest kendt god” version.

---

## SLIDE 18: Eksempel – Rollback i CI/CD
**Titel:** Eksempel: Rollback i pipeline

**Indhold:**
- **Filer:**  
  - `.github/workflows/ci-cd.yml` (version tagging)  
  - `docs/Spørgsmål-6-Recovery.md` (skitse af rollback steps)
- **Mønster:**
  - Images tagges med SHA
  - Health‑check + metrics som gate
  - Ved fejl: `kubectl rollout undo` til forrige version

---

## SLIDE 19: Kombineret Best Practice
**Titel:** Kombineret best practice

**Indhold:**
- **Brug begge principper:**
  - Feature flags → hurtig, granular recovery på feature‑niveau
  - Rollback → sikkerhed ved system‑wide fejl
- **Typisk flow:**
  1. Deploy ny version med nye features OFF (flags)
  2. Tænd flag for fx 10 % trafik
  3. Ved fejl:
     - Sluk feature flag (hurtigt)
     - ELLER rollback til tidligere version (hvis problemet er større)

---

## SLIDE 20: Konklusion
**Titel:** Konklusion

**Indhold:**
- Feature flags:
  - Separerer deployment og release
  - Giver gradual rollout, A/B‑tests og kill‑switch
- Rollback i CI/CD:
  - Version tagging, health‑checks, metrics og DB‑backup
- Design to be disabled vs Design for rollback:
  - Feature‑niveau vs system‑niveau
  - Bør bruges sammen for robust recovery

---

## SLIDE 21: Spørgsmål?
**Titel:** Spørgsmål?

**Indhold:**
- Dit navn
- Evt. kontaktinfo

---

## INSTRUKTIONER TIL GOOGLE SLIDES

1. **Opret ny Google Slides præsentation.**
2. **For hver slide:**
   - Brug titlen her som slide‑titel.
   - Kopier bullet‑indholdet ind som tekst.
3. **Screenshots og referencer:**
   - `.github/workflows/ci-cd.yml`:  
     - Vis `Generate version tags`‑steppet og matrix‑strategien.
   - `docs/FMEA-CICD-Risk-Analysis.md`:  
     - Vis `Wait for Service Health`‑steppet.
   - `CommentService/Services/ProfanityClient.cs`:  
     - Vis hvordan profanity‑klienten er isoleret som en separat service.
4. **Design‑tips:**
   - Brug konsistent farveskema og store, læsbare fonte.
   - Highlight nøgleord (Feature flags, Rollback, Health check, Error‑rate).
   - Hold maks. 3–5 bullets per slide for at passe til 15 minutters præsentation.

