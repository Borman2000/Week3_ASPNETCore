using AuthAPI.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.Gateway.Configs;
using Yarp.Gateway.Extensions;
using Yarp.ReverseProxy.Swagger;
using Yarp.ReverseProxy.Swagger.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

builder.Services.AddJwtData(builder);

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("authenticated", policy =>
		policy.RequireAuthenticatedUser());
});

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
	rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
	{
		options.Window = TimeSpan.FromSeconds(10);
		options.PermitLimit = 5;
	});
});

var proxyConf = builder.Configuration.GetSection("ReverseProxy");
builder.Services
	.AddReverseProxy()
	.AddSwagger(proxyConf)
	.LoadFromConfig(proxyConf);

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
	app.UseSwagger(c => {
//		c.PreSerializeFilters.Add((swaggerDoc, httpReq) => {
//			swaggerDoc.Servers = new List<OpenApiServer> {
//				new OpenApiServer { Url = "https://localhost" }
//			};
//		});
	});
	app.UseSwaggerUI(options =>
	{
		options.DocumentTitle = "YARP Gateway";
		var config = app.Services.GetRequiredService<IOptionsMonitor<ReverseProxyDocumentFilterConfig>>().CurrentValue;
		options.ConfigureSwaggerEndpoints(config);
	});
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapReverseProxy();

app.Run();