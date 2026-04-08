using System.Text;
using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.Services;

public class RedisCacheService : ICacheService
{
	private readonly IDistributedCache _cache;

	public RedisCacheService(IDistributedCache cache)
	{
		_cache = cache ?? throw new ArgumentNullException(nameof(cache));
	}

	public async Task<T?> GetAsync<T>(string key)
	{
		var cachedResponse = await _cache.GetAsync(key.ToLower());
		if (cachedResponse != null)
		{
			return JsonConvert.DeserializeObject<T>(Encoding.Default.GetString(cachedResponse));
		}
		return default;
	}

	public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
	{
		var serializedData = Encoding.Default.GetBytes(JsonConvert.SerializeObject(value));

		await _cache.SetAsync(key.ToLower(), serializedData);
	}

	public async Task RemoveAsync(string key)
	{
		await _cache.RemoveAsync(key.ToLower());
	}
}