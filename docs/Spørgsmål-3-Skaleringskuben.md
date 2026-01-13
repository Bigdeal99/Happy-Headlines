# Spørgsmål 3: Skaleringskuben

## A. Forklar hvordan headroom-beregninger kan hjælpe med kapacitetsplanlægning

### Headroom-beregninger:

#### 1. **Definition af Headroom**
Headroom er den ekstra kapacitet man har til rådighed ud over den nuværende belastning. Det er en buffer der giver plads til:
- Trafik-tilvækst
- Trafik-spidsbelastninger (spikes)
- Fejl i systemet (når en komponent fejler, skal andre håndtere ekstra load)
- Planlagt vedligeholdelse

#### 2. **Headroom-formel**
```
Headroom = (Tilgængelig Kapacitet - Nuværende Belastning) / Nuværende Belastning × 100%
```

Eller:
```
Headroom = (Max Kapacitet - Gennemsnitlig Belastning) / Gennemsnitlig Belastning × 100%
```

#### 3. **Hvordan headroom hjælper med kapacitetsplanlægning**

**A. Proaktiv Skaleringsbeslutninger:**
- Når headroom falder under en tærskel (fx 20%), skal man skale op
- Giver tid til at planlægge skalering før systemet når kapacitet
- Forhindrer reaktive nødskaleringssituationer

**B. Resource Allocation:**
- Hjælper med at identificere hvilke komponenter der skal skaleres først
- Prioriterer investeringer i infrastruktur
- Optimerer cost vs. performance balance

**C. Performance Degradation Forudsigelse:**
- Når headroom er lav, kan systemet håndtere færre spikes
- Giver tid til at forberede på trafik-tilvækst
- Forhindrer pludselige performance-problemer

**D. Disaster Recovery Planlægning:**
- Hvis en komponent fejler, skal andre håndtere ekstra load
- Headroom sikrer at systemet kan håndtere fejl uden at gå ned
- Giver buffer til failover-scenarier

#### 4. **Headroom Metrics**

**Kapacitets-metrics:**
- **CPU utilization**: 70% brugt = 30% headroom
- **Memory utilization**: 60% brugt = 40% headroom
- **Database connections**: 50/100 = 50% headroom
- **Network bandwidth**: 800 Mbps / 1000 Mbps = 20% headroom
- **Request throughput**: 400 req/s / 500 req/s capacity = 20% headroom

**Tærskelværdier:**
- **Kritisk**: < 10% headroom → Skal skale umiddelbart
- **Advarsel**: 10-20% headroom → Planlæg skalering
- **Normal**: 20-50% headroom → Acceptabelt niveau
- **Komfortabel**: > 50% headroom → God buffer

#### 5. **Eksempel på Headroom-beregning**

**Scenario: ArticleService**

**Nuværende belastning:**
- Gennemsnitlig: 200 requests/sekund
- Peak: 400 requests/sekund
- CPU: 60% utilization
- Memory: 70% utilization
- Database connections: 40/100

**Maksimal kapacitet:**
- Max throughput: 500 requests/sekund
- Max CPU: 100%
- Max Memory: 100%
- Max DB connections: 100

**Headroom-beregning:**
```
Throughput headroom = (500 - 200) / 200 × 100% = 150%
CPU headroom = (100% - 60%) / 60% × 100% = 67%
Memory headroom = (100% - 70%) / 70% × 100% = 43%
DB connection headroom = (100 - 40) / 40 × 100% = 150%
```

**Bottleneck**: Memory (43% headroom - lavest)

**Kapacitetsplanlægning:**
- Memory headroom er 43% → Acceptabelt, men overvåg
- Hvis trafik stiger 50%: 200 → 300 req/s
  - Memory vil stige til ~85% → 15% headroom (kritisk)
  - **Beslutning**: Skal skale op før trafik stiger

#### 6. **Headroom i Microservices-arkitektur**

I en microservices-arkitektur skal man beregne headroom for hver service:

**ArticleService:**
- Nuværende: 200 req/s, CPU 60%, Memory 70%
- Headroom: 67% CPU, 43% Memory

**CommentService:**
- Nuværende: 150 req/s, CPU 50%, Memory 60%
- Headroom: 100% CPU, 67% Memory

**ProfanityService:**
- Nuværende: 100 req/s, CPU 40%, Memory 50%
- Headroom: 150% CPU, 100% Memory

**System-bottleneck**: ArticleService memory (43% headroom)

**Kapacitetsplanlægning:**
- Fokusér på at skale ArticleService først
- CommentService og ProfanityService har god headroom
- Skaler ArticleService memory før næste trafik-tilvækst

