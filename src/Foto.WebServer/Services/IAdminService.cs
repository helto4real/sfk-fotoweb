﻿using System.Collections.ObjectModel;
using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IAdminService
{
  
    Task<IEnumerable<UrlToken>> GetCurrentTokens();
    ValueTask DeleteToken(Guid tokenId);
    Task<UrlToken?> AddTokenByTokenType(UrlTokenType tokenType);
    
    Task<(ReadOnlyCollection<RoleInfo>?, ErrorDetail?)> GetRolesAsync();
}