using GraphQL.Authorization.AspNetCore.Identity.Enums;
using GraphQL.Authorization.AspNetCore.Identity.Extensions;
using GraphQL.Types;

namespace GraphQL.Authorization.AspNetCore.Identity.Demo.GraphTypes
{
    public class DemoGraphType : ObjectGraphType<object>
    {
        public DemoGraphType()
        {
            Field<IntGraphType>("id");
            Field<StringGraphType>("name")
                //.AuthorizeWith(GraphQLPolicy.Authorized)
                .RequireRoles("Admin");
        }
    }
}