using LoggingSample.CustomLogger.Enums;

namespace LoggingSample.CustomLogger.Interfaces;

public interface ICustomLoggerFactory
{
    ICustomLogger CreateLogger(LoggerType type);
}
