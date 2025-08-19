using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;
using Service1.Services;
using Service1.Middleware;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .Enrich.WithProperty("ServiceName", "Service1")
        .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName);
});

// Add services
builder.Services.AddControllers();
builder.Services.AddHttpClient<IService2Client, Service2Client>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("Service2") ?? "http://service2:8081");
});

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("Service1", "1.0.0"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddJaegerExporter();
    });

var app = builder.Build();

// Add custom middleware for request tracking
app.UseMiddleware<RequestTrackingMiddleware>();

// Configure Prometheus metrics
app.UseMetricServer();
app.UseHttpMetrics();

app.UseRouting();
app.MapControllers();

app.Run();
