using System.Collections;
using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IAdminService
{
    Task<UserInfo?> GetUserByUsernameAsync(string username);
    Task<bool> UpdateUserAsync(UserInfo user);
    Task<bool> DeleteUserAsync(string username);
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> CreateUserAsync(NewUserInfo user);
    Task<IEnumerable<UrlToken>> GetCurrentTokens();
    ValueTask DeleteToken(Guid tokenId);
    Task<UrlToken?> AddTokenByTokenType(UrlTokenType tokenType);
}