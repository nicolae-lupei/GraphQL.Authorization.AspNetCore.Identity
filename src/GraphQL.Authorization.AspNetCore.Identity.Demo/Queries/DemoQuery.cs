using GraphQL.Authorization.AspNetCore.Identity.Demo.GraphTypes;
using GraphQL.Types;

namespace GraphQL.Authorization.AspNetCore.Identity.Demo.Queries
{
    public class DemoQuery : ObjectGraphType
    {
        public DemoQuery()
        {
            Field<DemoGraphType>("demo", resolve: _ => new
            {
                Id = 1,
                Name = "Random"
            });
        }
    }
}