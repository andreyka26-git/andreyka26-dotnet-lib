using LoggingSample.CustomLogger.Interfaces;

namespace LoggingSample.CustomLogger;

public class CustomLogger : ICustomLogger
{
    private readonly ILoggerDecorator _logger;

    public CustomLogger(ILoggerDecorator loggerDecorator)
    {
        _logger = loggerDecorator;
    }

    public void LogInformation(string message)
    {
        _logger.LogInformation($"[INFO] Message:{Environment.NewLine}{message} Log Message Id: {Guid.NewGuid()}");
    }

    public void LogError(string message)
    {
        _logger.LogError($"[ERROR] Message:{Environment.NewLine}{message} Log Message Id: {Guid.NewGuid()}");
    }

    public void LogCritical(string message)
    {
        _logger.LogCritical($"[CRITICAL] Message:{Environment.NewLine}{message} Log Message Id: {Guid.NewGuid()}");
    }
}
