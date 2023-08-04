using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;

namespace FotoApi.Infrastructure.Security.Authorization.Policies;

public interface IPolicyChecker
{
    Task<bool> CompliesToPolicy(string policyName);
}

class PolicyChecker(IHttpContextAccessor accessor, IAuthorizationService authService) : IPolicyChecker
{
    public async Task<bool> CompliesToPolicy(string policyName)
    {
        var authorizationRequirement = new OperationAuthorizationRequirement { Name = policyName };
        
        if (accessor.HttpContext is null)
            throw new InvalidOperationException("HttpContext is null");
        
        var result = await authService.AuthorizeAsync(accessor.HttpContext.User, policyName);
        // var result = await authService.AuthorizeAsync(accessor.HttpContext.User, null, authorizationRequirement);
        return result.Succeeded;
    }
}