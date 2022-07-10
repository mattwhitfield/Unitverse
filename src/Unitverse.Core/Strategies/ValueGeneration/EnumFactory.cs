namespace Unitverse.Core.Strategies.ValueGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;

    public static class EnumFactory
    {
        public static ExpressionSyntax Random(ITypeSymbol typeSymbol, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet)
        {
            if (typeSymbol == null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            if (frameworkSet == null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            var enumMembers = typeSymbol.GetMembers().OfType<IFieldSymbol>().Select(x => x.Name).ToList();

            var identifier = enumMembers[ValueGenerationStrategyFactory.Random.Next(enumMembers.Count)];

            return Generate.MemberAccess(typeSymbol.ToTypeSyntax(frameworkSet.Context), identifier);
        }
    }
}