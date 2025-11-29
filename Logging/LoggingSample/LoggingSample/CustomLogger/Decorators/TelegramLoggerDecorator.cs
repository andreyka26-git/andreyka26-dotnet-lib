using LoggingSample.CustomLogger.Interfaces;
using LoggingSample.CustomLogger.Services;

namespace LoggingSample.CustomLogger.Decorators;

public class TelegramLoggerDecorator : LoggerDecoratorBase
{
    private readonly TelegramLoggerService _logger;
    private readonly ILogger<TelegramLoggerDecorator> _consoleLogger;

    public TelegramLoggerDecorator(
        TelegramLoggerService logger,
        ILogger<TelegramLoggerDecorator> consoleLogger,
        ILoggerDecorator decorator = null) : base(decorator)
    {
        _consoleLogger = consoleLogger;
        _logger = logger;
    }

    protected override void LogInformationImpl(string message)
    {
        try
        {
            _logger.LogAsync(message).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _consoleLogger.LogError($"Cannot log using telegram: {ex.Message}");
        }
    }

    protected override void LogErrorImpl(string message)
    {
        try
        {
            _logger.LogAsync(message).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _consoleLogger.LogError($"Cannot log using telegram: {ex.Message}");
        }
    }

    protected override void LogCriticalImpl(string message)
    {
        try
        {
            _logger.LogAsync(message).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _consoleLogger.LogError($"Cannot log using telegram: {ex.Message}");
        }
    }
}
