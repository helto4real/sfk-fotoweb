namespace FotoApi.Features.HandleUsers.Dto;

public record UpdateLoginInfoResponse
{
    public bool EmailUpdated { get; init; }
    public bool PasswordUpdated { get; init; }
}