---

## B. Konstruer et eksempel, hvor en kombination af to akser løser et konkret skaleringsproblem

### Skaleringskubens Akser:

- **X-akse (Horizontal Duplication)**: Klon samme service flere gange (load balancing)
- **Y-akse (Functional Decomposition)**: Split efter funktionalitet (microservices)
- **Z-akse (Data Partitioning)**: Split efter data (sharding, geografisk)

### Eksempel: Happy-Headlines Skaleringsproblem

**Problemstilling:**
Happy-Headlines oplever performance-problemer:
- ArticleService kan kun håndtere 500 req/s
- Trafik stiger til 2000 req/s (4x stigning)
- Database er bottleneck (100 connections max)
- Geografisk spredt trafik (Europa, Asien, Nord-Amerika)

**Løsning: Kombination af Y-akse + Z-akse skalering**

### Kombination: Y-akse (Microservices) + Z-akse (Geografisk Partitioning)

#### Del 1: Y-akse Skalering (Allerede implementeret)

**Arkitektur:**
```
Monolitisk System
    ↓
Y-akse Split:
├── ArticleService (article management)
├── CommentService (comment management)
├── ProfanityService (profanity filtering)
├── PublisherService (article publishing)
└── NewsletterService (newsletter generation)
```

**Fordele:**
- Hver service kan skaleres uafhængigt
- CommentService kan håndtere høj trafik uden at påvirke ArticleService
- ProfanityService kan optimeres separat

**Kodeeksempel - Y-akse:**
```csharp
// ArticleService/Controllers/ArticlesController.cs
// Isoleret service for article management
[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    // Kun article-relateret funktionalitet
}

// CommentService/Controllers/CommentsController.cs  
// Separeret service for comment management
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    // Kun comment-relatered funktionalitet
}
```

#### Del 2: Z-akse Skalering (Geografisk Partitioning)

**Problem:** 
- Alle requests går til én database (article-db-global)
- Database bliver bottleneck ved høj trafik
- Geografisk latency (Europa → Global DB = høj latency)

**Løsning: Z-akse split efter Continent**

**Arkitektur:**
```
article-db-global (Global articles)
    ↓
Z-akse Split:
├── article-db-europe (Europa articles)
├── article-db-asia (Asien articles)
├── article-db-north-america (Nord-Amerika articles)
├── article-db-south-america (Syd-Amerika articles)
├── article-db-africa (Afrika articles)
├── article-db-oceania (Oceania articles)
└── article-db-antarctica (Antarktis articles)
```

**Kodeeksempel - Z-akse:**

```csharp
// ArticleService/Models/Article.cs (linje 8)
public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Continent { get; set; } = "Global"; // Z-axis split key
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}
```

```csharp
// PublisherService/Models/PublishArticleRequest.cs
public class PublishArticleRequest
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string Continent { get; set; } // Routing key for Z-axis
}
```

```csharp
// ArticleService/Services/ArticleQueueConsumer.cs (linje 57-63)
db.Articles.Add(new Article
{
    Title = msg.Title,
    Content = msg.Content,
    Continent = msg.Continent, // Stored for Z-axis routing
    PublishedAt = msg.PublishedAt
});
```

**Docker Compose Konfiguration:**
```yaml
# docker-compose.yml (linje 20-69)
# Global + 7 continent DBs (Z-axis partitioning)
article-db-global:
  image: mcr.microsoft.com/mssql/server:2022-latest

article-db-europe:
  image: mcr.microsoft.com/mssql/server:2022-latest

article-db-asia:
  image: mcr.microsoft.com/mssql/server:2022-latest

article-db-north-america:
  image: mcr.microsoft.com/mssql/server:2022-latest

# ... osv for alle kontinenter
```

### Kombineret Y + Z-akse Løsning

**Før (Monolitisk + Single DB):**
```
Monolitisk App → article-db-global
- 2000 req/s → Database bottleneck
- 100 connections → Overbelastet
- Response time: 500ms (geografisk latency)
```

**Efter (Y-akse + Z-akse):**
```
ArticleService (Y-akse) → Router → Continent DBs (Z-akse)
├── Europe requests → article-db-europe
├── Asia requests → article-db-asia
├── North-America requests → article-db-north-america
└── ... (7 continent DBs)

CommentService (Y-akse) → comment-db (separat)
ProfanityService (Y-akse) → profanity-db (separat)
```

### Performance-forbedring

**Y-akse fordele:**
- ArticleService: 500 req/s → 2000 req/s (4x)
- CommentService: Kan skaleres uafhængigt
- ProfanityService: Isoleret fra article trafik

