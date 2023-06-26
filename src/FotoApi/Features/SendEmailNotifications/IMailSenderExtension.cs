namespace FotoApi.Features.SendEmailNotifications;

internal static class IMailSenderExtension
{
    public static Task SendEmailConfirmationAsync(this IMailSender sender, string email, string urlToken,
        string photoWebUri)
    {
        var subject = "Bekräfta din e-postadress SFK FotoWebb";
        var message =
            $"Bekräfta din e-postaddress genom att klicka på följande länk : {photoWebUri}/confirm-email/?token={Uri.EscapeDataString(urlToken)}";
        return sender.SendEmailAsync(new MailInfo(email, subject, message));
    }
}