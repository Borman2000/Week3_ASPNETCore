// #define SERILOG_RESPONSES       // Serilog logging/measurement. Otherwise - manual.

using System.Diagnostics;
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

    app.Use(async (context, next) =>
    {
        var timer = Stopwatch.StartNew();
        await next.Invoke();
#if !SERILOG_RESPONSES
        Log.Information($"Request ({context.Request.Method} {context.Request.Path}) processed in {timer.ElapsedMilliseconds} ms with status response {context.Response.StatusCode}");
#endif
    });
}

app.UseHttpsRedirection();

BooksEndpoints.Map(app);

app.Run();

Log.Information("----- FINISHING -----");
Log.CloseAndFlush();
