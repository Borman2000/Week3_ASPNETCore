using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Application.Behaviors;

internal sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        string requestName = typeof(TRequest).Name;

// TODO: Log with CorrelationId?
        logger.LogInformation("Processing request {RequestName}", requestName);
		Log.Information($"Processing request {requestName}");

		TResponse result = await next();

		logger.LogInformation("Completed request {RequestName}", requestName);
		Log.Information($"Completed request {requestName}");

        return result;
    }
}
