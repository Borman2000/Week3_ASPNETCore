using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class PerformanceMetrics : IDisposable
{
	private readonly ILogger<PerformanceMetrics> _logger;
	private readonly Meter _meter;
	private readonly Histogram<double> _responseTimeHistogram;
	private readonly Counter<int> _cacheRequestsCounter;
	private readonly Counter<int> _cacheHitCounter;

//	private readonly IMetrics _metrics;

	public PerformanceMetrics(IMeterFactory meterFactory, ILogger<PerformanceMetrics> logger)
	{
		_logger = logger;

		_meter = meterFactory.Create("Books.api");
		_responseTimeHistogram = _meter.CreateHistogram<double>("books.api.request.duration", unit: "ms", description: "API Response Times");
		_cacheRequestsCounter = _meter.CreateCounter<int>("books.api.cache.counter");
		_cacheHitCounter = _meter.CreateCounter<int>("books.api.cache.hit");
	}

//	public IDisposable MeasureOperation(string operationName)
//	{
//		return null;
//	}

	public void RecordRequestTime(string operationName, string method, int statusCode, double timeImMs, string correlationId)
	{
		string route = operationName;
		if(operationName.IndexOf('/', 1) > 0)
			route = operationName.Substring(0, operationName.IndexOf('/', 1));

		_responseTimeHistogram.Record(timeImMs,
			[new KeyValuePair<string, object?>("http.request.method", method),
			new KeyValuePair<string, object?>("http.route", route),
//			new KeyValuePair<string, object?>("http.request.correlationId", correlationId),
			new KeyValuePair<string, object?>("http.response.status_code", statusCode)
			]);

//		_requestsCounter.Add(1, new KeyValuePair<string, object?>("http.route", operationName));
		_logger.Log(LogLevel.Information, $"Request {operationName} {statusCode}: {timeImMs}ms");
	}

	public void AddCacheRequest()
	{
		_cacheRequestsCounter.Add(1);
	}

	public void AddCacheHit()
	{
		_cacheHitCounter.Add(1);
	}

	public void Dispose() => _meter.Dispose();
}