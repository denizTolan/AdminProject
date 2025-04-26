namespace AdminProject.Authorization;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        var hasClaim = context.User.HasClaim(c =>
            c.Type == "Permission" && c.Value == requirement.Permission);

        if (hasClaim)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
