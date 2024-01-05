using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AuthFilter : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _minimalRole;

    public AuthFilter(string minimalRole)
    {
        _minimalRole = minimalRole;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            return;
        }

        if (!Auth.Roles.Validate(_minimalRole, user.FindFirst(ClaimTypes.Role)?.Value))
        {
            context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
            return;
        }
    }
}