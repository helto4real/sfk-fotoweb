using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleStBilder.Queries;

public record GetAllPackageStBilderQuery(CurrentUser Owner) :  IQuery<List<StBildResponse>>;