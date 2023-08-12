namespace FotoApi.Features.HandleUsers.Dto;

public record NewPasswordResetNotification(string Email, string Token);