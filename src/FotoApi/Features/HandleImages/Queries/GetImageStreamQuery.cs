using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Queries;

public record GetImageStreamQuery(Guid Id, CurrentUser Owner, bool IsThumbnail) : IQuery<FileStream>;