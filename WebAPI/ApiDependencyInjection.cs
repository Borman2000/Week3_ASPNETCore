using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace WebAPI;

public static class ApiDependencyInjection
{
    public static void AddSwagger(this IServiceCollection services)
    {
	    services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(s =>
        {
	        var provider = services.BuildServiceProvider()
		        .GetRequiredService<IApiVersionDescriptionProvider>();
	        foreach (var desc in provider.ApiVersionDescriptions)
	        {
		        s.SwaggerDoc(desc.GroupName, new OpenApiInfo
		        {
			        Title = "Book API",
			        Version = desc.ApiVersion.ToString(),
			        Description = desc.IsDeprecated ? "Deprecated" : "Stable"
		        });
	        }

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
    }

    public static void AddVersioning(this IServiceCollection services)
    {
	    services.AddApiVersioning(options =>
		    {
			    options.DefaultApiVersion = new ApiVersion(1,0);
			    options.AssumeDefaultVersionWhenUnspecified = true;
			    options.ReportApiVersions = true;
			    options.ApiVersionReader = ApiVersionReader.Combine(
				    new UrlSegmentApiVersionReader(),
				    new QueryStringApiVersionReader(),
				    new HeaderApiVersionReader("X-Api-Version"));
		    })
		    .AddApiExplorer(options =>
		    {
			    options.GroupNameFormat = "'v'VVV";
			    options.SubstituteApiVersionInUrl = true;
		    });
    }
}
