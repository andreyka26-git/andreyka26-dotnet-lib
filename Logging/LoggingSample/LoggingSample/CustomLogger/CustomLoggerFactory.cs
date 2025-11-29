using LoggingSample.CustomLogger.Decorators;
using LoggingSample.CustomLogger.Enums;
using LoggingSample.CustomLogger.Interfaces;
using LoggingSample.CustomLogger.Options;
using LoggingSample.CustomLogger.Services;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace LoggingSample.CustomLogger;

public class CustomLoggerFactory : ICustomLoggerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _config;
    private bool _useTelegram;
    private bool _useEmail;

    public CustomLoggerFactory(IServiceProvider serviceProvider, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _config = config;
        _useTelegram = config.GetValue<bool>("UseTelegramLogger");
        _useEmail = config.GetValue<bool>("UseEmailLogger");
    }

    public ICustomLogger CreateLogger(LoggerType type)
    {
        ILoggerDecorator decorator = null;

        if (type.HasFlag(LoggerType.Console))
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<ConsoleLoggerDecorator>>();
            decorator = new ConsoleLoggerDecorator(logger, decorator);
        }

        if (type.HasFlag(LoggerType.Telegram))
        {
            if (_useTelegram)
            {
                var consoleLogger = _serviceProvider.GetRequiredService<ILogger<TelegramLoggerDecorator>>();

                var telegramBotClient = _serviceProvider.GetRequiredService<ITelegramBotClient>();
                var errorLogger = _serviceProvider.GetRequiredService<ICustomLoggerFactory>().CreateLogger(LoggerType.Console);

                var telegramLoggerService = new TelegramLoggerService(telegramBotClient, _config, errorLogger);

                decorator = new TelegramLoggerDecorator(telegramLoggerService, consoleLogger, decorator);
            }
        }

        if (type.HasFlag(LoggerType.Email))
        {
            if (_useEmail)
            {
                var consoleLogger = _serviceProvider.GetRequiredService<ILogger<EmailLoggerDecorator>>();

                var errorLogger = _serviceProvider.GetRequiredService<ICustomLoggerFactory>().CreateLogger(LoggerType.Console | LoggerType.Telegram);
                var emailOptions = _serviceProvider.GetRequiredService<IOptions<EmailSenderOptions>>();

                var emailLoggerService = new EmailLoggerService(emailOptions, errorLogger);

                decorator = new EmailLoggerDecorator(emailLoggerService, consoleLogger, decorator);
            }
        }

        var customLogger = new CustomLogger(decorator);
        return customLogger;
    }
}
