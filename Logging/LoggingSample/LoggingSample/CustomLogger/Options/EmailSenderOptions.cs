namespace LoggingSample.CustomLogger.Options;

public class EmailSenderOptions
{
    public string SmtpLogin { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
}
