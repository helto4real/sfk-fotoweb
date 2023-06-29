using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using MediatR;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public class LoginAndCreateUserHandler : ICommandHandler<LoginAndCreateUserCommand, UserAuthorizedResponse>
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;
    private LoginMapper _mapper = new();
    public LoginAndCreateUserHandler(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }
    public async Task<UserAuthorizedResponse> Handle(LoginAndCreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(_mapper.ToCreateUserCommand(request), cancellationToken);
        var authorizedUserResponse =
            _mapper.ToUserAuthorizedResponse(result, _tokenService.GenerateToken(result.UserName, result.IsAdmin));
        return authorizedUserResponse;
    }
}