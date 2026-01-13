# EKSAMEN - Spørgsmål 3: Skaleringskuben
## Slide Outline & Speaking Notes

---

## SLIDE 1: Titel
**Speaking Notes:**
- "Jeg vil i dag præsentere skaleringskuben"
- "Jeg vil dække tre dele: headroom-beregninger, kombination af Y+Z akser, og kompromiser ved Z-akse skalering"
- "Fokus er på kapacitetsplanlægning og skalering i microservices"

---

## PART A: BESKRIV HEADROOM-BEREGNINGER

---

## SLIDE 2: Hvad er Headroom?
**Speaking Notes:**
- "Første del: Headroom-beregninger"
- "Headroom er den ekstra kapacitet man har til rådighed ud over den nuværende belastning"
- "Det er en buffer der giver plads til trafik-tilvækst, trafik-spidsbelastninger, fejl i systemet, og planlagt vedligeholdelse"
- "Formlen er: (Tilgængelig Kapacitet - Nuværende Belastning) / Nuværende Belastning × 100%"

---

## SLIDE 3: Hvordan Headroom Hjælper - Proaktiv Skalering
**Speaking Notes:**
- "Headroom hjælper med proaktive skaleringsbeslutninger"
- "Når headroom falder under en tærskel, fx 20%, skal man skale op"
- "Dette giver tid til at planlægge skalering før systemet når kapacitet"
- "Forhindrer reaktive nødskaleringssituationer"

---

## SLIDE 4: Hvordan Headroom Hjælper - Resource Allocation
**Speaking Notes:**
- "Headroom hjælper med resource allocation"
- "Hjælper med at identificere hvilke komponenter der skal skaleres først"
- "Prioriterer investeringer i infrastruktur"
- "Optimerer cost vs. performance balance"
- "Eksempel: Hvis ArticleService memory headroom er 43% (lavest), fokuserer vi på at skale memory først"

---

## SLIDE 5: Headroom Metrics og Tærskler
**Speaking Notes:**
- "Headroom metrics:"
- "CPU utilization, Memory utilization, Database connections, Network bandwidth, Request throughput"
- "Tærskler:"
- "Kritisk: Under 10% headroom - skal skale umiddelbart"
- "Advarsel: 10-20% headroom - planlæg skalering"
- "Normal: 20-50% headroom - acceptabelt niveau"
- "Komfortabel: Over 50% headroom - god buffer"

---

## SLIDE 6: Eksempel - Headroom Beregning
**Speaking Notes:**
- "Eksempel på headroom-beregning:"
- "Scenario: ArticleService"
- "Nuværende belastning: 200 requests per sekund, CPU 60%, Memory 70%, Database connections 40 ud af 100"
- "Maksimal kapacitet: 500 requests per sekund, CPU 100%, Memory 100%, Database connections 100"
- "Headroom beregning:"
- "Throughput: (500-200)/200 = 150% headroom"
- "CPU: (100%-60%)/60% = 67% headroom"
- "Memory: (100%-70%)/70% = 43% headroom"
- "Database connections: (100-40)/40 = 150% headroom"
- "Bottleneck: Memory med 43% headroom - lavest"

---

## SLIDE 7: Kapacitetsplanlægning med Headroom
**Speaking Notes:**
- "Kapacitetsplanlægning:"
- "Memory headroom er 43% - acceptabelt, men vi overvåger det"
- "Hvis trafik stiger 50%: fra 200 til 300 requests per sekund"
- "Memory vil stige til omkring 85% - det giver kun 15% headroom, hvilket er kritisk"
- "Beslutning: Vi skal skale op før trafik stiger"
- "Dette er proaktiv skalering baseret på headroom"

---

## PART B: DEMONSTRER KOMBINATION AF TO AKSER

---

## SLIDE 8: Skaleringskubens Tre Akser
**Speaking Notes:**
- "Nu viser jeg kombination af to akser"
- "Skaleringskuben har tre akser:"
- "X-akse: Horizontal Duplication - klon samme service flere gange"
- "Y-akse: Functional Decomposition - split efter funktionalitet (microservices)"
- "Z-akse: Data Partitioning - split efter data (sharding, geografisk)"
- "I dag fokuserer jeg på kombination af Y-akse og Z-akse"

---

## SLIDE 9: Problemstilling
**Speaking Notes:**
- "Problemstilling:"
- "Happy-Headlines oplever performance-problemer"
- "ArticleService kan kun håndtere 500 requests per sekund"
- "Trafik stiger til 2000 requests per sekund - det er en 4x stigning"
- "Database er bottleneck - kun 100 connections maksimum"
- "Geografisk spredt trafik - Europa, Asien, Nord-Amerika"
- "Vi skal løse dette med kombination af Y-akse og Z-akse skalering"

---

