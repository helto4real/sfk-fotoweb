using System.Net.Http.Headers;
using System.Security.Cryptography;
using Foto.Tests.Integration.TestContainer;
using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.SendEmailNotifications;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.MessagingDbContext;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Npgsql;
using Spectre.Console.Rendering;

namespace Foto.Tests.Integration;
[Collection("Integration tests collection")]
public class IntegrationTestsBase : IAsyncDisposable
{
    private readonly TestContainerLifeTime _testContinerLifetime;
    private FotoApplication _fotoApplication;

    internal FotoApplication App => _fotoApplication;
    
    public IntegrationTestsBase(TestContainerLifeTime testContinerLifetime)
    {
        _testContinerLifetime = testContinerLifetime;

        _fotoApplication = new(testContinerLifetime.Host, testContinerLifetime.Port, testContinerLifetime.UserName,
            testContinerLifetime.Password);
        var db = _fotoApplication.Services.GetRequiredService<IDbContextFactory<PhotoServiceDbContext>>().CreateDbContext();
        db.Database.Migrate();


    }
    public ValueTask DisposeAsync()
    {
        _fotoApplication.Dispose();
        return ValueTask.CompletedTask;
    }
    
    public PhotoServiceDbContext CreateFotoAppDbContext()
    {
        var db = _fotoApplication.Services.GetRequiredService<IDbContextFactory<PhotoServiceDbContext>>().CreateDbContext();
        return db;
    }
    
    public async Task CreateUserAsync(string username, string? password = null, string? email = null, string? refreshToken=null)
    {
        using var scope = _fotoApplication.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var newUser = new User { UserName = username, Email = email, RefreshToken = refreshToken??Guid.NewGuid().ToString()};
        var result = await userManager.CreateAsync(newUser, password ?? Guid.NewGuid().ToString());
        Assert.True(result.Succeeded);
    }
    public async Task PreRegisterUserAsync(string username, string? password = null)
    {
        using var scope = _fotoApplication.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var newUser = new User { UserName = username, Email = username, RefreshToken = "", RefreshTokenExpirationDate = DateTime.MinValue};
        var result = await userManager.CreateAsync(newUser);
        Assert.True(result.Succeeded);
    }

     public HttpClient CreateClient(string id, bool isAdmin = false)
     {
         return _fotoApplication.CreateDefaultClient(new AuthHandler(req =>
         {
             var token = CreateToken(id, isAdmin);
             req.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
         }));
     }
     
     public HttpClient CreateClient() => _fotoApplication.CreateDefaultClient();

     private string CreateToken(string id, bool isAdmin = false)
     {
         // Read the user JWTs configuration for testing so unit tests can generate
         // JWT tokens.
         var tokenService = _fotoApplication.Services.GetRequiredService<ITokenService>();

         return tokenService.GenerateToken(id, isAdmin);
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
     