using LoggingSample.CustomLogger.Interfaces;

namespace LoggingSample.CustomLogger.Decorators;

public abstract class LoggerDecoratorBase : ILoggerDecorator
{
    private readonly ILoggerDecorator _decorator;

    protected LoggerDecoratorBase(ILoggerDecorator decorator)
    {
        _decorator = decorator;
    }

    public void LogInformation(string message)
    {
        LogInformationImpl(message);
        _decorator?.LogInformation(message);
    }

    public void LogError(string message)
    {
        LogErrorImpl(message);
        _decorator?.LogError(message);
    }

    public void LogCritical(string message)
    {
        LogCriticalImpl(message);
        _decorator?.LogCritical(message);
    }

    protected abstract void LogInformationImpl(string message);
    protected abstract void LogErrorImpl(string message);
    protected abstract void LogCriticalImpl(string message);
}
