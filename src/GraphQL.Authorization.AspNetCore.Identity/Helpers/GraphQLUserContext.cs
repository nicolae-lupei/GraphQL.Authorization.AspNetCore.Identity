using System.Collections.Generic;
using System.Security.Claims;

namespace GraphQL.Authorization.AspNetCore.Identity.Helpers
{
    public class GraphQLUserContext : Dictionary<string, object>, IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }
    }
}