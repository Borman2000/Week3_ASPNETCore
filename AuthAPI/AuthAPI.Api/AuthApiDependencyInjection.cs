using System.Text.Json.Serialization;
using AuthAPI.Application.Mappings;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AuthAPI.Api;

public static class AuthApiDependencyInjection
{
	public static IServiceCollection AddGeneralServices(this IServiceCollection services)
	{
		AddSwagger(services);
		AddMapper(services);
		return services;
	}

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
	    services.AddEndpointsApiExplorer();
	    services.AddSwaggerGen(s =>
	    {
		    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		    {
			    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer YOUR_TOKEN')",
			    Name = "Authorization",
			    In = ParameterLocation.Header,
			    Type = SecuritySchemeType.ApiKey,
			    Scheme = "Bearer"
		    });

		    s.AddSecurityRequirement(new OpenApiSecurityRequirement
		    {
			    {
				    new OpenApiSecurityScheme
				    {
					    Reference = new OpenApiReference
					    {
						    Type = ReferenceType.SecurityScheme,
						    Id = "Bearer"
					    }
				    },
				    Array.Empty<string>()
			    }
		    });
	    });

        return services;
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
		    config.AddProfile<UserMappingProfile>();
	    }, loggerFactory);

	    IMapper mapper = mapConf.CreateMapper();
	    services.AddSingleton(mapper);

	    services.AddControllers().AddJsonOptions(options =>
	    {
		    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	    });
    }
}
