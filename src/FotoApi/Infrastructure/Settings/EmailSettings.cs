namespace FotoApi.Infrastructure.Settings;

public record EmailSettings
{
    public string SmtpServer { get; init; } = default!;
    public int SmtpPort { get; init; } = 587;
    public string SmtpUsername { get; init; } = default!;
    public string SmtpPassword { get; init; } = default!;
    public string SenderEmail { get; init; } = default!;
    public bool UseSsl { get; init; }
    public string SenderName { get; set; } = default!;
}