namespace FotoApi.Infrastructure.Security.Authorization.Dto;

public record RoleResponse
{
    public string Name { get; init; } = default!;
}