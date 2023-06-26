namespace FotoApi.Infrastructure.Settings;

internal static class ApiSettingsExtensions
{
    public static void AddPhotoApiConfiguration(this WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.Configure<ApiSettings>(appBuilder.Configuration.GetSection("ApiSettings"));
        appBuilder.Services.Configure<EmailSettings>(appBuilder.Configuration.GetSection("EmailSettings"));
    }
}