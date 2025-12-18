using Microsoft.Extensions.Logging;

namespace OnOff.Api.Infrastructure.Logging;

public class FileLogger : ILogger
{
    private readonly string _filePath;

    public FileLogger(string filePath)
    {
        _filePath = filePath;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null!;
    }

    public bool IsEnabled(LogLevel logLevel)
        => logLevel >= LogLevel.Information;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} " +
                  $"[{logLevel}] {formatter(state, exception)}";

        if (exception != null)
        {
            log += Environment.NewLine + exception;
        }

        log += Environment.NewLine;

        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        File.AppendAllText(_filePath, log);
    }


}
