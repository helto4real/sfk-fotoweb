using Microsoft.AspNetCore.Http.Features;

namespace Foto.WebServer.Api;

public static class CookieApi
{
    public static RouteGroupBuilder MapCookieConsentApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/cookie");

        group.MapGet("consent", (HttpContext context) =>
        {
            var consentFeature = context.Features.Get<ITrackingConsentFeature>();
            if (consentFeature is null)
                throw new NullReferenceException("ITrackingConsentFeature is null");
            if (!consentFeature.CanTrack) consentFeature.GrantConsent();
            return TypedResults.Redirect("/");
        });

        return group;
    }
}