# Google Slides Content - Spørgsmål 3: Skaleringskuben
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Skaleringskuben
**Undertitel:** Headroom, Y+Z akser, og Z-akse kompromiser
**Footer:** Dit navn | Dato

---

## SLIDE 2: Hvad er Headroom?
**Titel:** Hvad er Headroom?

**Indhold:**
- **Definition:** Ekstra kapacitet ud over nuværende belastning
- **Formål:** Buffer til trafik-tilvækst, spikes, fejl, vedligeholdelse
- **Formel:** (Tilgængelig Kapacitet - Nuværende Belastning) / Nuværende Belastning × 100%

---

## SLIDE 3: Hvordan Headroom Hjælper - Proaktiv Skalering
**Titel:** Proaktiv Skaleringsbeslutninger

**Indhold:**
- Når headroom < 20% → Planlæg skalering
- Giver tid før systemet når kapacitet
- Forhindrer reaktive nødskaleringssituationer

---

## SLIDE 4: Hvordan Headroom Hjælper - Resource Allocation
**Titel:** Resource Allocation

**Indhold:**
- Identificer hvilke komponenter skal skaleres først
- Prioriter investeringer i infrastruktur
- Optimér cost vs. performance balance
- **Eksempel:** ArticleService memory headroom = 43% → Fokusér her

---

## SLIDE 5: Headroom Metrics og Tærskler
**Titel:** Headroom Metrics og Tærskler

**Indhold:**
- **Metrics:** CPU, Memory, Database connections, Network bandwidth, Throughput
- **Tærskler:**
  - Kritisk: < 10% → Skal skale umiddelbart
  - Advarsel: 10-20% → Planlæg skalering
  - Normal: 20-50% → Acceptabelt
  - Komfortabel: > 50% → God buffer

---

## SLIDE 6: Eksempel - Headroom Beregning
**Titel:** Eksempel: Headroom Beregning

**Indhold:**
- **Scenario:** ArticleService
- **Nuværende:** 200 req/s, CPU 60%, Memory 70%, DB connections 40/100
- **Max kapacitet:** 500 req/s, CPU 100%, Memory 100%, DB connections 100
- **Headroom:** Throughput 150%, CPU 67%, Memory 43%, DB 150%
- **Bottleneck:** Memory (43% - lavest)

---

## SLIDE 7: Kapacitetsplanlægning med Headroom
**Titel:** Kapacitetsplanlægning

**Indhold:**
- Memory headroom: 43% → Acceptabelt, men overvåg
- Hvis trafik stiger 50%: 200 → 300 req/s
- Memory vil stige til ~85% → 15% headroom (kritisk)
- **Beslutning:** Skal skale op før trafik stiger

---

## SLIDE 8: Skaleringskubens Tre Akser
**Titel:** Skaleringskubens Tre Akser

**Indhold:**
- **X-akse:** Horizontal Duplication (klon service)
- **Y-akse:** Functional Decomposition (microservices)
- **Z-akse:** Data Partitioning (sharding, geografisk)
- **Fokus:** Y + Z kombination

---

## SLIDE 9: Problemstilling
**Titel:** Problemstilling

**Indhold:**
- ArticleService kan kun håndtere 500 req/s
- Trafik stiger til 2000 req/s (4x stigning)
- Database bottleneck (100 connections max)
- Geografisk spredt trafik (Europa, Asien, Nord-Amerika)

---

## SLIDE 10: Del 1 - Y-akse Skalering
**Titel:** Y-akse Skalering (Microservices)

**Indhold:**
- **Fil:** `ArticleService/Controllers/ArticlesController.cs`
- **Fil:** `CommentService/Controllers/CommentsController.cs`
- **Arkitektur:** Monolitisk → Microservices (6 services)
- **Fordel:** Hver service kan skaleres uafhængigt

**Screenshot:** Vis ArticlesController og CommentsController

---

## SLIDE 11: Del 2 - Z-akse Skalering
**Titel:** Z-akse Skalering (Geografisk Partitioning)

**Indhold:**
- **Fil:** `docker-compose.yml` linje 20-69
- **Fil:** `ArticleService/Models/Article.cs` linje 8
- **Arkitektur:** 1 database → 8 databases (global + 7 kontinenter)
- **Fordel:** Geografisk nærhed reducerer latency

**Screenshot:** Vis docker-compose.yml databases og Article.cs Continent property

---

## SLIDE 12: Kombineret Y + Z-akse Løsning
**Titel:** Kombineret Løsning

**Indhold:**
- **Før:** Monolitisk App → article-db-global (2000 req/s, bottleneck)
- **Efter:** ArticleService (Y) → Router → 8 Continent DBs (Z)
- **Resultat:** 2000 req/s → ~250 req/s per DB

---

## SLIDE 13: Performance Forbedring - Y-akse
**Titel:** Performance Forbedring: Y-akse

**Indhold:**
- ArticleService: 500 req/s → 2000 req/s (4x)
- CommentService: Kan skaleres uafhængigt
- ProfanityService: Isoleret fra article trafik
- **Fordel:** Independent scaling

