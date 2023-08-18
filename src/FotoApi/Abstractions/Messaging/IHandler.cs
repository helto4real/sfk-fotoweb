
namespace FotoApi.Abstractions.Messaging;

/// <summary>
///     Common interface for finding and registering handlers
/// </summary>
public interface IBaseHandler;

public interface IHandler<in TCommand, TResponse> : IBaseHandler
{
    public Task<TResponse> Handle(TCommand command, CancellationToken ct = default);
}

public interface IHandler<in TCommand> : IBaseHandler
{
    public Task Handle(TCommand command, CancellationToken ct = default);
}

public interface IEmptyRequestHandler<TResult> : IBaseHandler
{
    public Task<TResult> Handle(CancellationToken cancellationToken = default);
}