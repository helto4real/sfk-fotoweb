using FotoApi.Features.HandleUsers.Dto;

namespace FotoApi.Features.HandleUsers.Queries;

public record GetUserFromUsernameQuery(string Username) : IQuery<UserResponse?>
{
}