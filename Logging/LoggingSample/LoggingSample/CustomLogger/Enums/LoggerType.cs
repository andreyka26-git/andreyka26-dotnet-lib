namespace LoggingSample.CustomLogger.Enums;

[Flags]
public enum LoggerType
{
    None = 0,
    Console = 1,
    Telegram = 2,
    Email = 4
}
