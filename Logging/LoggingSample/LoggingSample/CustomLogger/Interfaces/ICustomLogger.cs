namespace LoggingSample.CustomLogger.Interfaces;

public interface ICustomLogger
{
    void LogInformation(string message);
    void LogError(string message);
    void LogCritical(string message);
}
