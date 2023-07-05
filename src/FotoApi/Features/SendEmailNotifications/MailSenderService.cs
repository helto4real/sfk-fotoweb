using FotoApi.Infrastructure.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FotoApi.Features.SendEmailNotifications;
public interface IMailSenderService : IHostedService { }
internal class MailSenderService : IMailSenderService
{
    private readonly EmailSettings _emailSettings;

    private readonly ILogger<MailSenderService> _logger;

    private readonly IMailQueue _mailQueue;
    private Task? _backgroundTask;
    private readonly CancellationTokenSource _cancelSource = new();
    private CancellationToken _combinedCancellationToken;
    private SmtpClient? _smtpClient;

    public MailSenderService(ILogger<MailSenderService> logger, IMailQueue mailQueue,
        IOptions<EmailSettings> emailSettings)
    {
        _logger = logger;
        _mailQueue = mailQueue;
        _emailSettings = emailSettings.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _combinedCancellationToken = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken, _cancelSource.Token).Token;

        _logger.LogInformation("Starting MailSenderService");
        _backgroundTask = Task.Run(DoWork, _combinedCancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Exit MailSenderService");
        if (!_cancelSource.IsCancellationRequested)
            await _cancelSource.CancelAsync();

        if (_backgroundTask is not null)
            await _backgroundTask;

        await DisconnectSmtpServer();
    }

    private async Task DoWork()
    {
        try
        {
            while (!_combinedCancellationToken.IsCancellationRequested)
            {
                var nextEmailMessage = await _mailQueue.GetNextFromQueue();

                await ConnectToSmtpServer();

                await SendEmail(nextEmailMessage);
                // Read all queued messages and send them
                while (_mailQueue.HasItemsInQueue() && !_combinedCancellationToken.IsCancellationRequested)
                    await SendEmail(nextEmailMessage);

                await DisconnectSmtpServer();
            }
        }
        catch (TaskCanceledException)
        {
            // No action needed            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in expired tokens manager");
        }
    }

    private async Task ConnectToSmtpServer()
    {
        _logger.LogInformation("Connecting to SMTP server");
        if (_smtpClient is not null) _smtpClient.Dispose();
        _smtpClient = new SmtpClient();
        await _smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.UseSsl,
            _combinedCancellationToken);

        if (!string.IsNullOrEmpty(_emailSettings.SmtpUsername))
            // Note: only needed if the SMTP server requires authentication
            await _smtpClient.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword,
                _combinedCancellationToken);
    }

    private async Task DisconnectSmtpServer()
    {
        _logger.LogInformation("Disconnecting SMTP server");
        if (_smtpClient is not null)
        {
            if (_smtpClient.IsConnected)
                await _smtpClient.DisconnectAsync(true, _combinedCancellationToken);
            _smtpClient.Dispose();
            _smtpClient = null;
        }
    }

    private async Task SendEmail(MailInfo nextEmailMessage)
    {
        if (_smtpClient is null)
        {
            _logger.LogError("SMTP client is null, cannot send email");
            return;
        }
        _logger.LogInformation("Sending email to {Email}", nextEmailMessage.Email);
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        message.To.Add(new MailboxAddress("", nextEmailMessage.Email));
        message.Subject = nextEmailMessage.Subject;

        message.Body = new TextPart("plain")
        {
            Text = nextEmailMessage.Message
        };
        await _smtpClient.SendAsync(message, _combinedCancellationToken);
        _logger.LogInformation("Email to {Email} sent!", nextEmailMessage.Email);
    }
}