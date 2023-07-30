namespace FotoApi.Abstractions.Messaging;

public interface IPipeline

{
    public Task<TResponse> Pipe<TRequest, TResponse>(TRequest request, Func<TRequest, CancellationToken, Task<TResponse>> handler, CancellationToken ct= default);
    public Task<TResponse> Pipe<TResponse>(Func<CancellationToken, Task<TResponse>> handler, CancellationToken ct= default);
    public Task Pipe<TRequest>(TRequest request, Func<TRequest, CancellationToken, Task> handler, CancellationToken ct= default);
}

