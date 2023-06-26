using MediatR;

namespace FotoApi.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}