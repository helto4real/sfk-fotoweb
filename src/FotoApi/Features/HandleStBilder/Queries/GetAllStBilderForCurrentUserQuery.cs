using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleStBilder.Queries;


public record GetAllStBilderForCurrentUserQuery(bool ShowPackagedImages, CurrentUser Owner) : IQuery<List<StBildResponse>>;

