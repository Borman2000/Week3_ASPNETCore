using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class CachedAuthorRepository : IAuthorRepository
{
	private readonly IAuthorRepository _inner;
	private readonly ICacheService _cache;

	public CachedAuthorRepository(IAuthorRepository inner, ICacheService cache)
	{
		_inner = inner;
		_cache = cache;
	}

	public async Task<IEnumerable<Author>> GetAllAsync()
	{
		return await _inner.GetAllAsync();
	}

	public async Task<Author?> GetByIdAsync(Guid id)
	{
		string cacheKey = $"Author_{id}";
		Author? result = await _cache.GetAsync<Author>(cacheKey);
		if (result != null)
		{
			return result; // Return from cache
		}

		result = await _inner.GetByIdAsync(id); // Fetch from DB

		// Store in cache with an expiration policy
		await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

		return result;
	}

	public async Task<Author?> AddAsync(Author entity)
	{
		return await _inner.AddAsync(entity);
	}

	public async Task AddBulkAsync(IEnumerable<Author> entities)
	{
		await _inner.AddBulkAsync(entities);
	}

	public async Task UpdateAsync(Author entity)
	{
		await _cache.RemoveAsync($"Author_{entity.Id}");
		await _cache.RemoveAsync($"Author_Books_{entity.Id}");
		await _inner.UpdateAsync(entity);
	}

	public async Task DeleteAsync(Guid id)
	{
		await _cache.RemoveAsync($"Author_{id}");
		await _cache.RemoveAsync($"Author_Books_{id}");
		await _inner.DeleteAsync(id);
	}

	public IQueryable<Author> Query()
	{
		throw new NotImplementedException();
	}

	public async Task<AuthorDto?> GetByIdWithBooksAsync(Guid id)
	{
		string cacheKey = $"Author_Books_{id}";
		AuthorDto? result = await _cache.GetAsync<AuthorDto>(cacheKey);
		if (result != null)
		{
			return result; // Return from cache
		}

		result = await _inner.GetByIdWithBooksAsync(id); // Fetch from DB

		// Store in cache with an expiration policy
		await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

		return result;
	}
}