## SLIDE 10: Del 1 - Y-akse Skalering (Microservices)
**Speaking Notes:**
- "Første del: Y-akse Skalering"
- "Vi splitter monolitisk system til microservices"
- "I ArticlesController ser vi isoleret service for article management"
- "I CommentsController ser vi separeret service for comment management"
- "Arkitektur: Fra monolitisk system til 6 microservices"
- "Fordele: Hver service kan skaleres uafhængigt"

**Screenshot Instructions:**
1. Åbn `ArticleService/Controllers/ArticlesController.cs`
2. Vis at det er isoleret service
3. Åbn `CommentService/Controllers/CommentsController.cs`
4. Vis at det er separeret service

---

## SLIDE 11: Del 2 - Z-akse Skalering (Geografisk Partitioning)
**Speaking Notes:**
- "Anden del: Z-akse Skalering"
- "Vi splitter database efter continent"
- "I docker-compose.yml ser vi 8 databases: global plus 7 kontinenter"
- "I Article.cs ser vi Continent property - dette er Z-axis split key"
- "Arkitektur: Fra 1 database til 8 databases"
- "Fordele: Geografisk nærhed reducerer latency, trafik fordelt"

**Screenshot Instructions:**
1. Åbn `docker-compose.yml`
2. Marker linje 20-69 (alle 8 databases)
3. Åbn `ArticleService/Models/Article.cs`
4. Marker linje 8 (Continent property)

---

## SLIDE 12: Kombineret Y + Z-akse Løsning
**Speaking Notes:**
- "Kombineret løsning:"
- "Før: Monolitisk app går til én database"
- "2000 requests per sekund, database bottleneck, 100 connections overbelastet"
- "Efter: ArticleService (Y-akse) router til 8 continent databases (Z-akse)"
- "2000 requests per sekund fordelt til ~250 requests per sekund per database"
- "Hver database kan håndtere sin del"

---

## SLIDE 13: Performance Forbedring - Y-akse
**Speaking Notes:**
- "Performance forbedring - Y-akse:"
- "ArticleService kan nu håndtere 2000 requests per sekund - 4x forbedring"
- "CommentService kan skaleres uafhængigt - hvis comment trafik stiger, påvirker det ikke ArticleService"
- "ProfanityService er isoleret fra article trafik"
- "Fordel: Independent scaling - hver service skaleres uafhængigt"

---

## SLIDE 14: Performance Forbedring - Z-akse
**Speaking Notes:**
- "Performance forbedring - Z-akse:"
- "Database connections: Fra 100 til 800 (8 databases × 100 connections hver)"
- "Trafik fordelt: 2000 requests per sekund til ~250 requests per sekund per database"
- "Geografisk latency: Fra 500ms til 50ms - lokale databases reducerer latency"
- "Database size: Fra 1TB til ~125GB per database - 8x mindre data per database"
- "Mindre data = hurtigere queries"

---

## SLIDE 15: Kombineret Resultat
**Speaking Notes:**
- "Kombineret resultat:"
- "Max throughput: Fra 500 til 2000 requests per sekund - 4x forbedring"
- "Database connections: Fra 100 til 800 - 8x forbedring"
- "Response time: Fra 500ms til 50ms - 10x forbedring"
- "Database size: Fra 1TB til 125GB per database - 8x mindre"
- "Geografisk latency: Fra høj til lav - 10x bedre"
- "Dette løser vores skaleringsproblem"

---

## PART C: VURDER KOMPROMISER VED Z-AKSE SKALERING

---

## SLIDE 16: Kompromis 1 - Data Consistency
**Speaking Notes:**
- "Nu diskuterer jeg kompromiser ved Z-akse skalering"
- "Første kompromis: Data Consistency Kompleksitet"
- "Monolitiske systemer har typisk ACID-transaktioner på tværs af alle data"
- "Z-akse split betyder data er på tværs af flere databases"
- "Før: ACID transaktioner på tværs af alle data"
- "Efter: Transaktioner kun inden for samme shard"
- "Konsekvens: Cross-shard transaktioner er komplekse eller umulige"

---

## SLIDE 17: Kompromis 2 - Query Kompleksitet
**Speaking Notes:**
- "Andet kompromis: Query Kompleksitet"
- "Queries der spænder over flere shards bliver komplekse"
- "Før: Simpel query - SELECT * FROM Articles WHERE PublishedAt > '2024-01-01'"
- "Efter: Skal query alle 8 databases og merge resultater"
- "Trade-off: Hver database query er hurtigere (mindre data), men cross-shard queries er langsommere (8 queries + merge)"
- "Mere kompleks kode"

---

## SLIDE 18: Kompromis 3 - Shard Routing
**Speaking Notes:**
- "Tredje kompromis: Shard Routing Kompleksitet"
- "Systemet skal vide hvilken shard data skal til/fra"
- "Før: Ingen routing - alt går til én database"
- "Efter: Routing logic nødvendig baseret på continent"
- "Trade-off: Data er geografisk tæt på brugere, men routing logic tilføjer kompleksitet"
- "Fejl i routing kan føre til data i forkert shard"

