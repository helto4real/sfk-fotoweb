using FotoApi.Features.HandleUsers.Dto;

namespace FotoApi.Features.HandleUsers.Commands;

public record GetUserFromTokenQuery(string Token) : IQuery<UserResponse>;