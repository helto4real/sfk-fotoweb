using FotoApi.Model;
using Riok.Mapperly.Abstractions;

namespace FotoApi.Features.HandleUrlTokens.Dto;

[Mapper]
public partial class UrlTokenMapper
{
    public partial UrlTokenResponse ToUrlTokenResponse(UrlToken urlToken);
}