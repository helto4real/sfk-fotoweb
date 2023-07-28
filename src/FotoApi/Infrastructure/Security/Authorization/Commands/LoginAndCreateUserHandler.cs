using FotoApi.Features.HandleUsers.CommandHandlers;
using FotoApi.Infrastructure.Pipelines;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using MediatR;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public class LoginAndCreateUserHandler(
        ITokenService tokenService, 
        CreateUserHandler createUserHandler,
        FotoAppPipeline pipe) 
    : IHandler<LoginAndCreateUserRequest, UserAuthorizedResponse>
{
    private readonly LoginMapper _mapper = new();

    public async Task<UserAuthorizedResponse> Handle(LoginAndCreateUserRequest request, CancellationToken ct)
    {
        var result = await pipe.Pipe(_mapper.ToCreateUserCommand(request),
            createUserHandler.Handle, ct);
        var authorizedUserResponse =
            _mapper.ToUserAuthorizedResponse(result, tokenService.GenerateToken(result.UserName, result.IsAdmin));
        return authorizedUserResponse;
    }
}