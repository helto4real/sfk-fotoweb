using System.Net.Http.Headers;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Authentication;

namespace Foto.WebServer.Authentication;

public class AuthorizedHttpClientHandler(IAuthService authService, IHttpContextAccessor accessor, ILogger<AuthorizedHttpClientHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authHeader = await GetAuthorizationHeader();
        request.Headers.Authorization = authHeader;
        return await base.SendAsync(request, cancellationToken);
    }
    private async Task<AuthenticationHeaderValue?> GetAuthorizationHeader()
    {
        // First get the refresh token from context
        var authResult = await accessor.HttpContext!.AuthenticateAsync();
        var properties = authResult.Properties!;

        var refreshToken = properties.GetTokenValue(TokenNames.AccessToken);

        if (refreshToken is null)
        {
            logger.LogError("Refreshtoken could not be found in cookie!");
            return null; // Cookie is not present, weird!
        }
        
        // Then we get a new access token from the refresh token
        var (accessToken, _) =
            authService.GetAccessTokenFromRefreshToken(refreshToken, authResult.Principal!.Identity!.Name!);
       
        if (accessToken is not null)
        {
            return new AuthenticationHeaderValue("Bearer", accessToken);
        }
            
        return null; //Fail authorize the user
    }
}