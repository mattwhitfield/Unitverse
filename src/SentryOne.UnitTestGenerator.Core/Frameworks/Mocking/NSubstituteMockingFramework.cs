namespace SentryOne.UnitTestGenerator.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class NSubstituteMockingFramework : IMockingFramework
    {
        private readonly IGenerationContext _context;

        public NSubstituteMockingFramework(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ExpressionSyntax GetFieldReference(ExpressionSyntax fieldReference)
        {
            return fieldReference;
        }

        public TypeSyntax GetFieldType(TypeSyntax type)
        {
            return type;
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.NSubstituteMockingFramework_GetUsings_NSubstitute));
        }

        public ExpressionSyntax GetThrowawayReference(TypeSyntax type)
        {
            return GetFieldInitializer(type);
        }

        public ExpressionSyntax GetFieldInitializer(TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _context.MocksUsed = true;
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("Substitute"),
                    SyntaxFactory.GenericName(
                            SyntaxFactory.Identifier(Strings.NSubstituteMockingFramework_MockInterface_For))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type)))));
        }
    }
}
