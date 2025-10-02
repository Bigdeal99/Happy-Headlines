using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc.Enrich.FromLogContext().WriteTo.Console());

builder.Services.AddHttpClient("article", c =>
{
    c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ARTICLE_URL") ?? "http://article-service:8080");
});
builder.Services.AddControllers();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("NewsletterService"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o => o.Endpoint = new Uri(Environment.GetEnvironmentVariable("OTLP_ENDPOINT") ?? "http://jaeger:4317")));

var app = builder.Build();
app.MapControllers();
app.Run();
