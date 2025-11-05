using System.Reflection;
using System.Text.Json.Serialization;
using Application.Behaviors;
using Application.Mappings;
using Application.Models;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application;

public static class ApplicationDependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddProblemDetails();
		services.AddServices(configuration);

		services.RegisterMapper();
		services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

		services.AddMediatR(cfg => {
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
			cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
//			cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
//			cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
			cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
			cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
		});

		return services;
	}

	private static void AddServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddEndpointsApiExplorer();
		services.AddOptions<ApiSettings>().Bind(configuration.GetSection(ApiSettings.Section)).ValidateDataAnnotations().ValidateOnStart();
	}

	private static void RegisterMapper(this IServiceCollection services)
	{
//including the automapper via dependency injection
		var loggerFactory = LoggerFactory.Create(bld =>
		{
			bld.AddConsole();
			bld.SetMinimumLevel(LogLevel.Information); // Adjust log level as needed
		});

		var mapConf = new MapperConfiguration(config =>
		{
			config.AddProfile<BookMappingProfile>();
		}, loggerFactory);

		IMapper mapper = mapConf.CreateMapper();
		services.AddSingleton(mapper);
		services.AddAutoMapper(cfg => mapConf.CreateMapper(), typeof(BookMappingProfile).Assembly);

		services.AddControllers().AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
		});
	}
}
