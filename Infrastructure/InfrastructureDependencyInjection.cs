using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using CSVParser;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Impl;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
	    services.AddMemoryCache();
	    services.AddScoped<ICacheService, InMemoryCacheService>();
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
