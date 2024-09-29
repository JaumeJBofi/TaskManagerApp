using Microsoft.AspNetCore.Authorization;
using TaskManagerApi.Utilities;

namespace TaskManagerApi.Handlers
{
    public class AdminAuthorizationHandler : AuthorizationHandler<IAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       IAuthorizationRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == ClaimsConstants.Role && c.Value == RoleConstants.Admin))
            {
                context.Succeed(requirement); // Automatically succeed if the user is an Admin
            }

            return Task.CompletedTask;
        }
    }
}
