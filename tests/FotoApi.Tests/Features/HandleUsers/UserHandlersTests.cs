using FluentAssertions;
using FotoApi.Features.HandleUsers.CommandHandlers;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Features.HandleUsers.QueryHandlers;
using FotoApi.Model;
using FotoApi.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Wolverine;

namespace FotoApi.Tests.Features.HandleUsers;

public class UsersHandlersTests : IAsyncLifetime
{
    private readonly TestContext _ctx = new();
    [Fact]
    public async Task GetUsersShouldReturnUsers()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        await _ctx.CreateUserAsync("theadmin", true, "theadmin@domain.com");
        await _ctx.CreateUserAsync("user", false, "user@domain.com");
        
        // ACT
        var handler = new GetUsersHandler(userManager);
        var result = await handler.Handle(CancellationToken.None);
        
        // ASSERT
        // We already have default admin user in the database
        result.Should().HaveCount(3);
        result[1].UserName.Should().Be("theadmin");
        result[1].Roles.Should().Contain("Admin");
        result[1].Email.Should().Be("theadmin@domain.com");
        result[2].UserName.Should().Be("user");
        result[2].Roles.Should().NotContain("Admin");
        result[2].Email.Should().Be("user@domain.com");
    }
    [Fact]
    public async Task GetUserFromUserNameShouldReturnUser()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        await _ctx.CreateUserAsync("user", false, "user@domain.com");
    
        // ACT
        var handler = new GetUserFromUsernameHandler(userManager);
        var result = await handler.Handle("user", CancellationToken.None);
    
        // ASSERT
        result!.Should().NotBeNull();
        result!.UserName.Should().Be("user");
        result.Roles.Should().NotContain("Admin");
    }
    
    [Fact]
    public async Task GetNonExistingUserFromUserNameShouldThrow()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
    
        // ACT
        var handler = new GetUserFromUsernameHandler(userManager);
        var action = async () => await handler.Handle("user", CancellationToken.None);
    
        // ASSERT
        await action.Should().ThrowAsync<UserNotFoundException>();
    }
    
    [Fact]
    public async Task GetUserFromTokenShouldReturnUser()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        await _ctx.CreateUserAsync("user", false, "user@domain.com");
        _ctx.TokenService.Setup(x => x.GetUserIdByAccessTokenAsync(It.IsAny<string>())).Returns("user");
    
        // ACT
        var handler = new GetUserFromTokenHandler(_ctx.TokenService.Object, userManager);
        var result = await handler.Handle("token", CancellationToken.None);
    
        // ASSERT
        result.Should().NotBeNull();
        result.UserName.Should().Be("user");
        result.Roles.Should().NotContain("Admin");
    }
    
    [Fact]
    public async Task GetMissingUserFromTokenShouldThrow()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        _ctx.TokenService.Setup(x => x.GetUserIdByAccessTokenAsync(It.IsAny<string>())).Returns("user");
    
        // ACT
        var handler = new GetUserFromTokenHandler(_ctx.TokenService.Object, userManager);
        var action = async () => await handler.Handle("missing_token", CancellationToken.None);
        
        // ASSERT
        await action.Should().ThrowAsync<UserNotFoundException>();
    }    
    
    [Fact]
    public async Task GetMissingTokenShouldThrow()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        _ctx.TokenService.Setup(x => x.GetUserIdByAccessTokenAsync(It.IsAny<string>())).Returns((string?)null);
    
        // ACT
        var handler = new GetUserFromTokenHandler(_ctx.TokenService.Object, userManager);
        var action = async () => await handler.Handle("missing_token", CancellationToken.None);
        
        // ASSERT
        await action.Should().ThrowAsync<NoUserWithTokenFoundException>();
    }
    
    [Fact]
    public async Task PrecreateUserShouldReturnUser()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        var request = new EmailRequest("prereg@domain.com");
        // ACT
        var handler = new PreCreateUserHandler(userManager, Mock.Of<ILogger<PreCreateUserHandler>>());
        var result = await handler.Handle(request, CancellationToken.None);
    
        // ASSERT
        result.Should().NotBeNull();
        result.UserName.Should().Be("prereg@domain.com");
        result.Roles.Should().NotContain("Admin");
        result.Email.Should().Be("prereg@domain.com");
    }
    
    [Fact]
    public async Task UpdateUserShouldUpdateUser()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        await _ctx.CreateUserAsync("user", false, "user@domain.com");
        var request = new UserRequest
        {
            UserName = "user",
            FirstName = "FirsName",
            LastName = "LastName",
            IsAdmin = false
        };
    
        // ACT
        var handler = new UpdateUserHandler(userManager);
        await handler.Handle(request, CancellationToken.None);
        var resultUser = await userManager.FindByNameAsync("user");
        // ASSERT
        resultUser!.UserName.Should().Be("user");
    }    
    
    [Fact]
    public async Task UpdateUserRoleToAdminShouldUpdateUser()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        await _ctx.CreateUserAsync("user", false, "user@domain.com");
        var request = new UserRequest
        {
            UserName = "user",
            FirstName = "FirsName",
            LastName = "LastName",
            IsAdmin = true
        };
    
        // ACT
        var handler = new UpdateUserHandler(userManager);
        await handler.Handle(request, CancellationToken.None);
        var resultUser = await userManager.FindByNameAsync("user");
        var isAdmin = await userManager.IsInRoleAsync(resultUser!, "Admin");
        
        // ASSERT
        isAdmin.Should().BeTrue();
    }
    
    [Fact]
    public async Task UpdateUserRoleRemoveAdminShouldUpdateUser()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        var username = "update_user";
        await _ctx.CreateUserAsync(username, true, "user@domain.com");
        var request = new UserRequest
        {
            UserName = username,
            FirstName = "FirsName",
            LastName = "LastName",
            IsAdmin = false
        };
    
        // ACT
        var handler = new UpdateUserHandler(userManager);
        await handler.Handle(request, CancellationToken.None);
        var resultUser = await userManager.FindByNameAsync(username);
        var isAdmin = await userManager.IsInRoleAsync(resultUser!, "Admin");
        
        // ASSERT
        isAdmin.Should().BeFalse();
    }
    
    [Fact]
    public async Task DeleteUserShouldDeleteUser()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        await _ctx.CreateUserAsync("delete_user", false, "user@domain.com");
    
        // ACT
        var handler = new DeleteUserHandler(userManager, _ctx.Bus.Object);
        await handler.Handle("delete_user", CancellationToken.None);
        
        // ASSERT
        (await userManager.FindByNameAsync("delete_user")).Should().BeNull();
        
        _ctx.Bus.Verify(e => e.PublishAsync(It.IsAny<UserDeletedNotification>(), It.IsAny<DeliveryOptions>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteNonExistingUserShouldThrow()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        // ACT
        var handler = new DeleteUserHandler(userManager, _ctx.Bus.Object);
        var action = async () => await handler.Handle("non_existing_user", CancellationToken.None);
        
        // ASSERT
        await action.Should().ThrowAsync<UserNotFoundException>();
    }
    
    [Fact]
    public async Task CreateUserShouldBeCreated()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        var dbContext = _ctx.CreateDbContext();
        var request = new NewUserRequest
        {
            UserName = "newuser@domain.com",
            // FirstName = "FirstName",
            // LastName = "LastName",
            Email = "newuser@domain.com",
            UrlToken = "token",
            Password = "P@ssw0rd!"
        };
        
        dbContext.UrlTokens.Add(new UrlToken
        {
            Id = Guid.NewGuid(),
            Data = "",
            Token = "token",
            ExpirationDate = DateTime.Now.AddMinutes(5),
            UrlTokenType = UrlTokenType.AllowAddUser
        });
        await dbContext.SaveChangesAsync();
        var createUserResult = await userManager.CreateAsync(new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "newuser@domain.com",
            Email = "newuser@domain.com",
            RefreshToken = ""
        });
        createUserResult.Succeeded.Should().BeTrue();
    
        // ACT
        var handler = new CreateUserHandler(userManager, dbContext, _ctx.Bus.Object, Mock.Of<ILogger<CreateUserHandler>>()); 
        var result = await handler.Handle(request, CancellationToken.None);
        
        // ASSERT
        result.Should().NotBeNull();
        result.UserName.Should().Be("newuser@domain.com");
    }

    [Fact]
    public async Task ConfirmEmailShouldConfirmEmail()
    {
        // ARRANGE
        var userManager = _ctx.Sp.GetRequiredService<UserManager<User>>();
        var dbContext = _ctx.CreateDbContext();

        var newuser = await _ctx.CreateUserAsync("confirm_user@domain.com", false, "confirm_user@domain.com");
        dbContext.UrlTokens.Add(new UrlToken
        {
            Id = Guid.NewGuid(),
            Data = newuser.Id,
            Token = "confirm_email_token",
            ExpirationDate = DateTime.Now.AddMinutes(5),
            UrlTokenType = UrlTokenType.ConfirmEmail
        });
        await dbContext.SaveChangesAsync();
        // ACT
        var handler = new ConfirmEmailHandler(dbContext, userManager);
        await handler.Handle("confirm_email_token", CancellationToken.None);

        var user = await userManager.FindByNameAsync("confirm_user@domain.com");

        user!.EmailConfirmed.Should().BeTrue();
    }

    public Task InitializeAsync()
    {
        return _ctx.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return _ctx.DisposeAsync();
    }
}


