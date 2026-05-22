using System.Diagnostics;

namespace Common.OpenTelemetryService;

public static class ActivitySourceProvider
{
    public static ActivitySource? Source { get; private set; }

    public static ActivitySource SetSource(string sourceName)
    {
        return Source ??= new ActivitySource(sourceName);
    }
}