---

## SLIDE 14: Performance Forbedring - Z-akse
**Titel:** Performance Forbedring: Z-akse

**Indhold:**
- Database connections: 100 → 800 (8 DBs × 100)
- Trafik fordelt: 2000 req/s → ~250 req/s per DB
- Geografisk latency: 500ms → 50ms (lokale DBs)
- Database size: 1TB → ~125GB per DB (8x mindre)

---

## SLIDE 15: Kombineret Resultat
**Titel:** Kombineret Resultat

**Indhold:**
| Metrik | Før | Efter | Forbedring |
|--------|-----|-------|------------|
| Max throughput | 500 req/s | 2000 req/s | 4x |
| DB connections | 100 | 800 | 8x |
| Response time | 500ms | 50ms | 10x |
| DB size | 1TB | 125GB/DB | 8x mindre |
| Geografisk latency | Høj | Lav | 10x bedre |

---

## SLIDE 16: Kompromis 1 - Data Consistency
**Titel:** Kompromis: Data Consistency

**Indhold:**
- **Problem:** ACID transaktioner på tværs af alle data
- **Før:** ACID transaktioner på tværs af alle data
- **Efter:** Transaktioner kun inden for samme shard
- **Konsekvens:** Cross-shard transaktioner komplekse eller umulige

---

## SLIDE 17: Kompromis 2 - Query Kompleksitet
**Titel:** Kompromis: Query Kompleksitet

**Indhold:**
- **Problem:** Queries der spænder over flere shards
- **Før:** Simpel query - `SELECT * FROM Articles WHERE PublishedAt > '2024-01-01'`
- **Efter:** Skal query alle 8 databases og merge resultater
- **Trade-off:** Hurtigere per DB, men langsommere cross-shard

---

## SLIDE 18: Kompromis 3 - Shard Routing
**Titel:** Kompromis: Shard Routing

**Indhold:**
- **Problem:** Systemet skal vide hvilken shard data skal til/fra
- **Før:** Ingen routing - alt går til én database
- **Efter:** Routing logic nødvendig baseret på continent
- **Trade-off:** Geografisk tæt, men routing kompleksitet

---

## SLIDE 19: Kompromis 4 - Data Migration
**Titel:** Kompromis: Data Migration

**Indhold:**
- **Problem:** Hvis data-distributionen ændrer sig
- **Før:** Ingen rebalancing nødvendig
- **Efter:** Hvis kontinent vokser, skal data flyttes
- **Eksempel:** Europa 80% → article-db-europe overbelastet → Migration kompleks

---

## SLIDE 20: Kompromis 5 - Backup og Recovery
**Titel:** Kompromis: Backup og Recovery

**Indhold:**
- **Problem:** Backup og recovery skal håndtere flere databases
- **Før:** 1 backup, 1 recovery procedure
- **Efter:** 8 backups, koordineret recovery
- **Trade-off:** Isolation ved fejl, men 8x mere komplekst

---

## SLIDE 21: Kompromis 6 - Monitoring og Development
**Titel:** Kompromis: Monitoring og Development

**Indhold:**
- **Monitoring:** 1 database → 8 databases at overvåge
- **Development:** 1 database → 8 databases i docker-compose
- **Trade-off:** Bedre isolation, men mere komplekst

---

## SLIDE 22: Kompromis 7 - Cost Overhead
**Titel:** Kompromis: Cost Overhead

**Indhold:**
- **Problem:** Flere databases = højere omkostninger
- **Før:** 1 database server
- **Efter:** 8 database servers
- **Trade-off:** Bedre performance, men 8x costs

---

## SLIDE 23: Kompromis Summary
**Titel:** Kompromis Summary

**Indhold:**
- **Positive:** 8x skalering, 10x latency, bedre isolation, mindre data per DB
- **Negative:** Data consistency, query kompleksitet, routing, migration, backup, monitoring, costs

---

## SLIDE 24: Når Brug Z-akse Skalering?
**Titel:** Når Brug Z-akse Skalering?

**Indhold:**
- **✅ Brug når:**
  - Geografisk spredt trafik
  - Data kan naturligt partitioneres (fx continent)
  - Cross-shard queries er sjældne
  - Team har erfaring med distributed systems
- **❌ Overvej X-akse først hvis:**
  - Data consistency er kritisk
  - Cross-shard queries er hyppige
  - Team mangler erfaring

---

## SLIDE 25: Konklusion
**Titel:** Konklusion

**Indhold:**
- Headroom hjælper med proaktiv kapacitetsplanlægning
- Y+Z kombination løser komplekse skaleringsproblemer
- Z-akse skalering giver store fordele, men også kompleksitet
- **Anbefaling:** Overvej Z-akse når geografisk spredt trafik og naturlig partitionering

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
   - Tilføj kode-snippets som billeder eller tekstbokse
4. **Screenshots:**
   - Tag screenshots af de angivne filer
   - Indsæt som billeder på relevante slides
   - Tilføj annotations hvis nødvendigt
