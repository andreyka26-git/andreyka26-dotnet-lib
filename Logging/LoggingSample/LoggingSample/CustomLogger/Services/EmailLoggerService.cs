using LoggingSample.CustomLogger.Interfaces;
using LoggingSample.CustomLogger.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace LoggingSample.CustomLogger.Services;

public class EmailLoggerService
{
    private readonly EmailSenderOptions _options;
    private readonly ICustomLogger _errorLogger;

    private readonly List<string> supportEmails = new List<string>
    {
        "b.andriy.b2000@gmail.com",
        "svirgun200@gmail.com"
    };

    public EmailLoggerService(IOptions<EmailSenderOptions> options, ICustomLogger errorLogger)
    {
        _options = options?.Value;
        _errorLogger = errorLogger;
    }

    public async Task LogAsync(string message)
    {
        try
        {
            var to = string.Join(",", supportEmails);
            var subject = "EXCEPTION";
            var body = message;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                UseDefaultCredentials = false,
                Port = 587,
                Credentials = new NetworkCredential(_options.SmtpLogin, _options.SmtpPassword),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(_options.SmtpLogin, to, subject, body);
        }
        catch (Exception ex)
        {
            _errorLogger.LogError($"[Email Logger] Cannot send message to email. Error: {ex.Message} Message: {message}");
        }
    }
}
