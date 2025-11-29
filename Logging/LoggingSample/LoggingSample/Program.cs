using LoggingSample.CustomLogger;
using LoggingSample.CustomLogger.Interfaces;
using LoggingSample.CustomLogger.Options;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure Email Options
builder.Services.Configure<EmailSenderOptions>(
    builder.Configuration.GetSection("EmailSenderOptions"));

// Register Telegram Bot Client
var telegramBotToken = builder.Configuration.GetValue<string>("TelegramLoggerSettings:BotToken");
if (!string.IsNullOrEmpty(telegramBotToken))
{
    builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(telegramBotToken));
}

// Register Custom Logger Factory
builder.Services.AddSingleton<ICustomLoggerFactory, CustomLoggerFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
