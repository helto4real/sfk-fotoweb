using System.Security.Principal;
using FotoApi.Infrastructure.Security.Authorization.Dto;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public record RefreshTokenCommand(string RefreshToken, string Username) : ICommand<UserAuthorizedResponse?>;