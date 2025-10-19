using System.ComponentModel.DataAnnotations;
using DataAccess.Repositories;
using DataAccess.Repositories.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataAccess;

public static class DataAccessDependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddIdentity();

        services.AddRepositories();

        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork>(c => c.GetRequiredService<BookStoreDbContext>());
        services.AddScoped<IBookRepository, BookRepository>();
        // services.AddScoped<ITodoListRepository, TodoListRepository>();
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection(DbSettings.Section).Get<DbSettings>()?.ConnectionString;
        services.AddDbContext<BookStoreDbContext>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        //
        //    if (databaseConfig.UseInMemoryDatabase)
        //        services.AddDbContext<DatabaseContext>(options =>
        //        {
        //            options.UseInMemoryDatabase("NTierDatabase");
        //            options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        //        });
        //    else
        //        services.AddDbContext<DatabaseContext>(options =>
        //            options.UseSqlServer(databaseConfig.ConnectionString,
        //                opt => opt.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)));
    }

    private static void AddIdentity(this IServiceCollection services)
    {
        // services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        //     .AddEntityFrameworkStores<DatabaseContext>();
        //
        // services.Configure<IdentityOptions>(options =>
        // {
        //     options.Password.RequireDigit = true;
        //     options.Password.RequireLowercase = true;
        //     options.Password.RequireNonAlphanumeric = true;
        //     options.Password.RequireUppercase = true;
        //     options.Password.RequiredLength = 6;
        //     options.Password.RequiredUniqueChars = 1;
        //
        //     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //     options.Lockout.MaxFailedAccessAttempts = 5;
        //     options.Lockout.AllowedForNewUsers = true;
        //
        //     options.User.AllowedUserNameCharacters =
        //         "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        //     options.User.RequireUniqueEmail = true;
        // });
    }

    public class DbSettings
    {
        public const string Section = "DBSettings";

        [Required(AllowEmptyStrings = false)]
        public string ConnectionString { get; set; }
    }}
