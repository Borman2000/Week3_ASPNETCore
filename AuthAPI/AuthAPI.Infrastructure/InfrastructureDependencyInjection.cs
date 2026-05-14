using AuthAPI.Application.DTOs;
using AuthAPI.Application.Interfaces;
using AuthAPI.Domain.Entities;
using AuthAPI.Domain.Interfaces;
using AuthAPI.Infrastructure.Repositories;
using AuthAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthAPI.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseAndRepo(configuration);
        services.AddServices();
        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }

    private static void AddDatabaseAndRepo(this IServiceCollection services, IConfiguration configuration)
    {
	    services.AddSingleton<AuditableInterceptor>();

	    var connectionString = configuration.GetConnectionString("DefaultConnection");
	    services.AddDbContext<UsersDbContext>((provider, opt) =>
	    {
		    var interceptor = provider.GetRequiredService<AuditableInterceptor>();
		    opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
			    .AddInterceptors(interceptor);
	    });

	    services.AddScoped<IUserRepository<ApplicationUser, UserDto>, UserRepository>();
    }

    private static void AddServices(this IServiceCollection services)
    {
	    services
		    .AddIdentity<ApplicationUser, IdentityRole>(options =>
		    {
			    options.Password.RequireDigit = true;
			    options.Password.RequireLowercase = true;
			    options.Password.RequireUppercase = true;
			    options.Password.RequireNonAlphanumeric = true;
			    options.Password.RequiredLength = 8;
		    })
		    .AddEntityFrameworkStores<UsersDbContext>()
		    .AddSignInManager()
		    .AddDefaultTokenProviders();

	    services.AddScoped<IIdentityService, IdentityService>();
    }
}
