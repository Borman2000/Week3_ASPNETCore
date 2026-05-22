using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
	private readonly ICacheService _cacheService;
	private readonly ILogger _logger;

	public CachingBehavior(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
	{
		_cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (request is ICachableQuery cacheableQuery)
		{
			if (cacheableQuery.BypassCache) return await next();

			TResponse response;

			async Task<TResponse> GetResponseAddedToCache()
			{
				response = await next();
				_logger.LogInformation("Cached response: {Response}", response);
				await _cacheService.SetAsync(cacheableQuery.CacheKey.ToLower(), response, cacheableQuery.SlidingExpiration);
				return response;
			}

			var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheableQuery.CacheKey.ToLower());
			if (!EqualityComparer<TResponse>.Default.Equals(cachedResponse, default)) {
				_logger.LogInformation("Cache hit for key: {CacheKey}", cacheableQuery.CacheKey);
				response = cachedResponse!;
			}
			else
			{
				response = await GetResponseAddedToCache();
			}

			return response;
		}

		return await next();
	}
}