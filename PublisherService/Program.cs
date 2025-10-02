using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PublisherService.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .Enrich.FromLogContext()
    .WriteTo.Console());

// HttpClient to ProfanityService (internal DNS)
builder.Services.AddHttpClient("profanity", c =>
{
    c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("PROFANITY_URL") ?? "http://profanity-service:8080");
});

// Rabbit publisher
builder.Services.AddSingleton<IArticleQueuePublisher, ArticleQueuePublisher>();
builder.Services.AddControllers();

// OpenTelemetry Tracing -> OTLP to Jaeger
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("PublisherService"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri(Environment.GetEnvironmentVariable("OTLP_ENDPOINT") ?? "http://jaeger:4317");
        }));

var app = builder.Build();
app.MapControllers();
app.Run();
