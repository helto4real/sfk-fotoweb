namespace FotoApi.Features.SendEmailNotifications;

public interface IMailSender
{
    Task SendEmailAsync(MailInfo mailInfo);
}

internal interface IMailQueue
{
    Task AddToQueue(MailInfo mailInfo);
    bool HasItemsInQueue();
    Task<MailInfo> GetNextFromQueue();
}

public record MailInfo(string Email, string Subject, string Message);