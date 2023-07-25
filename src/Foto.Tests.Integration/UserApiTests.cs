﻿using Foto.Tests.Integration.TestContainer;
using FotoApi;
using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.SendEmailNotifications;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Foto.Tests.Integration;

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
            UserName = "fotoapiuser@somedomain.com", Password = "P@ssw0rd!", FirstName = "Test", LastName = "Test", Email = "fotoapiuser@somedomain.com", UrlToken = urlToken
        });
        
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
    // [Fact]
    // public async Task MissingUserOrPasswordReturnsBadRequest()
    // {
    //     await using var application = new TodoApplication();
    //     await using var db = application.CreateTodoDbContext();
    //
    //     var client = application.CreateClient();
    //     var response = await client.PostAsJsonAsync("/users", new NewUserInfo { Username = "todouser", Password = "" });
    //
    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    //
    //     var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
    //     Assert.NotNull(problemDetails);
    //
    //     Assert.Equal("One or more validation errors occurred.", problemDetails.Title);
    //     Assert.NotEmpty(problemDetails.Errors);
    //     Assert.Equal(new[] { "The Password field is required." }, problemDetails.Errors["Password"]);
    //
    //     response = await client.PostAsJsonAsync("/users", new NewUserInfo { Username = "", Password = "password" });
    //
    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    //
    //     problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
    //     Assert.NotNull(problemDetails);
    //
    //     Assert.Equal("One or more validation errors occurred.", problemDetails.Title);
    //     Assert.NotEmpty(problemDetails.Errors);
    //     Assert.Equal(new[] { "The Username field is required." }, problemDetails.Errors["Username"]);
    // }
    //
    //
    //
    // [Fact]
    // public async Task MissingUsernameOrProviderKeyReturnsBadRequest()
    // {
    //     await using var application = new TodoApplication();
    //     await using var db = application.CreateTodoDbContext();
    //
    //     var client = application.CreateClient();
    //     var response = await client.PostAsJsonAsync("/users/token/Google", new ExternalUserInfo { Username = "todouser" });
    //
    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    //
    //     var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
    //     Assert.NotNull(problemDetails);
    //
    //     Assert.Equal("One or more validation errors occurred.", problemDetails.Title);
    //     Assert.NotEmpty(problemDetails.Errors);
    //     Assert.Equal(new[] { $"The {nameof(ExternalUserInfo.ProviderKey)} field is required." }, problemDetails.Errors[nameof(ExternalUserInfo.ProviderKey)]);
    //
    //     response = await client.PostAsJsonAsync("/users/token/Google", new ExternalUserInfo { ProviderKey = "somekey" });
    //
    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    //
    //     problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
    //     Assert.NotNull(problemDetails);
    //
    //     Assert.Equal("One or more validation errors occurred.", problemDetails.Title);
    //     Assert.NotEmpty(problemDetails.Errors);
    //     Assert.Equal(new[] { $"The Username field is required." }, problemDetails.Errors["Username"]);
    // }
    //
    //
    // [Fact]
    // public async Task CanGetATokenForExternalUser()
    // {
    //     await using var application = new TodoApplication();
    //     await using var db = application.CreateTodoDbContext();
    //
    //     var client = application.CreateClient();
    //     var response = await client.PostAsJsonAsync("/users/token/Google", new ExternalUserInfo { Username = "todouser", ProviderKey = "1003" });
    //
    //     Assert.True(response.IsSuccessStatusCode);
    //
    //     var token = await response.Content.ReadFromJsonAsync<AuthToken>();
    //
    //     Assert.NotNull(token);
    //
    //     // Check that the token is indeed valid
    //
    //     var req = new HttpRequestMessage(HttpMethod.Get, "/todos");
    //     req.Headers.Authorization = new("Bearer", token.Token);
    //     response = await client.SendAsync(req);
    //
    //     Assert.True(response.IsSuccessStatusCode);
    //
    //     using var scope = application.Services.CreateScope();
    //     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<FotoUser>>();
    //     var user = await userManager.FindByLoginAsync("Google", "1003");
    //     Assert.NotNull(user);
    //     Assert.Equal("todouser", user.UserName);
    // }
    //
    // [Fact]
    // public async Task BadRequestForInvalidCredentials()
    // {
    //     await using var application = new TodoApplication();
    //     await using var db = application.CreateTodoDbContext();
    //     await application.CreateUserAsync("todouser", "p@assw0rd1");
    //
    //     var client = application.CreateClient();
    //     var response = await client.PostAsJsonAsync("/users/token", new NewUserInfo { Username = "todouser", Password = "prd1" });
    //
    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    // }
     public string AddCreateUserToken(PhotoServiceDbContext db)
     {
         var urlToken = UrlTokenCreator.CreateUrlTokenFromUrlTokenType(UrlTokenType.AllowAddUser);
         db.UrlTokens.Add(urlToken);
         db.SaveChanges();
         return urlToken.Token;
     }
    public UserApiTests(TestContainerLifeTime testContinerLifetime) : base(testContinerLifetime)
    {
    }
}
