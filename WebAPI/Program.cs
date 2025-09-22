// #define SERILOG_RESPONSES       // Serilog logging/measurement. Otherwise - manual.
#define USE_INLINE

#if USE_INLINE
using System.Diagnostics;
using Microsoft.Extensions.Options;
#endif
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebAPI;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"Minimal_API_{DateTime.Now.ToString("yyyyMMdd")}.log")
    .CreateLogger();
Log.Information("----- STARTING -----");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<BooksDb>(opt => opt.UseInMemoryDatabase("BookList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.Configure<CustomOptions>(builder.Configuration.GetSection(CustomOptions.Custom));

#if SERILOG_RESPONSES
builder.Services.AddSerilog();
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
#if SERILOG_RESPONSES
    app.UseSerilogRequestLogging();
#endif
#if USE_INLINE
    app.Use(async (context, next) =>
    {
        var timer = Stopwatch.StartNew();
        var strCorrName = app.Services.GetRequiredService<IOptions<CustomOptions>>().Value.CorrelationName;
        var correlationId = context.Request.Headers[strCorrName].FirstOrDefault();

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

		context.Response.Headers[strCorrName] = correlationId;

        context.Items["CorrelationId"] = correlationId;

        await next.Invoke();

#if !SERILOG_RESPONSES
        Log.Information($"Request {correlationId}: ({context.Request.Method} {context.Request.Path}) processed in {timer.ElapsedMilliseconds} ms with status response {context.Response.StatusCode}");
#endif
    });
#else
    app.UseCustomMiddleware();
#endif
}

app.UseHttpsRedirection();

BooksEndpoints.Map(app);

app.Run();

Log.Information("----- FINISHING -----");
Log.CloseAndFlush();
