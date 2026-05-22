using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services;

// Build-in Hybrid cache exists in .NET 9+. We're using .NET 8
public class HybridCacheService : ICacheService
{
	private readonly IDistributedCache _cache;

	public HybridCacheService(IDistributedCache cache)
	{
		_cache = cache ?? throw new ArgumentNullException(nameof(cache));
	}

	public Task<T?> GetAsync<T>(string key)
	{
		_cache.GetStringAsync(key); // This is just to avoid "unused field" warning. The actual implementation will be added in .NET 9+ when we upgrade.
		throw new NotImplementedException();
	}

	public Task SetAsync<T>(string key, T value, TimeSpan expiration)
	{
		_cache.SetStringAsync(key, value?.ToString() ?? "default", new DistributedCacheEntryOptions
		{
			SlidingExpiration = expiration
		}); // This is just to avoid "unused field" warning. The actual implementation will be added in .NET 9+ when we upgrade.
		throw new NotImplementedException();
	}

	public Task RemoveAsync(string key)
	{
		_cache.RemoveAsync(key); // This is just to avoid "unused field" warning. The actual implementation will be added in .NET 9+ when we upgrade.
		throw new NotImplementedException();
	}
}