namespace SentryOne.UnitTestGenerator.Core.Strategies.ClassDecoration
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;

    internal class RequiresStaGenerationStrategy : IClassDecorationStrategy
    {
        private readonly IFrameworkSet _frameworkSet;

        public RequiresStaGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 100;

        public TypeDeclarationSyntax Apply(TypeDeclarationSyntax declaration, ClassModel model)
        {
            var typeInfo = model.SemanticModel.GetDeclaredSymbol(model.Declaration);
            if (typeInfo == null)
            {
                return declaration;
            }

            bool requiresAttribute = false;
            while (typeInfo != null)
            {
                if (string.Equals(typeInfo.ToFullName(), "System.Windows.Forms.Control", StringComparison.OrdinalIgnoreCase))
                {
                    requiresAttribute = true;
                    break;
                }

                if (string.Equals(typeInfo.ToFullName(), "System.Windows.DependencyObject", StringComparison.OrdinalIgnoreCase))
                {
                    requiresAttribute = true;
                    break;
                }

                typeInfo = typeInfo.BaseType;
            }

            if (requiresAttribute)
            {
                var attribute = _frameworkSet.TestFramework.SingleThreadedApartmentAttribute;
                if (attribute != null)
                {
                    declaration = declaration.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute)));
                }
            }

            return declaration;
        }

        public bool CanHandle(TypeDeclarationSyntax existingSyntax, ClassModel model)
        {
            return true;
        }
    }
}