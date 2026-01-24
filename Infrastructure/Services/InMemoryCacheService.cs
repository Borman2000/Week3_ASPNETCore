using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class InMemoryCacheService : ICacheService
{
	private readonly IMemoryCache _cache;

	public InMemoryCacheService(IMemoryCache cache)
	{
		_cache = cache ?? throw new ArgumentNullException(nameof(cache));
	}

	public Task<T?> GetAsync<T>(string key)
	{
		_cache.TryGetValue(key, out T? value);
		return Task.FromResult(value);
	}

	public Task SetAsync<T>(string key, T value, TimeSpan expiration)
	{
		_cache.Set(key, value, expiration);
		return Task.CompletedTask;
	}

	public Task RemoveAsync(string key)
	{
		_cache.Remove(key);
		return Task.CompletedTask;
	}
}