**Z-akse fordele:**
- Database connections: 100 → 800 (8 DBs × 100)
- Trafik fordelt: 2000 req/s → ~250 req/s per DB
- Geografisk latency: 500ms → 50ms (lokale DBs)
- Database size: 1TB → ~125GB per DB (8x mindre)

**Kombineret resultat:**
| Metrik | Før | Efter | Forbedring |
|--------|-----|-------|------------|
| Max throughput | 500 req/s | 2000 req/s | 4x |
| Database connections | 100 (bottleneck) | 800 (8 DBs) | 8x |
| Response time | 500ms | 50ms | 10x |
| Database size | 1TB | 125GB/DB | 8x mindre |
| Geografisk latency | Høj | Lav | 10x bedre |

### Konkret Skaleringsproblem Løst

**Problem:**
- 2000 req/s trafik
- Database bottleneck (100 connections)
- Høj geografisk latency

**Løsning:**
1. **Y-akse**: Split monolitisk system → Microservices
   - ArticleService isoleret fra CommentService
   - Hver service kan skaleres uafhængigt

2. **Z-akse**: Split database efter continent
   - 8 databases i stedet for 1
   - 800 connections i stedet for 100
   - Geografisk nærhed reducerer latency

**Resultat:**
- ✅ Kan håndtere 2000 req/s (4x forbedring)
- ✅ Database bottleneck løst (8x flere connections)
- ✅ Geografisk latency reduceret (10x forbedring)
- ✅ Hver database er mindre og hurtigere (8x mindre data)

---

## C. Vurder hvilke kompromiser der opstår ved at vælge z-akse-skalering i et system, der oprindeligt er designet til monolitisk datalagring

### Kompromiser ved Z-akse Skalering

#### 1. **Data Consistency Kompleksitet**

**Problem:**
Monolitiske systemer har typisk ACID-transaktioner på tværs af alle data. Z-akse split betyder data er på tværs af flere databases.

**Kompromis:**
- **Før (Monolitisk)**: ACID transaktioner på tværs af alle data
- **Efter (Z-akse)**: Transaktioner kun inden for samme shard
- **Konsekvens**: Cross-shard transaktioner er komplekse eller umulige

**Eksempel fra kodebasen:**
```csharp
// ArticleService/Models/Article.cs
public string Continent { get; set; } = "Global"; // Z-axis split key

// Problem: Hvad hvis en artikel skal være i flere kontinenter?
// Løsning: Enten kopier data eller accepter at data er shardet
```

**Trade-off:**
- ✅ Bedre performance (mindre data per DB)
- ❌ Mere kompleks data consistency
- ❌ Cross-shard queries er langsomme eller umulige

#### 2. **Query Kompleksitet**

**Problem:**
Queries der spænder over flere shards bliver komplekse.

**Kompromis:**
- **Før (Monolitisk)**: Simpel query: `SELECT * FROM Articles WHERE PublishedAt > '2024-01-01'`
- **Efter (Z-akse)**: Skal query alle 8 databases og merge resultater

**Eksempel:**
```csharp
// Før: Simpel query
var articles = await _context.Articles
    .Where(a => a.PublishedAt >= since)
    .OrderByDescending(a => a.PublishedAt)
    .Take(10)
    .ToListAsync();

// Efter Z-akse: Kompleks query
// Skal query alle 8 continent databases
var europeArticles = await europeDb.Articles.Where(...).ToListAsync();
var asiaArticles = await asiaDb.Articles.Where(...).ToListAsync();
// ... for alle 8 databases
var allArticles = europeArticles.Concat(asiaArticles)...OrderBy(...).Take(10);
```

**Trade-off:**
- ✅ Hver database query er hurtigere (mindre data)
- ❌ Cross-shard queries er langsommere (8 queries + merge)
- ❌ Mere kompleks kode

#### 3. **Shard Routing Kompleksitet**

**Problem:**
Systemet skal vide hvilken shard data skal til/fra.

**Kompromis:**
- **Før (Monolitisk)**: Ingen routing - alt går til én database
- **Efter (Z-akse)**: Routing logic nødvendig baseret på continent

**Eksempel fra kodebasen:**
```csharp
// ArticleService/Program.cs (linje 14-15)
// Nuværende: Kun global DB connection
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")
    ?? "Server=article-db-global;Database=Articles;...";

// Efter Z-akse skalering: Routing logic nødvendig
public string GetConnectionString(string continent)
{
    return continent switch
    {
        "Europe" => "Server=article-db-europe;...",
        "Asia" => "Server=article-db-asia;...",
        "North-America" => "Server=article-db-north-america;...",
        // ... osv
        _ => "Server=article-db-global;..."
    };
}
```

