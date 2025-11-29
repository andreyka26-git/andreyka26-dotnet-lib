using LoggingSample.CustomLogger.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LoggingSample.CustomLogger.Services;

public class TelegramLoggerService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ICustomLogger _errorLogger;
    private readonly string _chatId;

    public TelegramLoggerService(ITelegramBotClient telegramBotClient,
        IConfiguration configuration,
        ICustomLogger errorLogger)
    {
        _telegramBotClient = telegramBotClient;
        _chatId = configuration.GetValue<string>("TelegramLoggerSettings:ChatId");
        _errorLogger = errorLogger;
    }

    public async Task LogAsync(string message)
    {
        try
        {
            await _telegramBotClient.SendMessage(new ChatId(_chatId), message);
        }
        catch (Exception ex)
        {
            _errorLogger.LogError($"[Telegram Logger] Cannot send message to telegram. Error: {ex.Message} Message: {message}");
        }
    }
}
