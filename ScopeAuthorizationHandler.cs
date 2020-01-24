using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace JWT
{
    public class ScopeAuthorizationHandler : AuthorizationHandler<ScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ScopeRequirement requirement)
        {
            var scopeClaim = context.User.FindFirst("scope");
            if (scopeClaim != null && !string.IsNullOrEmpty(scopeClaim.Value))
            {
                if (scopeClaim.Value.Contains(requirement.Scope))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }


            return Task.CompletedTask;
        }
    }

    public class ScopeRequirement : IAuthorizationRequirement
    {
        public string Scope { get; }

        public ScopeRequirement(string scope)
        {
            Scope = scope;
        }
    }
}
