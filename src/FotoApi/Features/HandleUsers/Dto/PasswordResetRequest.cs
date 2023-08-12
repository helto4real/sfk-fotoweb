namespace FotoApi.Features.HandleUsers.Dto;

public record PasswordResetRequest(string Email, string Token, string NewPassword);