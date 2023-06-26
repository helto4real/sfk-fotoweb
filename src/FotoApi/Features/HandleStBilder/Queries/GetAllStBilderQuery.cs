using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleStBilder.Queries;


public record GetAllStBilderQuery(bool UseOnlyCurrentUserImages, CurrentUser Owner) : IQuery<List<StBildResponse>>;

