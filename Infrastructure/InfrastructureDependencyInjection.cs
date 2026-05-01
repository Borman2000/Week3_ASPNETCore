//#define IN_MEMORY_CACHE

using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using Application.Interfaces;
using Common.OpenTelemetryService;
using CSVParser;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Impl;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddRepositories();
        services.AddHttpClient(configuration);

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
	    services.AddOpenTelemetryTracing(configuration);
	    services.AddOpenTelemetryMetrics(configuration)
		    .WithInstrumentation<PerformanceMetrics>();

	    services.AddApplicationInsightsTelemetry();

		#if IN_MEMORY_CACHE
		    services.AddMemoryCache();
		    services.AddScoped<ICacheService, InMemoryCacheService>();
	    #else
		    services.AddStackExchangeRedisCache(options =>
		    {
			    options.Configuration = configuration["Redis:Configuration"];
			    options.InstanceName = configuration["Redis:InstanceName"];
			    IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
			    options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer);
		    });

		    services.AddScoped<ICacheService, RedisCacheService>();
	    #endif

	    services.AddScoped<IEmailService, EmailService>();
	    services.AddScoped<ParserSimple>();
	    services.AddScoped<ParserMemory>();
	    return services;
    }

    private static void AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
	    services.AddHttpClient("NotificationApiService", client =>
	    {
		    client.BaseAddress = new Uri("https://localhost:7019");
		    client.DefaultRequestHeaders.Accept.Clear();
		    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	    });
    }

    public class DbSettings
    {
        public const string Section = "DBSettings";

        [Required(AllowEmptyStrings = false)]
        public string ConnectionString { get; set; }
    }}
