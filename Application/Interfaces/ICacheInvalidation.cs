namespace Application.Interfaces;

/**
 *		Indicates that command modifies data and requires one or more cache entries to be removed.
 */
public interface ICacheInvalidation
{
	IEnumerable<string> CacheKeys { get; }
}