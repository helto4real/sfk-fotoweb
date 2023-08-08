namespace FotoApi.Features.SendEmailNotifications;

internal static class IMailSenderExtension
{
    public static Task SendEmailConfirmationAsync(this IMailSender sender, string email, string urlToken,
        string photoWebUri, CancellationToken ct)
    {
        var subject = "Bekräfta din e-postadress SFK FotoWebb";
        var message =
            $"Bekräfta din e-postaddress genom att klicka på följande länk : {photoWebUri}/confirm-email/?token={Uri.EscapeDataString(urlToken)}";
        return sender.SendEmailAsync(new MailInfo(email, subject, message), ct);
    }

    public static Task SendAccountChangedEmailAsync(this IMailSender sender, string email, string message,
        CancellationToken ct)
    {
        var subject = "SFK FotoWebb - Epost";
        return sender.SendEmailAsync(new MailInfo(email, subject, message), ct);
    }
}