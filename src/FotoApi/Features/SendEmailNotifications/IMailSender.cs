namespace FotoApi.Features.SendEmailNotifications;

public interface IMailSender
{
    Task SendEmailAsync(MailInfo mailInfo, CancellationToken ct);
}

internal interface IMailQueue
{
    Task AddToQueueAsync(MailInfo mailInfo, CancellationToken ct);
    bool HasItemsInQueue();
    Task<MailInfo> GetNextFromQueueAsync(CancellationToken ct);
}

public record MailInfo(string Email, string Subject, string Message);