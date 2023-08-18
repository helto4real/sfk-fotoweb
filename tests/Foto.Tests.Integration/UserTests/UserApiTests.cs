using Foto.Tests.Integration.TestContainer;
using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;

namespace Foto.Tests.Integration.UserTests;

[Collection("Integration tests collection")]
public class UserApiTests : IntegrationTestsBase
{
    [Fact]
    public async Task CanCreateAUser()
    {
        await using var db = CreateFotoAppDbContext();
        var urlToken = AddCreateUserToken(db);
        await PreRegisterUserAsync("fotoapiuser@somedomain.com");
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/users", new NewUserRequest
        {
            UserName = "fotoapiuser@somedomain.com", Password = "P@ssw0rd!", Email = "fotoapiuser@somedomain.com", UrlToken = urlToken
        });
        //FirstName = "Test", LastName = "Test" removed
        
        Assert.True(response.IsSuccessStatusCode);

        var user = db.Users.Single(n => n.UserName == "fotoapiuser@somedomain.com");
        Assert.NotNull(user);

        Assert.Equal("fotoapiuser@somedomain.com", user.UserName);
    }

    [Fact]
    public async Task CanGetATokenForValidUser()
    {
        const string user = "appuser";
        const string password = "P@assw0rd1";
        await using var db = CreateFotoAppDbContext();
        await CreateUserAsync(user, password, "someuser@domain.com");
    
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/users/token", new  { Username = user, Password = password });
    
        Assert.True(response.IsSuccessStatusCode);
    
        var token = await response.Content.ReadFromJsonAsync<UserAuthorizedResponse>();
    
        Assert.NotNull(token);
    
        // Check that the token is indeed valid
    
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/images/user");
        req.Headers.Authorization = new("Bearer", token.Token);
        response = await client.SendAsync(req);
    
        Assert.True(response.IsSuccessStatusCode);
    }
    
    private string AddCreateUserToken(PhotoServiceDbContext db)
     {
         var urlToken = UrlTokenCreator.CreateUrlTokenFromUrlTokenType(UrlTokenType.AllowAddUser);
         db.UrlTokens.Add(urlToken);
         db.SaveChanges();
         return urlToken.Token;
     }
    public UserApiTests(TestContainerLifeTime testContainerLifetime) : base(testContainerLifetime)
    {
    }
}
