using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IUserService
{
    Task<UserInfo?> GetUserByUsernameAsync(string username);
    Task<bool> UpdateUserAsync(UserInfo user);
    Task<bool> DeleteUserAsync(string username);
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> CreateUserAsync(NewUserInfo user);
    Task<(User?, ErrorDetail?)> PreCreateUserAsync(string? email);
    Task<bool> ConfirmEmailAsync(string token);
}