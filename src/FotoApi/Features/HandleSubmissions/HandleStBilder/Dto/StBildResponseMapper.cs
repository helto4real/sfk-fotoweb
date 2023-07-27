using FotoApi.Features.HandleSubmissions.HandleStBilder.Commands;
using FotoApi.Model;
using Riok.Mapperly.Abstractions;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;

[Mapper]
public partial class StBildResponseMapper
{
    public partial StBildResponse ToStBildResponse(StBild stBild);
    public partial StBild ToStBild(NewStBildRequest request);
}

