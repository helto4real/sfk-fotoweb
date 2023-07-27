using FotoApi.Features.HandleUsers.CommandHandlers;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Infrastructure.Security.Authorization.Commands;
using MediatR;
using Riok.Mapperly.Abstractions;

namespace FotoApi.Infrastructure.Security.Authorization.Dto;

[Mapper]
public partial class LoginMapper
{
    public partial NewUserRequest ToCreateUserCommand(LoginAndCreateUserRequest request);

    public UserAuthorizedResponse ToUserAuthorizedResponse(UserResponse userResponse, string token)
    {
        var userAuthorizedResponse = ToUserAuthorizedResponse(userResponse);
        userAuthorizedResponse.Token = token;
        return userAuthorizedResponse;
    }
    private partial UserAuthorizedResponse ToUserAuthorizedResponse(UserResponse userResponse);

    public partial LoginAndCreateUserRequest ToLoginAndCreateUserCommand(NewUserRequest request);

}