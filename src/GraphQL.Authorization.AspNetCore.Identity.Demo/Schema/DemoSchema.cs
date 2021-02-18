using System;
using GraphQL.Authorization.AspNetCore.Identity.Demo.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.Authorization.AspNetCore.Identity.Demo.Schema
{
    public class DemoSchema : GraphQL.Types.Schema
    {
        public DemoSchema(IServiceProvider resolver) : base(resolver)
        {
            Query = resolver.GetRequiredService<DemoQuery>();
        }
    }
}