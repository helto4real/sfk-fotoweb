using FotoApi.Model;
using Riok.Mapperly.Abstractions;

namespace FotoApi.Features.HandleUrlTokens.Model;

[Mapper]
public partial class UrlTokenMapper
{
    public partial UrlTokenResponse ToUrlTokenResponse(UrlToken urlToken);
}