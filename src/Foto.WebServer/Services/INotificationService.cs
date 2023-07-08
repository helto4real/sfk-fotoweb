namespace Foto.WebServer.Services;

public delegate Task NotificationEventHandler<in T>(T data);

public interface INotificationService<out T> : IAsyncDisposable
{
    Task RegisterCallback(NotificationEventHandler<T> callback, string topic);
}