using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
	private readonly ICacheService _cache;
	private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

	public CacheInvalidationBehavior(ICacheService cache, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
	{
		_cache = cache ?? throw new ArgumentNullException(nameof(cache));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (request is ICacheInvalidation invalidationCommand)
		{
			foreach (var cacheKey in invalidationCommand.CacheKeys)
			{
				_logger.LogInformation("Invalidating cache for key: {CacheKey}", cacheKey);
				await _cache.RemoveAsync(cacheKey.ToLower());
			}
		}

		return await next();
	}
}