---

## SLIDE 19: Kompromis 4 - Data Migration
**Speaking Notes:**
- "Fjerde kompromis: Rebalancing og Data Migration"
- "Hvis data-distributionen ændrer sig, skal data flyttes mellem shards"
- "Før: Ingen rebalancing nødvendig"
- "Efter: Hvis et kontinent vokser, skal data muligvis flyttes"
- "Eksempel: Hvis Europa har 80% af artikler, bliver article-db-europe overbelastet"
- "Skal flytte nogle artikler til article-db-global eller oprette article-db-europe-2"
- "Migration er kompleks og risikabel"

---

## SLIDE 20: Kompromis 5 - Backup og Recovery
**Speaking Notes:**
- "Femte kompromis: Backup og Recovery Kompleksitet"
- "Backup og recovery skal håndtere flere databases"
- "Før: 1 backup, 1 recovery procedure"
- "Efter: 8 backups, koordineret recovery"
- "Trade-off: Hvis én shard fejler, påvirker det kun den shard"
- "Men backup/restore er 8x mere komplekst"
- "Point-in-time recovery kræver koordinering af alle shards"

---

## SLIDE 21: Kompromis 6 - Monitoring og Development
**Speaking Notes:**
- "Sjette kompromis: Monitoring og Development Kompleksitet"
- "Monitoring skal dække flere databases"
- "Før: 1 database at overvåge"
- "Efter: 8 databases at overvåge"
- "Development: Fra 1 database til 8 databases i docker-compose"
- "Trade-off: Bedre isolation, men mere komplekst setup"
- "Mere memory og disk space brugt"

---

## SLIDE 22: Kompromis 7 - Cost Overhead
**Speaking Notes:**
- "Syvende kompromis: Cost Overhead"
- "Flere databases = højere omkostninger"
- "Før: 1 database server"
- "Efter: 8 database servers"
- "Trade-off: Bedre performance og skalering, men 8x database server costs"
- "Mere kompleks cost management"

---

## SLIDE 23: Kompromis Summary
**Speaking Notes:**
- "Kompromis summary:"
- "Positive kompromiser:"
- "8x bedre skalering (8 databases)"
- "10x bedre geografisk latency"
- "Bedre isolation (fejl i én shard påvirker ikke andre)"
- "Mindre data per database = hurtigere queries"
- "Negative kompromiser:"
- "Mere kompleks data consistency"
- "Cross-shard queries er langsomme"
- "Routing logic kompleksitet"
- "Data migration kompleksitet"
- "Backup/recovery kompleksitet"
- "Monitoring kompleksitet"
- "Development setup kompleksitet"
- "Højere costs (8x databases)"

---

## SLIDE 24: Når Brug Z-akse Skalering?
**Speaking Notes:**
- "Når brug Z-akse skalering?"
- "Brug det når geografisk spredt trafik"
- "Når data kan naturligt partitioneres, fx efter continent"
- "Når cross-shard queries er sjældne"
- "Når team har erfaring med distributed systems"
- "Overvej X-akse (replication) først hvis:"
- "Data consistency er kritisk"
- "Cross-shard queries er hyppige"
- "Team mangler erfaring med sharding"

---

## SLIDE 25: Konklusion
**Speaking Notes:**
- "Konklusion:"
- "Headroom hjælper med proaktiv kapacitetsplanlægning"
- "Kombination af Y-akse og Z-akse løser komplekse skaleringsproblemer"
- "Z-akse skalering giver store fordele - 8x skalering, 10x bedre latency"
- "Men også kompleksitet - data consistency, query kompleksitet, routing"
- "Anbefaling: Overvej Z-akse når geografisk spredt trafik og data kan naturligt partitioneres"

---

## SLIDE 26: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres opmærksomhed"
- "Jeg er klar til spørgsmål"

---

## EKSTRA NOTER TIL EKSAMEN:

### Hvis de spørger om headroom beregning:
- Formel: (Tilgængelig Kapacitet - Nuværende Belastning) / Nuværende Belastning × 100%
- Eksempel: (500-200)/200 = 150% headroom
- Bottleneck: Lavest headroom (fx Memory 43%)

### Hvis de spørger om Y+Z kombination:
- Y-akse: Microservices (6 services)
- Z-akse: Geografisk partitioning (8 databases)
- Resultat: 4x throughput, 8x connections, 10x latency forbedring

### Hvis de spørger om Z-akse kompromiser:
- Positive: 8x skalering, 10x latency, bedre isolation
- Negative: Data consistency, query kompleksitet, routing, migration, backup, costs

### Hvis de spørger om hvornår IKKE at bruge Z-akse:
- Data consistency er kritisk
- Cross-shard queries er hyppige
- Team mangler erfaring med sharding
- Overvej X-akse (replication) først
