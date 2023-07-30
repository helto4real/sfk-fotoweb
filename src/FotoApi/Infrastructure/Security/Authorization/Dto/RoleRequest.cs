namespace FotoApi.Infrastructure.Security.Authorization.Dto;

public record RoleRequest
{
    public string Name { get; init; } = default!;
}