using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Middleware;

public class ValidationExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;

	public ValidationExceptionHandlingMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (ValidationException exception)
		{
			var problemDetails = new ValidationProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Type = "ValidationFailure",
				Title = "Validation error",
				Detail = "One or more validation errors has occurred"
			};

			foreach (var error in exception.Errors)
				problemDetails.Errors[error.Key] = error.Value;

			context.Response.StatusCode = StatusCodes.Status400BadRequest;

			await context.Response.WriteAsJsonAsync(problemDetails);
		}
	}
}