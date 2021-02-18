# GraphQL.Authorization.AspNetCore.Identity

[![Build Status](https://travis-ci.com/nicolae-lupei/GraphQL.Authorization.AspNetCore.Identity.svg?branch=main)](https://travis-ci.com/nicolae-lupei/GraphQL.Authorization.AspNetCore.Identity)

![.NET](https://github.com/nicolae-lupei/GraphQL.Authorization.AspNetCore.Identity/workflows/.NET/badge.svg)

[![NuGet](https://img.shields.io/nuget/v/GraphQL.Authorization.AspNetCore.Identity.svg)](https://www.nuget.org/packages/GraphQL.Authorization.AspNetCore.Identity)
[![Nuget](https://img.shields.io/nuget/dt/GraphQL.Authorization.AspNetCore.Identity)](https://www.nuget.org/packages/GraphQL.Authorization.AspNetCore.Identity)

GraphQL extensions for authorization through IdentityRoles

# Examples

### Requirements:

```c#
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization();

            services.AddGraphQL(x =>
                {
                    x.EnableMetrics = true;
                })
                .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = true)
                .AddGraphTypes(ServiceLifetime.Scoped)
                .AddDataLoader()
                .AddSystemTextJson()
                .AddGraphQLAuthorization()
                .AddUserContextBuilder(ctx =>
                {
                    var principalProvider = ctx.RequestServices.GetRequiredService<IHttpContextAccessor>();
                    var principal = principalProvider?.HttpContext?.User;

                    return new GraphQLUserContext
                    {
                        User = principal
                    };
                });

        }
```

1. Fully functional [web demo](src/GraphQL.Authorization.AspNetCore.Identity.Demo).
2. GraphType syntax - use `AuthorizeWith`.

```c#
public class DemoGraphType : ObjectGraphType<object>
{
    this.AuthorizeWith(GraphQLPolicy.Authorized);
    public DemoGraphType()
    {
        Field<IntGraphType>("id");
        Field<StringGraphType>("name")
            .AuthorizeWith(GraphQLPolicy.Authorized);
    }
}

```

3. GraphType syntax - use `RequireRoles`

```c#
public class DemoGraphType : ObjectGraphType<object>
{
    public DemoGraphType()
    {
        Field<IntGraphType>("id");
        Field<StringGraphType>("name")
            .RequireRoles("Admin");
    }
}
```
