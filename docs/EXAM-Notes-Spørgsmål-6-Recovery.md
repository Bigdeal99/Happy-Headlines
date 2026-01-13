# EKSAMEN - Spørgsmål 6: Recovery
## Slide Outline & Speaking Notes (ca. 15 min)

---

## SLIDE 1: Titel
**Speaking Notes (kort):**
- "Jeg vil præsentere recovery i et distribueret system."
- "Fokus er på: 1) hvad feature flags giver i deployment, 2) hvordan vi kan lave rollback‑mekanismer i en CI/CD‑pipeline, og 3) forskellen på 'Design to be disabled' og 'Design for rollback'."
- "Jeg bruger Happy‑Headlines og vores CI/CD‑ og FMEA‑setup som eksempler."

---

## A: Feature flags i deployment‑processen

---

## SLIDE 2: Feature Flags – Hvad og Hvorfor?
**Speaking Notes:**
- "Feature flags er små toggles i koden, der bestemmer om en feature er aktiv – uden at vi skal deploye igen."
- "De giver os tre vigtige ting i deployment: gradual rollout, hurtig deaktivering og eksperimenter som A/B‑tests."
- "Det er et centralt recovery‑værktøj, fordi vi kan reagere på fejl i sekunder i stedet for minutter."

---

## SLIDE 3: Mulighed 1 – Gradual Rollout (Canary)
**Speaking Notes:**
- "Med feature flags kan vi lave canary‑deployments: kun en lille procentdel ser den nye funktion først."
- "Vi monitorerer fx error‑rate og latency."
- "Hvis alt er okay → skaler op til 100 %. Hvis ikke → sluk flagget igen."

---

## SLIDE 4: Mulighed 2 – Instant Rollback uden Redeploy
**Speaking Notes:**
- "Hvis en ny feature giver fejl, kan vi bare slukke flagget."
- "Det kræver ikke et nyt build eller deployment – det er ren konfiguration."
- "Det er især værdifuldt, hvis fejlen ligger i en ikke‑kritisk del, fx et nyt UI eller en optional funktion."

---

## SLIDE 5: Andre muligheder med Feature Flags
**Speaking Notes:**
- "Vi kan også bruge flags til A/B‑tests – to versioner af samme feature, styret af flags."
- "Vi kan have features kun tændt i staging, indtil vi er klar til production."
- "Og vi kan have en decideret kill‑switch til at slukke en hel feature, hvis noget går galt."

---

## SLIDE 6: Feature Flags vs. Deployment
**Speaking Notes:**
- "En vigtig pointe: feature flags separerer *deployment* fra *release*."
- "Koden kan godt være deployet, men feature er OFF."
- "Det betyder, at vi kan have stabil drift og stadig aktivere nye ting kontrolleret."

---

## B: Rollback‑mekanismer i CI/CD‑pipeline

---

## SLIDE 7: Overblik over Rollback‑strategi
**Speaking Notes:**
- "Vores overordnede rollback‑strategi har fire ben."
- "Først tagger vi alle images med SHA, så vi ved præcis, hvilken version vi ruller tilbage til."
- "Dernæst health‑checks og metrics, som kan trigge rollback automatisk."
- "Og når vi ændrer database, tager vi backup, så vi også kan rulle data tilbage."

---

## SLIDE 8: Version Tagging i CI/CD (Implementeret)
**Speaking Notes:**
- "I vores rigtige pipeline tagger vi alle images med både fuld og kort SHA."
- "Det kan man se i `Generate version tags`‑steppet i `.github/workflows/ci-cd.yml`."
- "Det betyder, at vi altid kan pege på 'den forrige gode version' når vi skal rulle tilbage."

**Screenshot‑tip:**
- Vis `.github/workflows/ci-cd.yml` omkring `Generate version tags` (linje 65–76).

---

## SLIDE 9: Health Check som Barrier (fra FMEA)
**Speaking Notes:**
- "I FMEA‑dokumentet har vi en konkret 'health check barrier'."
- "Pipelinen kalder `/health` op til 30 gange – hvis servicen aldrig bliver healthy, fejler deployment."
- "Det er fundamentet for automatisk rollback, fordi vi kan sige: 'bliver den ikke sund i tide, så ruller vi tilbage'."

---

## SLIDE 10: Eksempel – Automatisk Rollback på Health‑fejl
**Speaking Notes:**
- "I mine Recovery‑noter har jeg skitseret, hvordan vi kan koble health‑check og rollback sammen."
- "Hvis `Health check validation` sætter `healthy=false`, kalder vi `kubectl rollout undo`."
- "Det ruller deployment til forrige image – altså den version, der sidst kørte stabilt."

