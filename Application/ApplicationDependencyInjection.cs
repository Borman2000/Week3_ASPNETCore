using Application.Models;
using DataAccess.Repositories;
using DataAccess.Repositories.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationDependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddServices(configuration);

		services.RegisterMapper();

		return services;
	}

	private static void AddServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddEndpointsApiExplorer();
		services.AddOptions<ApiSettings>().Bind(configuration.GetSection(ApiSettings.Section)).ValidateDataAnnotations().ValidateOnStart();
		services.AddScoped<IAuthorRepository, AuthorRepository>();
		services.AddScoped<IBookRepository, BookRepository>();
		services.AddScoped<ICategoryRepository, CategoryRepository>();
	}

	private static void RegisterMapper(this IServiceCollection services)
	{
		// services.AddMapster();
	}
}
