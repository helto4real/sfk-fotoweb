using System.Threading.Channels;

namespace FotoApi.Features.SendEmailNotifications;

internal class MailSender : IMailSender, IMailQueue
{
    private readonly ILogger<MailSender> _logger;
    private readonly Channel<MailInfo> _mailQueue = Channel.CreateUnbounded<MailInfo>();

    public MailSender(ILogger<MailSender> logger)
    {
        _logger = logger;
    }

    public async Task AddToQueueAsync(MailInfo mailInfo, CancellationToken ct)
    {
        await _mailQueue.Writer.WriteAsync(mailInfo, ct);
    }

    public bool HasItemsInQueue()
    {
        return _mailQueue.Reader.TryRead(out _);
    }

    public async Task<MailInfo> GetNextFromQueueAsync(CancellationToken ct)
    {
        return await _mailQueue.Reader.ReadAsync(ct);
    }

    public Task SendEmailAsync(MailInfo mailInfo, CancellationToken ct)
    {
        return AddToQueueAsync(mailInfo, ct);
    }
}