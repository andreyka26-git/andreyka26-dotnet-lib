namespace LoggingSample.CustomLogger.Interfaces;

public interface ILoggerDecorator
{
    void LogInformation(string message);
    void LogError(string message);
    void LogCritical(string message);
}
