namespace Unitverse.Core.Templating.Model.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;

    public static class Helpers
    {
        public static IEnumerable<AttributeFilterModel> GetAttributeModels(this ParameterSyntax parameter, SemanticModel model)
        {
            return GetAttributeModels(parameter.AttributeLists, model);
        }

        public static IEnumerable<AttributeFilterModel> GetAttributeModels(this MemberDeclarationSyntax memberDeclaration, SemanticModel model)
        {
            return GetAttributeModels(memberDeclaration.AttributeLists, model);
        }

        public static IEnumerable<AttributeFilterModel> GetAttributeModels(SyntaxList<AttributeListSyntax> attributeLists, SemanticModel model)
        {
            foreach (var attribute in attributeLists.SelectMany(x => x.Attributes))
            {
                var typeSymbol = model.GetNamedTypeSymbol(attribute);
                if (typeSymbol != null)
                {
                    yield return new AttributeFilterModel(typeSymbol);
                }
            }
        }

        public static TypeFilterModel? GetTypeModel(this SyntaxNode? node, SemanticModel model)
        {
            TypeFilterModel? typeModel = null;

            if (node != null)
            {
                var typeSymbol = model.GetNamedTypeSymbol(node);
                if (typeSymbol != null)
                {
                    typeModel = new TypeFilterModel(typeSymbol);
                }
            }

            return typeModel;
        }
    }
}
