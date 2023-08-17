using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace FotoApi.Infrastructure.Security.Authentication;

public class AuthException(string message) : HubException(message);

public class AuthHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next
    )
    {
        var feature = invocationContext.Context.Features.Get<IConnectionHeartbeatFeature>();
        if (feature == null)
        {
            return await next(invocationContext);
        }

        var context = invocationContext.Context.GetHttpContext();
        if (context == null)
        {
            throw new InvalidOperationException("The HTTP context cannot be resolved.");
        }
    
        // Extract the authentication ticket from the access token.
        // Note: this operation should be cheap as the authentication result
        // was already computed when SignalR invoked the authentication handler
        // and automatically cached by AuthenticationHandler.AuthenticateAsync().
        var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (result.Ticket == null)
        {
            invocationContext.Context.Abort();
    
            await next(invocationContext);
        }
    
        feature.OnHeartbeat(state =>
        {
            var (ticket, hubConnectionContext) = ((AuthenticationTicket, HubConnectionContext)) state;
    
            // Ensure the access token token is still valid.
            // If it's not, abort the connection immediately.
            if (ticket.Properties.ExpiresUtc < DateTimeOffset.UtcNow)
            {
                hubConnectionContext.Abort();
            }
        }, (result.Ticket, invocationContext.Context));
        
        return await next(invocationContext);
    }

    public Task OnConnectedAsync(
        HubLifetimeContext context,
        Func<HubLifetimeContext, Task> next
    )
    {
        return next(context);
    }

    public Task OnDisconnectedAsync(
        HubLifetimeContext context,
        Exception? exception,
        Func<HubLifetimeContext, Exception?, Task> next
    )
    {
        return next(context, exception);
    }
}