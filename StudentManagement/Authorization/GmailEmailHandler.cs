using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagement.Authorization
{
    public class GmailEmailHandler : AuthorizationHandler<GmailEmailRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GmailEmailRequirement requirement)
        {
            var email = context.User.FindFirstValue(ClaimTypes.Email);

            if (!string.IsNullOrEmpty(email) && email.EndsWith("@gmail.com"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
