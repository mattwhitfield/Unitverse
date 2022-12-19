namespace Unitverse.Core.Helpers
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class TypeInfoExtensions
    {
        public static bool IsReferenceTypeAndNotString(this ITypeSymbol? symbol)
        {
            return symbol != null && symbol.IsReferenceType && symbol.SpecialType != SpecialType.System_String;
        }

        public static string GetLastNamePart(this ITypeSymbol symbol)
        {
            var name = symbol.ToFullName();
            return GetLastNamePart(name);
        }

        public static string ToFullName(this ISymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            return symbol.ToDisplayString(new SymbolDisplayFormat(
                SymbolDisplayGlobalNamespaceStyle.Omitted,
                SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                SymbolDisplayGenericsOptions.None,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes));
        }

        public static string ToIdentifierName(this ITypeSymbol symbol)
        {
            if (symbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                return "ArrayOf" + arrayTypeSymbol.ElementType.ToIdentifierName().ToPascalCase();
            }

            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.IsGenericType)
                {
                    return GetLastNamePart(namedTypeSymbol.Name) + "Of" + namedTypeSymbol.TypeArguments.Select(x => x.ToIdentifierName().ToPascalCase()).Aggregate((x, y) => x + "And" + y);
                }
            }

            return symbol.GetLastNamePart();
        }

        public static bool IsInterface(this TypeInfo typeInfo)
        {
            return typeInfo.Type != null && typeInfo.Type.TypeKind == TypeKind.Interface;
        }

        public static bool IsWellKnownSequenceInterface(this TypeInfo typeInfo)
        {
            if (typeInfo.Type == null)
            {
                return false;
            }

            var name = typeInfo.Type.ToFullName();
            return name == "System.Collections.Generic.IEnumerable" ||
                   name == "System.Collections.Generic.IList";
        }

        public static TypeSyntax ToTypeSyntax(this TypeInfo typeInfo, IGenerationContext context)
        {
            return typeInfo.Type?.ToTypeSyntax(context) ?? SyntaxFactory.IdentifierName("UnknownType");
        }

        public static TypeSyntax ToTypeSyntax(this ITypeSymbol symbol, IGenerationContext context)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.AddEmittedType(symbol);
            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                foreach (var typeArgument in namedTypeSymbol.TypeArguments)
                {
                    typeArgument.ToTypeSyntax(context);
                    if (typeArgument.Kind == SymbolKind.TypeParameter)
                    {
                        context.AddVisitedGenericType(typeArgument.Name);
                    }
                }
            }

            return SyntaxFactory.ParseTypeName(symbol.ToDisplayString(new SymbolDisplayFormat(
                SymbolDisplayGlobalNamespaceStyle.Omitted,
                SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes)));
        }

        private static string GetLastNamePart(string name)
        {
            int i = name.LastIndexOf('.');
            if (i > 0 && i < name.Length - 1)
            {
                return name.Substring(i + 1);
            }

            return name;
        }
    }
}