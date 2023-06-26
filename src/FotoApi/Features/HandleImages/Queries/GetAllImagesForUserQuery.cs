using FotoApi.Features.HandleImages.Dto;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Queries;

public record GetAllImagesForUserQuery(CurrentUser Owner) : IQuery<List<ImageResponse>>;