using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Foto.WebServer.Api;

public static class CookieApi
{
    public static RouteGroupBuilder MapCookieConsent(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/cookie");

        group.MapGet("consent", (HttpContext context) =>
        {
            if (context?.Features is null)
                throw new NullReferenceException("HttpContext.Features is null");
            ITrackingConsentFeature? consentFeature = context.Features.Get<ITrackingConsentFeature>();
            if (consentFeature is null)
                throw new NullReferenceException("ITrackingConsentFeature is null");
            if (!consentFeature.CanTrack)
            {
                consentFeature.GrantConsent();
            }
            return TypedResults.Redirect("/");
        });

        return group;
    }
}