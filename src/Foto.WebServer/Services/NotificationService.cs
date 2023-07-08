using Foto.WebServer.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Foto.WebServer.Services;

public class NotificationService<T> : INotificationService<T>
{
    private readonly NavigationManager _navigationManager;
    private readonly IHttpContextAccessor _accessor;
    private NotificationEventHandler<T>? _callback;
    private HubConnection? _hubConnection;
    
    public NotificationService(NavigationManager navigationManager, IHttpContextAccessor accessor)
    {
        _navigationManager = navigationManager;
        _accessor = accessor;
    }
    public async Task RegisterCallback(NotificationEventHandler<T> callback, string topic)
    {
        var authResult = await _accessor.HttpContext!.AuthenticateAsync();
        var properties = authResult.Properties!;

        var token = properties.GetTokenValue(TokenNames.AccessToken);
        _callback = callback;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri($"/signalr?access_token={token}"))
            .WithAutomaticReconnect()
            .Build();
        
        await _hubConnection.StartAsync();
        
        _hubConnection.On<T>(topic, async (message) =>
        {
            await _callback(message);
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}