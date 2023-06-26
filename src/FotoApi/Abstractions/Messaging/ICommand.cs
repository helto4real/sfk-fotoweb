using MediatR;

namespace FotoApi.Abstractions.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
    
}

public interface ICommand : IRequest
{
    
}