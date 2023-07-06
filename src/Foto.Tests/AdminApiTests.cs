using FotoApi.Model;

namespace Foto.Tests;

public class AdminApiTests
{
    [Fact]
    public async Task GetAllUsersShouldReturnSingle()
    {
        await using var application = new FotoApplication();
        await using var db = application.CreateTodoDbContext();

        await application.AddDefaultAdmin();
        var client = application.CreateClient("admin", true);
        var response = await client.GetAsync("api/admin/users");
        Assert.True(response.IsSuccessStatusCode);
        var users =await  response.Content.ReadFromJsonAsync<List<User>>();
        Assert.NotNull(users);
        
        var user = users.Single();
        Assert.Equal("admin", user.UserName);
    }
}