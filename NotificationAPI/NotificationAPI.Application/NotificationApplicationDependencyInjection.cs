using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationAPI.Application.Mappings;
using NotificationAPI.Application.Models;

namespace NotificationAPI.Application;

public static class NotificationApplicationDependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddServices(configuration);

		services.AddMapper();

		return services;
	}

	private static void AddServices(this IServiceCollection services, IConfiguration configuration)
	{
		var tmp = configuration.GetSection(EmailSettings.Section);
		services.AddOptions<EmailSettings>().Bind(configuration.GetSection(EmailSettings.Section)).ValidateDataAnnotations().ValidateOnStart();
	}

	private static void AddMapper(this IServiceCollection services)
	{
//including the automapper via dependency injection
		var loggerFactory = LoggerFactory.Create(bld =>
		{
			bld.AddConsole();
			bld.SetMinimumLevel(LogLevel.Information); // Adjust log level as needed
		});

		var mapConf = new MapperConfiguration(config =>
		{
			config.AddProfile<NotificationMappingProfile>();
		}, loggerFactory);

		IMapper mapper = mapConf.CreateMapper();
		services.AddSingleton(mapper);
	}
}
