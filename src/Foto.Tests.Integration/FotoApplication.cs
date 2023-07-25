using System.Data;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.SendEmailNotifications;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.MessagingDbContext;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Model;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Npgsql;

namespace Foto.Tests.Integration;

internal class FotoApplication : WebApplicationFactory<Program>
{
    // private readonly NpgsqlConnection _photoAppConnection;
    // private readonly NpgsqlConnection _messagingConnection;
    private readonly string _photoAppConnectionString;
    private readonly string _messagingConnectionString;
    private readonly EnvironmentConfigManager _environmentConfigManager;
    public Mock<IMailSender> MailSenderMock => new();
    public Mock<IPhotoStore> PhotoStoreMock => new();
    public Mock<IMailQueue> MailQueue => new();
    
    public FotoApplication(string host, uint port, string username, string password)
    {
        // _photoAppConnection = new($"Host={host}:{port};Database=PhotoApp;Username={username};Password={password}");
        // _messagingConnection = new($"Host={host}:{port};Database=Messaging;Username={username};Password={password}");
        _photoAppConnectionString = $"Host={host}:{port};Database=PhotoApp;Username={username};Password={password}";
        _messagingConnectionString = $"Host={host}:{port};Database=Messaging;Username={username};Password={password}";
        // Hack to make the real app use correct connection strings
        _environmentConfigManager = new(_photoAppConnectionString, _messagingConnectionString);
    }


    public string AddCreateUserToken(PhotoServiceDbContext db)
    {
        var urlToken = UrlTokenCreator.CreateUrlTokenFromUrlTokenType(UrlTokenType.AllowAddUser);
        db.UrlTokens.Add(urlToken);
        db.SaveChanges();
        return urlToken.Token;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // We're going to use the factory from our tests
            services.AddDbContextFactory<PhotoServiceDbContext>();
            services.AddDbContextFactory<MessagingDbContext>();

            // We need to replace the configuration for the DbContext to use a different configured database
            services.AddDbContextOptions<PhotoServiceDbContext>(o => o.UseNpgsql(_photoAppConnectionString)); 
            services.AddDbContextOptions<MessagingDbContext>(o => o.UseNpgsql(_messagingConnectionString)); 
            
            // Lower the requirements for the tests
            services.Configure<IdentityOptions>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireDigit = false;
                o.Password.RequiredUniqueChars = 0;
                o.Password.RequiredLength = 1;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
            });
            services.AddSingleton<IMailSender>(s => MailSenderMock.Object);
            services.AddScoped<IPhotoStore>(n => PhotoStoreMock.Object);
            services.AddSingleton<IMailQueue>(s => MailQueue.Object);
            // Just not start the services by replacing them with a do nothing service
            services.AddSingleton<IMailSenderService, DoNothingService>();
            services.AddSingleton<IHandleExpiredUrlTokensService, DoNothingService>();
        });

        // We need to configure signing keys for CI scenarios where
        // there's no user-jwts tool
        var keyBytes = new byte[32];
        RandomNumberGenerator.Fill(keyBytes);
        var base64Key = Convert.ToBase64String(keyBytes);

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Schemes:Bearer:SigningKeys:0:Issuer"] = "dotnet-user-jwts",
                ["Authentication:Schemes:Bearer:SigningKeys:0:Value"] = base64Key,
                ["ConnectionStrings:FotoApi"] = _photoAppConnectionString,
                ["ConnectionStrings:Messaging"] = _messagingConnectionString
            });
        });

        return base.CreateHost(builder);
    }
    private string CreateToken(string id, bool isAdmin = false)
    {
        // Read the user JWTs configuration for testing so unit tests can generate
        // JWT tokens.
        var tokenService = Services.GetRequiredService<ITokenService>();

        return tokenService.GenerateToken(id, isAdmin);
    }

    protected override void Dispose(bool disposing)
    {
        _environmentConfigManager.Dispose();
        // _photoAppConnection.Dispose();
        // _messagingConnection.Dispose();
        base.Dispose(disposing);
    }

    private sealed class AuthHandler : DelegatingHandler
    {
        private readonly Action<HttpRequestMessage> _onRequest;

        public AuthHandler(Action<HttpRequestMessage> onRequest)
        {
            _onRequest = onRequest;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _onRequest(request);
            return base.SendAsync(request, cancellationToken);
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Remove<T>(this IServiceCollection services)
    {
        if (services.IsReadOnly)
        {
            throw new ReadOnlyException($"{nameof(services)} is read only");
        }

        var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T));
        if (serviceDescriptor != null) services.Remove(serviceDescriptor);

        return services;
    }
}

public class DoNothingService : IMailSenderService, IHandleExpiredUrlTokensService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

