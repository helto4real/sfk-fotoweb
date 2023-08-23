using Foto.WebServer.Dto;
using Foto.WebServer.Services;

namespace Foto.Tests.Integration.Web.Services;

public class FakeSignInService : ISignInService
{
    public Task<HttpResponseMessage> RefreshTokenOnExpired(Func<Task<HttpResponseMessage>> func, bool doNotSignOutOnUnauthorized = false)
    {
        return func();
    }

    public Task<bool> ValidateAccessTokenAndRefreshIfNeedAsync(HttpClient httpClient)
    {
        return Task.FromResult(true);
    }

    public Task<bool> IsCurrentUserExternalAsync()
    {
        return Task.FromResult(false);
    }

    public Task<(AccountInfo?, ErrorDetail?)> LoginAsync(LoginUserInfo loginUserInfo)
    {
        return Task.FromResult<(AccountInfo?, ErrorDetail?)>((null, null));
    }
}