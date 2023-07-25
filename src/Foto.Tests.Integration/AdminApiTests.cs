using Foto.Tests.Integration.TestContainer;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;
using Microsoft.EntityFrameworkCore;
using NuGet.Frameworks;

namespace Foto.Tests.Integration;

[Collection("Integration tests collection")]
public class AdminApiTests : IntegrationTestsBase
{
    [Fact]
    public async Task GetAllUsersShouldReturnSingle()
    {
        await using var db = CreateFotoAppDbContext();

        var client = CreateClient("admin", true);
        var response = await client.GetAsync("api/admin/users");
        Assert.True(response.IsSuccessStatusCode);
        var users =await  response.Content.ReadFromJsonAsync<List<User>>();
        Assert.NotNull(users);
        
        var user = users.Single();
        Assert.Equal("admin", user.UserName);
    }

    public AdminApiTests(TestContainerLifeTime testContinerLifetime) : base(testContinerLifetime)
    {
    }
}