using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationAPI.Application.Interfaces;
using NotificationAPI.Domain.Interfaces;
using NotificationAPI.Infrastructure.Channels;
using NotificationAPI.Infrastructure.Repositories;
using NotificationAPI.Infrastructure.Services;

namespace NotificationAPI.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseAndRepo(configuration);
//        services.AddDatabaseDeveloperPageExceptionFilter();
		services.AddServices();
		services.AddHttpClient(configuration);

        return services;
    }

    private static void AddDatabaseAndRepo(this IServiceCollection services, IConfiguration configuration)
    {
	    var connectionString = configuration.GetConnectionString("DefaultConnection");
	    services.AddDbContext<NotificationDbContext>(opt =>
	    {
		    opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
	    });

	    services.AddScoped<INotificationRepository, NotificationRepository>();
	    services.AddScoped<ITemplateRepository, TemplateRepository>();
    }

    private static void AddServices(this IServiceCollection services)
    {
	    services.AddScoped<NotificationDispatcher>();
//	    services.AddScoped<IUserRepository<ApplicationUser, UserDto>, UserRepository>();
	    services.AddScoped<INotificationChannel, EmailChannel>();
	    services.AddScoped<INotificationChannel, SmsChannel>();
	    services.AddScoped<INotificationChannel, PushChannel>();
	    services.AddScoped<ITemplateService, TemplateService>();
	    services.AddScoped<IRateLimiter, RateLimiter>();
    }

    private static void AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
	    services.AddHttpClient("UsersApiService", client =>
	    {
		    client.BaseAddress = new Uri(configuration["AuthApiSettings:BaseUrl"] ?? "https://localhost:7219");
		    client.DefaultRequestHeaders.Accept.Clear();
		    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	    });
    }
}
