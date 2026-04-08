namespace Application.Interfaces;

/**
 *		Indicates that query should be cached.
 */
public interface ICachableQuery
{
	bool BypassCache { get; }
	string CacheKey { get; }
	TimeSpan SlidingExpiration { get; }
}