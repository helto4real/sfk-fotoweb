using System.Diagnostics.CodeAnalysis;

namespace FotoApi.Features.SendEmailNotifications;

public interface IMailSender
{
    Task SendEmailAsync(MailInfo mailInfo, CancellationToken ct);
}

internal interface IMailQueue
{
    Task<MailInfo> GetNextFromQueueAsync(CancellationToken ct);
    bool TryRead([MaybeNullWhen(false)] out MailInfo nextEmailMessage);
}

public record MailInfo(string Email, string Subject, string Message);