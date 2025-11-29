using LoggingSample.CustomLogger.Enums;
using LoggingSample.CustomLogger.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LoggingSample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DummyController : ControllerBase
{
    private readonly ICustomLoggerFactory _loggerFactory;

    public DummyController(ICustomLoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    [HttpGet("custom-logger")]
    public IActionResult TestCustomLoggers()
    {
        // Test Console only logger
        var consoleLogger = _loggerFactory.CreateLogger(LoggerType.Console);
        consoleLogger.LogInformation("This is a Console-only logger test message");

        // Test Telegram only logger
        var telegramLogger = _loggerFactory.CreateLogger(LoggerType.Telegram);
        telegramLogger.LogInformation("This is a Telegram-only logger test message");

        // Test Console and Email logger
        var consoleEmailLogger = _loggerFactory.CreateLogger(LoggerType.Console | LoggerType.Email);
        consoleEmailLogger.LogInformation("This is a Console and Email logger test message");

        return Ok(new
        {
            message = "Custom loggers tested successfully",
            loggers = new[]
            {
                "Console-only logger",
                "Telegram-only logger",
                "Console and Email logger"
            }
        });
    }
}
