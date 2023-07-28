using Foto.WebServer.Services;

namespace Foto.Tests.Integration.Web.Services;

public class FakeSignInService : ISignInService
{
    public Task<HttpResponseMessage> RefreshTokenOnExpired(Func<Task<HttpResponseMessage>> func)
    {
        return func();
    }
}