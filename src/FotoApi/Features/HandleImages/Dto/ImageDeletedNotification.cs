using MediatR;

namespace FotoApi.Features.HandleImages.Dto;

public record ImageDeletedNotification(Guid Id) : INotification;