namespace Foto.WebServer.Dto;
public record User(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    bool IsAdmin);