**Trade-off:**
- ✅ Data er geografisk tæt på brugere
- ❌ Routing logic tilføjer kompleksitet
- ❌ Fejl i routing kan føre til data i forkert shard

#### 4. **Rebalancing og Data Migration**

**Problem:**
Hvis data-distributionen ændrer sig, skal data flyttes mellem shards.

**Kompromis:**
- **Før (Monolitisk)**: Ingen rebalancing nødvendig
- **Efter (Z-akse)**: Hvis et kontinent vokser, skal data muligvis flyttes

**Eksempel:**
- Europa har 80% af artikler → article-db-europe bliver overbelastet
- Skal flytte nogle artikler til article-db-global eller oprette article-db-europe-2
- Migration er kompleks og risikabel

**Trade-off:**
- ✅ Kan skaleres ved at tilføje nye shards
- ❌ Data migration er kompleks og risikabel
- ❌ Kan kræve downtime eller kompleks live migration

#### 5. **Backup og Recovery Kompleksitet**

**Problem:**
Backup og recovery skal håndtere flere databases.

**Kompromis:**
- **Før (Monolitisk)**: 1 backup, 1 recovery procedure
- **Efter (Z-akse)**: 8 backups, koordineret recovery

**Trade-off:**
- ✅ Hvis én shard fejler, påvirker det kun den shard
- ❌ Backup/restore er 8x mere komplekst
- ❌ Point-in-time recovery kræver koordinering af alle shards

#### 6. **Monitoring og Observability**

**Problem:**
Monitoring skal dække flere databases.

**Kompromis:**
- **Før (Monolitisk)**: 1 database at overvåge
- **Efter (Z-akse)**: 8 databases at overvåge

**Eksempel fra kodebasen:**
```yaml
# docs/prometheus.yml
# Nuværende: 1 article-service
scrape_configs:
  - job_name: 'article-service'
    static_configs:
      - targets: ['article-service:8080']

# Efter Z-akse: Skal overvåge alle shards
# - article-service-europe:8080
# - article-service-asia:8080
# - ... osv
```

**Trade-off:**
- ✅ Kan identificere problemer i specifikke shards
- ❌ 8x mere monitoring data
- ❌ Mere kompleks dashboard setup

#### 7. **Development og Testing Kompleksitet**

**Problem:**
Local development og testing bliver mere komplekst.

**Kompromis:**
- **Før (Monolitisk)**: 1 database i docker-compose
- **Efter (Z-akse)**: 8 databases i docker-compose

**Eksempel fra kodebasen:**
```yaml
# docker-compose.yml
# Nuværende: 8 databases allerede defineret (men kun 1 bruges)
article-db-global: ...
article-db-europe: ...
article-db-asia: ...
# ... 8 databases

# Development kompleksitet:
# - Skal starte 8 databases
# - Mere memory brugt
# - Længere startup tid
```

**Trade-off:**
- ✅ Production-lignende miljø
- ❌ Mere kompleks local setup
- ❌ Mere memory og disk space brugt

#### 8. **Cost Overhead**

**Problem:**
Flere databases = højere omkostninger.

**Kompromis:**
- **Før (Monolitisk)**: 1 database server
- **Efter (Z-akse)**: 8 database servers

**Trade-off:**
- ✅ Bedre performance og skalering
- ❌ 8x database server costs (hvis ikke shared infrastructure)
- ❌ Mere kompleks cost management

### Konklusion: Netto Vurdering

**Positive kompromiser:**
- ✅ 8x bedre skalering (8 databases)
- ✅ 10x bedre geografisk latency
- ✅ Bedre isolation (fejl i én shard påvirker ikke andre)
- ✅ Mindre data per database = hurtigere queries

**Negative kompromiser:**
- ❌ Mere kompleks data consistency
- ❌ Cross-shard queries er langsomme
- ❌ Routing logic kompleksitet
- ❌ Data migration kompleksitet
- ❌ Backup/recovery kompleksitet
- ❌ Monitoring kompleksitet
- ❌ Development setup kompleksitet
- ❌ Højere costs (8x databases)

**Anbefaling:**
Z-akse skalering er værd at overveje når:
- ✅ Geografisk spredt trafik
- ✅ Data kan naturligt partitioneres (fx continent)
- ✅ Cross-shard queries er sjældne
- ✅ Team har erfaring med distributed systems

**Alternativ:**
Overvej X-akse (replication) først hvis:
- Data consistency er kritisk
- Cross-shard queries er hyppige
- Team mangler erfaring med sharding
