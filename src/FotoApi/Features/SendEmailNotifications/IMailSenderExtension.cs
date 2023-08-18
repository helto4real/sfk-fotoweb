namespace FotoApi.Features.SendEmailNotifications;

internal static class MailSenderExtension
{
    public static Task SendEmailConfirmationAsync(this IMailSender sender, string email, string urlToken,
        string photoWebUri, CancellationToken ct)
    {
        var subject = "SFK FotoWebb - Bekräfta din e-postadress";
        var message =
            $"Bekräfta din e-postaddress genom att klicka på följande länk : {photoWebUri}/confirm-email/?token={Uri.EscapeDataString(urlToken)}";
        return sender.SendEmailAsync(new MailInfo(email, subject, message), ct);
    }    
    
    public static Task SendPasswordResetAsync(this IMailSender sender, string email, string token,
        string photoWebUri, CancellationToken ct)
    {
        var subject = "SFK Fotowebb - Återställ lösenord";
        var message =
            $"Du kan nu återställa ditt lösenord genom att klicka på följande länk : {photoWebUri}/password-reset/?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}. Om du inte begärt återställning av lösenord, vänligen ignorera detta meddelande.";
        return sender.SendEmailAsync(new MailInfo(email, subject, message), ct);
    }

    public static Task SendAccountChangedEmailAsync(this IMailSender sender, string email, string message,
        CancellationToken ct)
    {
        var subject = "SFK FotoWebb - Epost";
        return sender.SendEmailAsync(new MailInfo(email, subject, message), ct);
    }
}