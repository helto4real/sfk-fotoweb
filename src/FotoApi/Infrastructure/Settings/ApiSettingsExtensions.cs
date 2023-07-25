namespace FotoApi.Infrastructure.Settings;

internal static class ApiSettingsExtensions
{
    public static WebApplicationBuilder AddPhotoApiConfiguration(this WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.Configure<ApiSettings>(appBuilder.Configuration.GetSection("ApiSettings"));
        appBuilder.Services.Configure<EmailSettings>(appBuilder.Configuration.GetSection("EmailSettings"));
        return appBuilder;
    }
}