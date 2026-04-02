namespace NotificationAPI.Infrastructure;

public interface IRateLimiter
{
	Task<bool> TryAcquireAsync(string key, int maxRequests, TimeSpan window);
}