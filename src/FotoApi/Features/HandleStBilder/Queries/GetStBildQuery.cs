using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleStBilder.Queries;

public record GetStBildQuery(Guid Id, CurrentUser Owner) : IQuery<StBildResponse>
{
}