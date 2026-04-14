using System.Text;
using AuthAPI.Application.Interfaces;
using AuthAPI.Domain;
using AuthAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthAPI.Infrastructure;

public static class JwtHelper
{
	public static IServiceCollection AddJwtData(this IServiceCollection services, WebApplicationBuilder builder)
	{
		builder.Configuration.AddJsonFile("JwtConfig.json", optional: false);

		services.AddJwtAuthentication(builder.Configuration);
		services.AddClaimsAuthorization();

		return services;
	}

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
// Configure JWT settings
		IConfigurationSection jwtConfig = configuration.GetSection(nameof(JwtSettings));
		JwtSettings jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;
	    services.Configure<JwtSettings>(jwtConfig);
	    services.AddOptions<JwtSettings>().Bind(jwtConfig);

	    services.AddAuthentication(options =>
		    {
			    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		    })
		    .AddJwtBearer(options =>
		    {
			    options.TokenValidationParameters = new TokenValidationParameters
			    {
				    ValidateIssuer = true,
				    ValidateAudience = true,
				    ValidateLifetime = true,
				    ValidateIssuerSigningKey = true,
				    ValidIssuer = jwtSettings.Issuer,
				    ValidAudience = jwtSettings.Audience,
				    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
//				    ValidIssuer = configuration["JwtSettings:Issuer"],
//				    ValidAudience = configuration["JwtSettings:Audience"],
//				    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
			    };
		    });

	    services.AddSingleton<ITokenGenerator>(new TokenGenerator(jwtSettings.SecretKey, jwtSettings.Issuer, jwtSettings.Audience, jwtSettings.AccessTokenExpirationMinutes));

	    return services;
    }

    private static IServiceCollection AddClaimsAuthorization(this IServiceCollection services)
    {
	    services.AddAuthorization(options =>
	    {
		    options.AddPolicy(ClaimType.Users.Create, policy => policy.RequireClaim(ClaimType.Users.Create, "true"));
		    options.AddPolicy(ClaimType.Users.Read, policy => policy.RequireClaim(ClaimType.Users.Read));
		    options.AddPolicy(ClaimType.Users.Update, policy => policy.RequireClaim(ClaimType.Users.Update));
		    options.AddPolicy(ClaimType.Users.Delete, policy => policy.RequireClaim(ClaimType.Users.Delete));

		    options.AddPolicy(ClaimType.Authors.Create, policy => policy.RequireClaim(ClaimType.Authors.Create, "true"));
		    options.AddPolicy(ClaimType.Authors.Read, policy => policy.RequireClaim(ClaimType.Authors.Read));
		    options.AddPolicy(ClaimType.Authors.Update, policy => policy.RequireClaim(ClaimType.Authors.Update));
		    options.AddPolicy(ClaimType.Authors.Delete, policy => policy.RequireClaim(ClaimType.Authors.Delete));

		    options.AddPolicy(ClaimType.Books.Create, policy => policy.RequireClaim(ClaimType.Books.Create, "true"));
		    options.AddPolicy(ClaimType.Books.Read, policy => policy.RequireClaim(ClaimType.Books.Read));
		    options.AddPolicy(ClaimType.Books.Update, policy => policy.RequireClaim(ClaimType.Books.Update));
		    options.AddPolicy(ClaimType.Books.Delete, policy => policy.RequireClaim(ClaimType.Books.Delete));

		    options.AddPolicy(ClaimType.Categories.Create, policy => policy.RequireClaim(ClaimType.Categories.Create, "true"));
		    options.AddPolicy(ClaimType.Categories.Read, policy => policy.RequireClaim(ClaimType.Categories.Read));
		    options.AddPolicy(ClaimType.Categories.Update, policy => policy.RequireClaim(ClaimType.Categories.Update));
		    options.AddPolicy(ClaimType.Categories.Delete, policy => policy.RequireClaim(ClaimType.Categories.Delete));
	    });

	    return services;
    }
}