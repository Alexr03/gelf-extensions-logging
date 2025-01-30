using System;

namespace Gelf.Extensions.Logging;

public static class Utilities
{
    public static double GetTimestamp(DateTimeOffset? dateTime = null)
    {
        dateTime ??= DateTimeOffset.UtcNow;
        var totalMilliseconds = dateTime.Value.ToUnixTimeMilliseconds();
        var totalSeconds = totalMilliseconds / 1000d;
        return Math.Round(totalSeconds, 3);
    }

    public static long GetTimestampMilliseconds(DateTimeOffset? dateTime = null)
    {
        dateTime ??= DateTimeOffset.UtcNow;
        return dateTime.Value.ToUnixTimeMilliseconds();
    }
}
