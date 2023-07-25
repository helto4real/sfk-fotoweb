using FotoApi.Features.HandleStBilder.Commands;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Model;
using Riok.Mapperly.Abstractions;

namespace FotoApi.Features.HandleStBilder.Dto;

[Mapper]
public partial class StBildResponseMapper
{
    public partial StBildResponse ToStBildResponse(StBild stBild);
    public partial StBild ToStBild(NewStBildRequest request);
}

