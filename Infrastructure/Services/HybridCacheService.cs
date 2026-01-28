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
		throw new NotImplementedException();
	}

	public Task SetAsync<T>(string key, T value, TimeSpan expiration)
	{
		throw new NotImplementedException();
	}

	public Task RemoveAsync(string key)
	{
		throw new NotImplementedException();
	}
}