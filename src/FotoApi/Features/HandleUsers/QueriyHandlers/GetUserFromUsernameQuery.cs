using FotoApi.Features.HandleUsers.Dto;

namespace FotoApi.Features.HandleUsers.QueriyHandlers;

public record GetUserFromUsernameQuery(string Username) : IQuery<UserResponse?>
{
}