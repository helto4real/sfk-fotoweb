using Foto.Tests.Integration.TestContainer;
using FotoApi.Model;

namespace Foto.Tests.Integration;

[Collection("Integration tests collection")]
public class AdminApiTests : IntegrationTestsBase
{
    [Fact]
    public async Task GetAllUsersShouldReturnUsers()
    {
        await using var db = CreateFotoAppDbContext();

        var client = CreateClient("admin", true);
        var response = await client.GetAsync("api/users");
        Assert.True(response.IsSuccessStatusCode);
        var users =await  response.Content.ReadFromJsonAsync<List<User>>();
        Assert.NotNull(users);
        
        var user = users.Single(n => n.UserName == "admin");
        Assert.Equal("admin", user.UserName);
    }

    public AdminApiTests(TestContainerLifeTime testContinerLifetime) : base(testContinerLifetime)
    {
    }
}