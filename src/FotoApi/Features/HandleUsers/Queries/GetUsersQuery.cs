using FotoApi.Features.HandleUsers.Dto;

namespace FotoApi.Features.HandleUsers.Queries;

public record GetUsersQuery : IQuery<List<UserResponse>>;