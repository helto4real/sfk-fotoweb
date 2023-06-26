using FotoApi.Features.HandleImages.Dto;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Queries;

public record GetUserImageQuery(Guid Id, CurrentUser Owner) : IQuery<ImageResponse>;