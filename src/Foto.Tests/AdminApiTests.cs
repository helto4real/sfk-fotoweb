using Foto.Tests.TestContainer;
using FotoApi.Model;

namespace Foto.Tests;

public class AdminApiTests : IntegrationTestsBase
{
    [Fact]
    public async Task GetAllUsersShouldReturnSingle()
    {
        await using var db = CreateTodoDbContext();

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