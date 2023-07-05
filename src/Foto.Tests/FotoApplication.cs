using System.Data;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.SendEmailNotifications;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;



namespace Foto.Tests;

internal class FotoApplication : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _sqliteConnection = new("Filename=:memory:");
    public Mock<IMailSender> MailSenderMock => new();
    public Mock<IPhotoStore> PhotoStoreMock => new();
    public Mock<IMailQueue> MailQueue => new();
    
    public PhotoServiceDbContext CreateTodoDbContext()
    {
        var db = Services.GetRequiredService<IDbContextFactory<PhotoServiceDbContext>>().CreateDbContext();
        db.Database.EnsureCreated();
        // db.Database.Migrate();
        return db;
    }

    public async Task CreateUserAsync(string username, string? password = null)
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var newUser = new User { UserName = username };
        var result = await userManager.CreateAsync(newUser, password ?? Guid.NewGuid().ToString());
        Assert.True(result.Succeeded);
    }
    public async Task PreRegisterUserAsync(string username, string? password = null)
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var newUser = new User { UserName = username, Email = username};
        var result = await userManager.CreateAsync(newUser);
        Assert.True(result.Succeeded);
    }
    
    public async Task AddDefaultAdmin()
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var role = roleManager.CreateAsync(new Role { Name = "Admin" });
        
        var newUser = new User { UserName = "admin", Email = "admin@somedomain.com"};
        var result = await userManager.CreateAsync(newUser, "P@ssw0rd!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newUser, "Admin");
        }
        Assert.True(result.Succeeded);
    }

    public HttpClient CreateClient(string id, bool isAdmin = false)
    {
        return CreateDefaultClient(new AuthHandler(req =>
        {
            var token = CreateToken(id, isAdmin);
            req.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        }));
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
        // Open the connection, this creates the SQLite in-memory database, which will persist until the connection is closed
        _sqliteConnection.Open();

        builder.ConfigureServices(services =>
        {
            // services.AddSqlite<PhotoServiceDbContext>("Filename=:memory:");
            // We're going to use the factory from our tests
            services.AddDbContextFactory<PhotoServiceDbContext>();

            // We need to replace the configuration for the DbContext to use a different configured database
            services.AddDbContextOptions<PhotoServiceDbContext>(o => o.UseSqlite(_sqliteConnection)); 
            // services.AddIdentityCore<User>(options => options.User.RequireUniqueEmail = true)
            //     .AddRoles<Role>()
            //     .AddEntityFrameworkStores<PhotoServiceDbContext>();
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
            services.AddSingleton<IDefaultAdminUserInitializerService, DoNothingService>();
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
                ["Authentication:Schemes:Bearer:SigningKeys:0:Value"] = base64Key
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
        _sqliteConnection?.Dispose();
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

public class DoNothingService : IMailSenderService, IDefaultAdminUserInitializerService, IHandleExpiredUrlTokensService
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

