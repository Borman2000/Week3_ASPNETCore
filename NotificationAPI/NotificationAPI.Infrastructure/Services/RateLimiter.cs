using System.Collections.Concurrent;

namespace NotificationAPI.Infrastructure.Services;

public class RateLimiter : IRateLimiter
{
	private readonly ConcurrentDictionary<string, SlidingWindow> _windows = new();

	public Task<bool> TryAcquireAsync(string key, int maxRequests, TimeSpan window)
	{
		var slidingWindow = _windows.GetOrAdd(key, _ => new SlidingWindow(window));
		return Task.FromResult(slidingWindow.TryAcquire(maxRequests));
	}

	private class SlidingWindow
	{
		private readonly TimeSpan _windowSize;
		private readonly Queue<DateTime> _timestamps = new();
		private readonly object _lock = new();

		public SlidingWindow(TimeSpan windowSize)
		{
			_windowSize = windowSize;
		}

		public bool TryAcquire(int maxRequests)
		{
			lock (_lock)
			{
				var now = DateTime.UtcNow;
				var windowStart = now - _windowSize;

				// Remove expired timestamps
				while (_timestamps.Count > 0 && _timestamps.Peek() < windowStart)
				{
					_timestamps.Dequeue();
				}

				if (_timestamps.Count >= maxRequests)
				{
					return false;
				}

				_timestamps.Enqueue(now);
				return true;
			}
		}
	}
}