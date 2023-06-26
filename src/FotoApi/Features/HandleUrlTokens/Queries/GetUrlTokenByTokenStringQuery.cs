using FotoApi.Features.HandleUrlTokens.Model;

namespace FotoApi.Features.HandleUrlTokens.Queries;

public record GetUrlTokenByTokenStringQuery(string TokenString) : IQuery<UrlTokenResponse>;