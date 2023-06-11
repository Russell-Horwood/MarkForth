using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace MarkForth.Extensions.Logging;

public static class LoggerExtensions
{

    #region LogEmptyStream.

    internal static void LogEmptyStream(this ILogger logger, string name)
    {
        logEmptyStream(logger, name, null);
    }

    private static readonly Action<ILogger, string, Exception?> logEmptyStream = LoggerMessage.Define<string>
    (
        LogLevel.Warning,
        new EventId(1, nameof(LogEmptyStream)),
        "The {Name} stream has zero length"
    );

    #endregion LogEmptyStream.

    #region LogError.

    private static readonly ConcurrentDictionary<int, Action<ILogger, string, Exception>> actions =
        new ConcurrentDictionary<int, Action<ILogger, string, Exception>>();

    public static string LogError(this ILogger logger, Exception exception)
    {
        string logEntryId = Guid.NewGuid().ToString();

        actions.GetOrAdd
        (
            exception.HResult,
            hResult => LoggerMessage.Define<string>
            (
                LogLevel.Error,
                new EventId(exception.HResult, exception.GetType().Name),
                "{Message}"
            )
        )
        (logger, $"Log entry id: {logEntryId}", exception);

        return logEntryId;
    }

    #endregion LogError.

}
