using LoggingSample.CustomLogger.Interfaces;
using LoggingSample.CustomLogger.Services;

namespace LoggingSample.CustomLogger.Decorators;

public class EmailLoggerDecorator : LoggerDecoratorBase
{
    private readonly EmailLoggerService _logger;
    private readonly ILogger<EmailLoggerDecorator> _consoleLogger;

    public EmailLoggerDecorator(EmailLoggerService logger,
        ILogger<EmailLoggerDecorator> consoleLogger,
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
            _consoleLogger.LogError($"Cannot log using email logger: {ex.Message}");
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
            _consoleLogger.LogError($"Cannot log using email logger: {ex.Message}");
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
            _consoleLogger.LogError($"Cannot log using email logger: {ex.Message}");
        }
    }
}
