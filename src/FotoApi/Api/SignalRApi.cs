using FotoApi.Features.SignalR;
using MediatR;

namespace FotoApi.Api;

using Microsoft.AspNetCore.SignalR;

public class SignalRApi : Hub, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    public override async Task OnConnectedAsync()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(-1, _cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _cancellationTokenSource.CancelAsync();
        await base.OnDisconnectedAsync(exception);
    }

    public new void Dispose()
    {
        _cancellationTokenSource.Cancel();
        base.Dispose();
    }
}