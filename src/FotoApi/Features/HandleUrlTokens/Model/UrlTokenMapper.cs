using FotoApi.Features.HandleUrlTokens.Commands;
using FotoApi.Model;
using Riok.Mapperly.Abstractions;

namespace FotoApi.Features.HandleUrlTokens.Model;

[Mapper]
public partial class UrlTokenMapper
{
    public partial AddUrlTokenFromUrlTokenTypeCommand ToAddUrlTokenFromUrlTokenTypeCommand(
        UrlTokenTypeRequest urlTokenRequest);

    public partial UrlTokenResponse ToUrlTokenResponse(UrlToken urlToken);
}