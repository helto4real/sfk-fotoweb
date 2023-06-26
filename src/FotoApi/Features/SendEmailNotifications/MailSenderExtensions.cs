﻿namespace FotoApi.Features.SendEmailNotifications;

internal static class MailSenderExtensions
{
    public static IServiceCollection AddMailSenderService(this IServiceCollection services)
    {
        services.AddSingleton<MailSender>();
        services.AddSingleton<IMailSender>(s => s.GetRequiredService<MailSender>());
        services.AddSingleton<IMailQueue>(s => s.GetRequiredService<MailSender>());
        services.AddHostedService<MailSenderService>();

        // services.Configure<MailInfo>(config.GetSection("NetDaemon"))
        return services;
    }
}