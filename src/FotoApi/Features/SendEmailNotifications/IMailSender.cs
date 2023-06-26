namespace FotoApi.Features.SendEmailNotifications;

internal interface IMailSender
{
    Task SendEmailAsync(MailInfo mailInfo);
}

internal interface IMailQueue
{
    Task AddToQueue(MailInfo mailInfo);
    bool HasItemsInQueue();
    Task<MailInfo> GetNextFromQueue();
}

internal record MailInfo(string Email, string Subject, string Message);