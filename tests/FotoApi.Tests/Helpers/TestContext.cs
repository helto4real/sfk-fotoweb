using FluentAssertions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Wolverine;

namespace FotoApi.Tests.Helpers;

public class TestContext : IAsyncLifetime
{
    private readonly SqliteConnection _sqliteConnection = new("Filename=:memory:");
    
    public  Mock<IMessageBus> Bus { get; } = new();
    
    public ServiceProvider Sp { get; set; } = default!;
    
    public PhotoServiceDbContext CreateDbContext()
    {
        var db = Sp.GetRequiredService<IDbContextFactory<PhotoServiceDbContext>>().CreateDbContext();
        db.Database.EnsureCreated();
        return db;
    }
    
    public Mock<ITokenService> TokenService { get; } = new();
    
    public async Task InitializeAsync()
    {
        _sqliteConnection.Open();
        Sp = CreateServiceCollection().BuildServiceProvider();
        var db = await Sp.GetRequiredService<IDbContextFactory<PhotoServiceDbContext>>().CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _sqliteConnection.DisposeAsync();
    }
    
    public async Task<User> CreateUserAsync(string username, bool isAdmin, string? email = null, string? refreshToken=null)
    {
        var userManager = Sp.GetRequiredService<UserManager<User>>();
        var user = new User
        {
            UserName = username,
            Id = Guid.NewGuid().ToString(),
            Email = email,
            RefreshToken = refreshToken ?? string.Empty,
            RefreshTokenExpirationDate = DateTime.MinValue.ToUniversalTime()
        };
        var result = await userManager.CreateAsync(user);
        result.Succeeded.Should().BeTrue();
        
        if (isAdmin)
        {
            var addToRoleResult = await userManager.AddToRoleAsync(user, "Admin");
            addToRoleResult.Succeeded.Should().BeTrue();
        }
        return user;
    }
    
    private ServiceCollection CreateServiceCollection()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddDbContextFactory<PhotoServiceDbContext>();
        serviceCollection.AddDbContextOptions<PhotoServiceDbContext>(o => o.UseSqlite(_sqliteConnection));
        // serviceCollection.AddDbContext<PhotoServiceDbContext>();
        serviceCollection.AddSingleton<ITokenService>(_ => TokenService.Object);
        
        serviceCollection.AddIdentityCore<User>(options => options.User.RequireUniqueEmail = true)
            .AddRoles<Role>()
            .AddEntityFrameworkStores<PhotoServiceDbContext>();
        return serviceCollection;
    }
}

// [CollectionDefinition("Fixture")]
// public class TestFixtureColection : ICollectionFixture<TestContext>
// {
//
// }
