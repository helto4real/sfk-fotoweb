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

    public async Task AddToQueue(MailInfo mailInfo)
    {
        await _mailQueue.Writer.WriteAsync(mailInfo);
    }

    public bool HasItemsInQueue()
    {
        return _mailQueue.Reader.TryRead(out _);
    }

    public async Task<MailInfo> GetNextFromQueue()
    {
        return await _mailQueue.Reader.ReadAsync();
    }

    public Task SendEmailAsync(MailInfo mailInfo)
    {
        return AddToQueue(mailInfo);
    }
}