using PharmaGo.Factory;
using PharmaGo.WebApi.Filters;
using PharmaGo.WebApi.Instrumentation;
using Prometheus;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterBusinessLogicServices(builder.Configuration);
builder.Services.RegisterDataAccessServices(builder.Configuration);
builder.Services.AddControllers(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.AddControllers();

builder.Services.AddHostedService<TestMetricService>();

builder.Services.AddPharmaGoOpenTelemetryMetrics();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins",
        policy =>
        {
            policy.WithOrigins("*") // note the port is included 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.MigrateDatabase();
app.InitializeDatabaseData();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyAllowedOrigins");

app.UseAuthorization();
app.UseMetricServer();    // expone /metrics
app.UseHttpMetrics();     // opcional para métricas HTTP automáticas

// Middleware para contabilizar todas las respuestas a /api/reserva incluyendo 4xx (model binding) y 5xx
app.Use(async (context, next) =>
{
    await next();

    try
    {
        var path = context.Request.Path.HasValue ? context.Request.Path.Value : string.Empty;
        var method = context.Request.Method ?? "";
        var statusCode = context.Response.StatusCode.ToString();

        if (!string.IsNullOrEmpty(path) && path.StartsWith("/api/reserva", System.StringComparison.OrdinalIgnoreCase))
        {
            // prometheus-net
            AppMetrics.ReservationRequestsTotal.WithLabels(path, method, statusCode).Inc();
            if (statusCode.StartsWith("4") || statusCode.StartsWith("5"))
            {
                var errorType = statusCode.StartsWith("5") ? "server" : "client";
                AppMetrics.ReservationErrorsTotal.WithLabels(path, method, statusCode, errorType).Inc();
            }

            // OpenTelemetry
            OpenTelemetryMetrics.ReservationRequests.Add(1, new System.Collections.Generic.KeyValuePair<string, object>("endpoint", path), new System.Collections.Generic.KeyValuePair<string, object>("method", method), new System.Collections.Generic.KeyValuePair<string, object>("status_code", statusCode));
            if (statusCode.StartsWith("4") || statusCode.StartsWith("5"))
            {
                OpenTelemetryMetrics.ReservationErrors.Add(1, new System.Collections.Generic.KeyValuePair<string, object>("endpoint", path), new System.Collections.Generic.KeyValuePair<string, object>("method", method), new System.Collections.Generic.KeyValuePair<string, object>("status_code", statusCode));
            }
        }
    }
    catch
    {
        // no queremos que el middleware falle la request si hay un problema con las métricas
    }
});

app.MapControllers();

app.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }
