namespace Foto.WebServer.Dto;

public record NewMemberInfo(
    string Email,
    string? PhoneNumber,
    string FirstName,
    string LastName,
    string? Address,
    string? ZipCode);