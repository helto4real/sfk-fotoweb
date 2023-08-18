using System.Net.Http.Headers;
using Foto.Tests.Integration.TestContainer;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Foto.Tests.Integration;
[Collection("Integration tests collection")]
public class IntegrationTestsBase : IAsyncDisposable
{
    private readonly TestContainerLifeTime _testContainerLifetime;
    private readonly FotoApplication _fotoApplication;

    internal FotoApplication App => _fotoApplication;
    
    public IntegrationTestsBase(TestContainerLifeTime testContainerLifetime)
    {
        _testContainerLifetime = testContainerLifetime;

        _fotoApplication = new(testContainerLifetime.Host, testContainerLifetime.Port, testContainerLifetime.UserName,
            testContainerLifetime.Password);
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
    
    public async Task CreateUserAsync(string username, string? password = null, string? email = null, string? refreshToken=null, List<string>? roles = null)
    {
        using var scope = _fotoApplication.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var newUser = new User { UserName = username, Email = email, RefreshToken = refreshToken??Guid.NewGuid().ToString()};
        var result = await userManager.CreateAsync(newUser, password ?? Guid.NewGuid().ToString());
        Assert.True(result.Succeeded);
        var roleResult = await userManager.AddToRolesAsync(newUser, roles ?? new List<string> {"Member"});
        Assert.True(roleResult.Succeeded);
    }

    public async Task<Member> CreateMemberAsync(string email, bool active = true, List<string>? roles = null)
    {
        using var scope = _fotoApplication.Services.CreateScope();
        var db = await scope.ServiceProvider.GetRequiredService<IDbContextFactory<PhotoServiceDbContext>>().CreateDbContextAsync();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        await CreateUserAsync(email, null, email, roles: roles);
        var user = await userManager.FindByEmailAsync(email);
        var member = new Member
        {
            OwnerReference = user!.Id,
            FirstName = "MemberFirstName",
            LastName = "MemberLastName",
            Address = "MemberStreet 1",
            ZipCode = "9999",
            City = "MemberCity",
            IsActive = active
        };
        db.Members.Add(member);
        await db.SaveChangesAsync();
        return member;
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
         var roles = isAdmin switch
         {
             true => new[] { "Admin" },
             false => Array.Empty<string>()
         };
         
         return tokenService.GenerateToken(id, roles);
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
     