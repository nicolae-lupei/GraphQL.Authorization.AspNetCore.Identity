using System;
using System.Linq;
using GraphQL.Authorization.AspNetCore.Identity.Enums;
using GraphQL.Authorization.AspNetCore.Identity.ValidationRules;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GraphQL.Authorization.AspNetCore.Identity.Extensions
{
    public static class GraphQLBuilderExtensions
    {
        public static IGraphQLBuilder AddGraphQLAuthorization(this IGraphQLBuilder builder)
        {
            builder.AddGraphQLAuthorization(options =>
            {
                options.AddPolicy(GraphQLPolicy.Authorized.ToString(), p => p.RequireAuthenticatedUser());
            });

            return builder;
        }

        /// <summary>
        /// Adds the GraphQL authorization.
        /// </summary>
        /// <param name="builder">The GraphQL builder.</param>
        /// <param name="options"></param>
        /// <returns>The GraphQL builder.</returns>
        public static IGraphQLBuilder AddGraphQLAuthorization(this IGraphQLBuilder builder, Action<AuthorizationSettings> options)
        {
            builder.Services.TryAddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var authSettings = new AuthorizationSettings();
            options?.Invoke(authSettings);
            builder.Services.TryAddSingleton(authSettings);
            builder
                .Services
                .AddTransient<IValidationRule, AuthorizationValidationRule>()
                .AddTransient<IValidationRule, RolesAuthorizationValidationRule>();
            return builder;
        }

        /// <summary>
        /// Authorize with policy
        /// </summary>
        /// <param name="type"></param>
        /// <param name="policies"></param>
        /// <returns></returns>
        public static IProvideMetadata AuthorizeWith(this IProvideMetadata type, params GraphQLPolicy[] policies)
        {
            type.AuthorizeWith(policies.Select(x => x.ToString()).ToArray());
            return type;
        }
    }
}