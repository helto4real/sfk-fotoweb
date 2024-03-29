﻿using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Infrastructure.Security.Authorization.QueryHandlers;

public class GetRolesHandler(RoleManager<Role> roleManager) : IEmptyRequestHandler<IReadOnlyCollection<RoleResponse>>
{
    public async Task<IReadOnlyCollection<RoleResponse>> Handle(CancellationToken cancellationToken = default)
    {
        var result = from r in roleManager.Roles
            orderby r.SortOrder
            select new RoleResponse
            {
                Name = r.Name ?? string.Empty
            }; 
        return await result.ToListAsync(cancellationToken);
    }
}