using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Middleware;

public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
{
	public async Task Invoke(HttpContext context)
	{
		try
		{
			await next(context);
		}
		catch (Exception error)
		{
			var response = context.Response;
			response.ContentType = "application/json";
			response.StatusCode = StatusCodes.Status500InternalServerError;
			var problemDetails = new ProblemDetails
			{
				Status = response.StatusCode,
				Title = error.Message,
			};
			logger.LogError(error.Message);
			var result = JsonSerializer.Serialize(problemDetails);
			await response.WriteAsync(result);
		}
	}
}

public static class ErrorHandlerMiddlewareExtensions
{
	public static IApplicationBuilder UseErrorHandlerMiddleware(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<ErrorHandlerMiddleware>();
	}
}