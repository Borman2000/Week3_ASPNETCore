// #define SERILOG_RESPONSES       // Serilog logging/measurement. Otherwise - manual.

using Application;
using Application.Models;
using DataAccess;
using Microsoft.Extensions.Options;
using Serilog;
using WebAPI;
using WebAPI.Middleware;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"Minimal_API_{DateTime.Now.ToString("yyyyMMdd")}.log")
    .CreateLogger();
Log.Information("----- STARTING -----");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();
builder.Services.AddApplication(builder.Configuration)
                .AddDataAccess(builder.Configuration);
builder.Services.AddHealthChecks();

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
    app.UseCustomMiddleware();
}

// app.UseDefaultFiles(); // Enables serving default files like index.html
app.UseStaticFiles();  // Enables serving static files from wwwroot
app.UseHttpsRedirection();

// using (var scope = app.Services.CreateScope())
// {
// 	var scopedService = scope.ServiceProvider.GetRequiredService<IBookRepository>();
//	BooksEndpoints.Map(app, scopedService);
// }
	Endpoints.Map(app);

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI V1"); });
app.MapHealthChecks("/health");

// To test Environments: Development <=> Production in IIS Express in launchSettings.json
Console.WriteLine($"App name: {app.Services.GetRequiredService<IOptions<ApiSettings>>().Value.Name}, version: {app.Services.GetRequiredService<IOptions<ApiSettings>>().Value.Version}");

app.Run();

Log.Information("----- FINISHING -----");
Log.CloseAndFlush();
