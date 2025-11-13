using Microsoft.Extensions.Http.Polly;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using NewsletterService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc.Enrich.FromLogContext().WriteTo.Console());

// Article Service HTTP Client
builder.Services.AddHttpClient("article", c =>
{
    c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ARTICLE_URL") ?? "http://article-service:8080");
});

// Subscriber Service HTTP Client with Circuit Breaker (Fault Isolation)
var subscriberServiceUrl = Environment.GetEnvironmentVariable("SUBSCRIBER_URL") ?? "http://subscriber-service:8080";
var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 3,
        durationOfBreak: TimeSpan.FromSeconds(30));

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        retryCount: 2,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));

var subscriberPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);

builder.Services.AddHttpClient<SubscriberClient>("subscriber", c =>
{
    c.BaseAddress = new Uri(subscriberServiceUrl);
    c.Timeout = TimeSpan.FromSeconds(10);
})
.AddPolicyHandler(subscriberPolicy);

builder.Services.AddScoped<SubscriberClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = factory.CreateClient("subscriber");
    var logger = sp.GetRequiredService<ILogger<SubscriberClient>>();
    return new SubscriberClient(httpClient, logger);
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