---

## SLIDE 11: Monitoring‑baseret Rollback (Prometheus)
**Speaking Notes:**
- "Health‑checks fanger typisk 'service er helt nede'."
- "Men vi kan også trigge rollback på kvalitet – fx hvis mere end 5 % af requests giver 5xx."
- "Det gør vi med en Prometheus‑query i pipelinen, og hvis den overskrider en tærskel, ruller vi tilbage."

---

## SLIDE 12: Database‑backup og Migration‑Rollback
**Speaking Notes:**
- "Kode rollback alene er ikke nok, hvis vi også har ændret database‑schema."
- "Derfor tager vi en backup af databasen før migration."
- "Hvis migrationen fejler, kan vi gendanne backup og rulle både kode og data tilbage."

---

## SLIDE 13: Samlet Rollback‑Flow
**Slide Content (diagram):**
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

**Speaking Notes:**
- "Her er det samlede flow."
- "Først deploy, så health‑check, så metrics."
- "Hvis noget fejler undervejs, ruller vi tilbage til den tidligere SHA‑version."

---

## C: Design to be disabled vs Design for rollback

---

## SLIDE 14: Sammenligning – Kort Tabel
**Speaking Notes:**
- "Nu sammenligner jeg de to principper."
- "Design to be disabled handler om features, Design for rollback handler om hele versioner."
- "De supplerer hinanden – de er ikke konkurrenter."

---

## SLIDE 15: Design to be disabled (Feature‑niveau)
**Speaking Notes:**
- "Design to be disabled betyder, at vi bevidst bygger features, så de kan slås fra."
- "Det er feature‑niveau: resten af systemet kører videre."
- "Vi bruger det ved fx A/B‑tests, gradvis udrulning og når vi hurtigt vil slå noget fra."

---

## SLIDE 16: Eksempelidé – Profanity‑check Feature
**Speaking Notes:**
- "Et naturligt sted for 'design to be disabled' hos os er profanity‑checket."
- "Vi har i forvejen en separat `ProfanityService` og en klient i `ProfanityClient.cs`."
- "Hvis vi sætter selve kaldet bag et flag, kan vi midlertidigt slukke for profanity‑check uden at stoppe kommentarfunktionen."
- "Det ville være et klassisk feature‑flag‑scenarie."

---

## SLIDE 17: Design for rollback (System‑niveau)
**Speaking Notes:**
- "Design for rollback tager det større perspektiv."
- "Her ruller vi hele deployment tilbage – ikke bare en enkelt feature."
- "Det bruger vi ved system‑wide fejl, DB‑problemer eller breaking API‑ændringer."

---

## SLIDE 18: Eksempel – Rollback i CI/CD
**Speaking Notes:**
- "I mine Recovery‑noter viser jeg, hvordan vores pipeline kan kalde `kubectl rollout undo`."
- "Det bruger SHA‑tags fra CI/CD‑pipelinen til at vælge forrige version."
- "Det er et konkret eksempel på 'Design for rollback' på system‑niveau."

---

## SLIDE 19: Kombineret Best Practice
**Speaking Notes:**
- "Best practice er at kombinere dem."
- "Vi deployer ny version med flags OFF, tænder dem forsigtigt, og har rollback som ekstra sikkerhedsnet."
- "På den måde får vi både hurtig lokal recovery og robusthed, hvis noget større går galt."

---

## SLIDE 20: Konklusion
**Speaking Notes:**
- "Feature flags hjælper os med hurtig, granulær recovery omkring enkelte features."
- "Rollback‑mekanismer i CI/CD giver os et sikkert net, når hele versionen er dårlig."
- "Design to be disabled handler om features; Design for rollback handler om systemet."
- "I praksis skal vi have begge principper i vores arkitektur."

---

## SLIDE 21: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres tid – jeg er klar til spørgsmål."

---

## Nøglereferencer til projektet

- `docs/Spørgsmål-6-Recovery.md` – detaljerede notes om feature flags, rollback og sammenligning
- `docs/FMEA-CICD-Risk-Analysis.md` – health‑check barrier og rollback‑relaterede risici
- `.github/workflows/ci-cd.yml` – faktisk CI/CD‑pipeline med build + tagging af images
- `CommentService/Services/ProfanityClient.cs` – eksempel på komponent hvor feature‑flags kunne give granular recovery

