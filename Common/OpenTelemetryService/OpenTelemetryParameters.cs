namespace Common.OpenTelemetryService;

public class OpenTelemetryParameters
{
    public string ServiceName { get; init; }
    public string ServiceVersion { get; init; }
    public string ActivitySourceName { get; init; }
    public string Endpoint { get; init; }
    public bool IsUseConsole { get; init; }
}