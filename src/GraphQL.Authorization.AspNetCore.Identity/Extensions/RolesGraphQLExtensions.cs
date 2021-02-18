using System.Collections.Generic;
using System.Linq;
using GraphQL.Authorization.AspNetCore.Identity.Helpers;
using GraphQL.Builders;
using GraphQL.Types;

namespace GraphQL.Authorization.AspNetCore.Identity.Extensions
{
    public static class RolesGraphQLExtensions
    {
        /// <summary>
        /// Check if field or operation require to verify roles requirement
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool RequiresRolesAuthorization(this IProvideMetadata type)
        {
            var permissions = type.GetMetadata<IEnumerable<string>>(GraphQLResources.RolesMetadataKey, new List<string>());
            return permissions.Any();
        }

        /// <summary>
        /// Indicate that node type require to be validated with the following roles  
        /// </summary>
        /// <param name="type"></param>
        /// <param name="requiredRoles"></param>
        public static void RequireRoles(this IProvideMetadata type, params string[] requiredRoles)
        {
            var roles = type.GetMetadata<HashSet<string>>(GraphQLResources.RolesMetadataKey);

            if (roles == null)
            {
                roles = new HashSet<string>();
                type.Metadata[GraphQLResources.RolesMetadataKey] = roles;
            }

            foreach (var role in requiredRoles)
            {
                roles.Add(role);
            }
        }

        public static FieldBuilder<TSourceType, TReturnType> RequireRoles<TSourceType, TReturnType>(this FieldBuilder<TSourceType, TReturnType> builder, params string[] roles)
        {
            builder.FieldType.RequireRoles(roles);
            return builder;
        }
    }
}