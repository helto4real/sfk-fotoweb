using Foto.WebServer.Services;

namespace Foto.Tests.Integration.Web.Services;

public class FakeSignInService : ISignInService
{
    public Task<HttpResponseMessage> RefreshTokenOnExpired(Func<Task<HttpResponseMessage>> func)
    {
        return func();
    }

    public Task<bool> ValidateAccessTokenAndRefreshIfNeedAsync(HttpClient httpClient)
    {
        return Task.FromResult(true);
    }
}