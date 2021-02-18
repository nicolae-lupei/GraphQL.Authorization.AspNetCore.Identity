using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Authorization.AspNetCore.Identity.Extensions;
using GraphQL.Authorization.AspNetCore.Identity.Helpers;
using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation;

namespace GraphQL.Authorization.AspNetCore.Identity.ValidationRules
{
    public class RolesAuthorizationValidationRule : IValidationRule
    {
        public Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            var userContext = context.UserContext as IProvideClaimsPrincipal;

            return Task.FromResult((INodeVisitor)new EnterLeaveListener(_ =>
            {
                var operationType = OperationType.Query;

                _.Match<Operation>(astType =>
                {
                    operationType = astType.OperationType;

                    var type = context.TypeInfo.GetLastType();
                    CheckRoles(astType, type, userContext, context, operationType);
                });

                _.Match<ObjectField>(objectFieldAst =>
                {
                    if (context.TypeInfo.GetArgument()?.ResolvedType.GetNamedType() is IComplexGraphType argumentType)
                    {
                        var fieldType = argumentType.GetField(objectFieldAst.Name);
                        CheckRoles(objectFieldAst, fieldType, userContext, context, operationType);
                    }
                });

                _.Match<Field>(fieldAst =>
                {
                    var fieldDef = context.TypeInfo.GetFieldDef();

                    if (fieldDef == null)
                        return;
                    // check target field
                    CheckRoles(fieldAst, fieldDef, userContext, context, operationType);
                    // check returned graph type
                    CheckRoles(fieldAst, fieldDef.ResolvedType.GetNamedType(), userContext, context, operationType);
                });
            }));
        }

        private static void CheckRoles(
            INode node,
            IProvideMetadata type,
            IProvideClaimsPrincipal userContext,
            ValidationContext context,
            OperationType operationType)
        {
            if (type == null || context.HasErrors || !type.RequiresRolesAuthorization())
                return;

            var requiredRoles = type.Metadata[GraphQLResources.RolesMetadataKey] as HashSet<string> ?? new HashSet<string>();
            var userRoles = userContext?.User.Claims
                .Where(x => x.Type.Equals("role") || x.Type.EndsWith("role"))
                .Select(x => x.Value)
                .ToHashSet() ?? new HashSet<string>();

            if (requiredRoles.All(x => userRoles.Contains(x))) return;

            context.ReportError(new ValidationError(
                context.OriginalQuery,
                "authorization",
                $"You are not authorized to run this {operationType.ToString().ToLower()}.\n You do not have the necessary roles",
                node));
        }
    }
}