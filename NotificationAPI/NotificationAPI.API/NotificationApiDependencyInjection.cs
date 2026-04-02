using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace NotificationAPI.API;

public static class NotificationApiDependencyInjection
{
	public static IServiceCollection AddGeneralServices(this IServiceCollection services)
	{
		AddSwagger(services);
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
}
