using MediatR;

namespace FotoApi.Features.SignalR;

public record SignalRNotification(string Message) : INotification;