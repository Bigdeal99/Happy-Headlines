using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PublisherService.Messaging;
using Serilog;

var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://seq:5341";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(seqUrl)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// OpenTelemetry → Jaeger
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("PublisherService"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddJaegerExporter(o =>
        {
            o.AgentHost = Environment.GetEnvironmentVariable("JAEGER_HOST") ?? "jaeger";
            o.AgentPort = 6831;
        })
    );

builder.Services.AddSingleton<IArticlePublisher, ArticlePublisher>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
