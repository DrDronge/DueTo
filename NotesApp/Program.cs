using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

var log = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "Gainhub.Api") // useful to filter by the service
    // Configure Seq sink for all logs
    .WriteTo.Seq("http://localhost:5341")
    // Configure Sentry sink only for Error and Fatal levels
    // .WriteTo.Sentry(o =>
    // {
    //     // https://develop.sentry.dev/self-hosted/#getting-started
    //     o.Dsn = "http://localhost:9000";
    //     o.MinimumEventLevel = LogEventLevel.Error; // Only send Error and Fatal to Sentry
    // })
    .CreateLogger();

builder.Logging.AddSerilog(log);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("GainHub API")
            .WithDarkMode(true)
            .WithDownloadButton(true)
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.MapGet("/badrequest", () => Results.Problem(
    type: "Bad Request",
    detail: "test",
    statusCode: StatusCodes.Status400BadRequest,
    title: "Bad Request")
);