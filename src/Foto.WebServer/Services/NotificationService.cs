using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Foto.WebServer.Services;

public class NotificationService<T>(NavigationManager navigationManager, IHttpContextAccessor accessor,
        IAuthService authService)
    : INotificationService<T>
{
    private NotificationEventHandler<T>? _callback;
    private HubConnection? _hubConnection;
    private readonly CancellationTokenSource _cancelSource = new();
    public async Task RegisterCallback(NotificationEventHandler<T> callback, string topic)
    {
        var authResult = await accessor.HttpContext!.AuthenticateAsync();
        var properties = authResult.Properties!;

        var refreshToken = properties.GetTokenValue(TokenNames.AccessToken);

        if (refreshToken is null)
            return; // Cookie is not present, weird!

        var (accessToken, _) =
            authService.GetAccessTokenFromRefreshToken(refreshToken, authResult.Principal!.Identity!.Name!);
        if (accessToken is null)
            return; //Fail authorize the user

        _callback = callback;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri($"/signalr?access_token={accessToken}"))
            .WithAutomaticReconnect()
            .Build();

        await _hubConnection.StartAsync(_cancelSource.Token);

        _hubConnection.On<T>(topic, async message => { await _callback(message); });
    }

    public async ValueTask DisposeAsync()
    {
        await _cancelSource.CancelAsync();
        if (_hubConnection is not null) await _hubConnection.DisposeAsync();
    }
}