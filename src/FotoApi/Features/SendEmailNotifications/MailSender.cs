using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace FotoApi.Features.SendEmailNotifications;

internal class MailSender : IMailSender, IMailQueue
{
    private readonly Channel<MailInfo> _mailQueue = Channel.CreateUnbounded<MailInfo>();

    public async Task AddToQueueAsync(MailInfo mailInfo, CancellationToken ct)
    {
        await _mailQueue.Writer.WriteAsync(mailInfo, ct);
    }

    public async Task<MailInfo> GetNextFromQueueAsync(CancellationToken ct)
    {
        return await _mailQueue.Reader.ReadAsync(ct);
    }
    
    public bool TryRead([MaybeNullWhen(false)] out MailInfo nextEmailMessage)
    {
        return _mailQueue.Reader.TryRead(out nextEmailMessage);
    }

    public Task SendEmailAsync(MailInfo mailInfo, CancellationToken ct)
    {
        return AddToQueueAsync(mailInfo, ct);
    }
}