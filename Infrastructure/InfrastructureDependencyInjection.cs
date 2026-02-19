//#define IN_MEMORY_CACHE

using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using CSVParser;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Impl;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddRepositories();

        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork>(c => c.GetRequiredService<BookStoreDbContext>());
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.Decorate<IAuthorRepository, CachedAuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        // services.AddScoped<ITodoListRepository, TodoListRepository>();
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection(DbSettings.Section).Get<DbSettings>()?.ConnectionString;
        services.AddDbContext<BookStoreDbContext>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
	    services.AddOpenTelemetry()
		    .ConfigureResource(configuration => configuration.AddService("Books.api"))
		    .WithMetrics(builder =>
		    {
			    builder
				    .AddAspNetCoreInstrumentation()
//				    .AddHttpClientInstrumentation()
//				    .AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", "System.Net.Http", "Books.api")
				    .AddMeter("Books.api")
				    .AddInstrumentation<PerformanceMetrics>()
				    .AddPrometheusExporter() // Configures an endpoint for Prometheus to scrape
//				    .AddRuntimeInstrumentation() // Collects default .NET runtime metrics (GC, CPU, etc.)
//				    .AddProcessInstrumentation()
				    .AddOtlpExporter(options =>
				    {
//					    options.Endpoint = new Uri(configuration["Otlp:Endpoint"] ?? "http://localhost:4317");
					    options.Endpoint = new Uri("http://localhost:4317");
				    });
		    });
	    services.AddApplicationInsightsTelemetry();
	    services.AddSingleton<PerformanceMetrics>();

		#if IN_MEMORY_CACHE
		    services.AddMemoryCache();
		    services.AddScoped<ICacheService, InMemoryCacheService>();
	    #else
		    services.AddStackExchangeRedisCache(options =>
		    {
			    options.Configuration = configuration["Redis:Configuration"];
			    options.InstanceName = configuration["Redis:InstanceName"];
		    });

		    services.AddScoped<ICacheService, RedisCacheService>();
	    #endif

	    services.AddScoped<IEmailService, EmailService>();
	    services.AddScoped<ParserSimple>();
	    services.AddScoped<ParserMemory>();
	    return services;
    }

    public class DbSettings
    {
        public const string Section = "DBSettings";

        [Required(AllowEmptyStrings = false)]
        public string ConnectionString { get; set; }
    }}
