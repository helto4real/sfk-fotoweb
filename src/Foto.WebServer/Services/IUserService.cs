using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IUserService
{
    Task<(UserInfo?, ErrorDetail?)> GetUserByUsernameAsync(string username);
    Task<(UserInfo?, ErrorDetail?)> GetCurrentUserAsync();
    Task<ErrorDetail?> UpdateUserAsync(UserInfo user);
    Task<ErrorDetail?> DeleteUserAsync(string username);
    Task<(IEnumerable<User>?, ErrorDetail?)> GetAllUsers();
    Task<(User?, ErrorDetail?)> CreateUserAsync(NewUserInfo user);
    Task<(User?, ErrorDetail?)> PreCreateUserAsync(string? email);
    Task<ErrorDetail?> ConfirmEmailAsync(string token);
}