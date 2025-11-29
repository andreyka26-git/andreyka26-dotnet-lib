using LoggingSample.CustomLogger.Interfaces;

namespace LoggingSample.CustomLogger.Decorators;

public class ConsoleLoggerDecorator : LoggerDecoratorBase
{
    private readonly ILogger<ConsoleLoggerDecorator> _logger;

    public ConsoleLoggerDecorator(
        ILogger<ConsoleLoggerDecorator> logger,
        ILoggerDecorator decorator = null) : base(decorator)
    {
        _logger = logger;
    }

    protected override void LogInformationImpl(string message)
    {
        _logger.LogInformation(message);
    }

    protected override void LogErrorImpl(string message)
    {
        _logger.LogError(message);
    }

    protected override void LogCriticalImpl(string message)
    {
        _logger.LogCritical(message);
    }
}
