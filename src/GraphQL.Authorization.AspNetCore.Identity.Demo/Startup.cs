using System.Reflection;
using System.Security.Claims;
using GraphQL.Authorization.AspNetCore.Identity.Demo.Schema;
using GraphQL.Authorization.AspNetCore.Identity.Extensions;
using GraphQL.Authorization.AspNetCore.Identity.Helpers;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace GraphQL.Authorization.AspNetCore.Identity.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GraphQL.Authorization.AspNetCore.Identity.Demo", Version = "v1" });
            });

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
                    var tempPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "test"),
                        new Claim(ClaimTypes.Name, "test"),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.Role, "Manager")
                    }));

                    return new GraphQLUserContext
                    {
                        User = tempPrincipal
                    };
                });

            services.AddScoped<ISchema, DemoSchema>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GraphQL.Authorization.AspNetCore.Identity.Demo v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            const string path = "/api/graphql";
            app.UseGraphQL<ISchema>(path);

            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
            {
                GraphQLEndPoint = path
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
