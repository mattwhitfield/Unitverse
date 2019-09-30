namespace SentryOne.UnitTestGenerator.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class RhinoMocksMockingFramework : IMockingFramework
    {
        private readonly IGenerationContext _context;

        public RhinoMocksMockingFramework(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.RhinoMocksMockingFramework_GetUsings_Rhino_Mocks));
        }

        public ExpressionSyntax MockInterface(TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _context.MocksUsed = true;
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("MockRepository"),
                    SyntaxFactory.GenericName(
                            SyntaxFactory.Identifier(Strings.RhinoMocksMockingFramework_MockInterface_GenerateStub))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type)))));
        }

        public IEnumerable<INugetPackageReference> ReferencedNugetPackages(IVersioningOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            yield return new NugetPackageReference("RhinoMocks", options.RhinoMocksNugetPackageVersion);
        }
    }
}