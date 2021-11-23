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

    public class MoqMockingFramework : IMockingFramework
    {
        private readonly IGenerationContext _context;

        public MoqMockingFramework(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.MoqMockingFramework_GetUsings_Moq));
        }

        public TypeSyntax GetFieldType(TypeSyntax type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(Strings.MoqMockingFramework_MockInterface_Mock))
                                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type)));
        }

        public ExpressionSyntax GetFieldReference(ExpressionSyntax fieldReference)
        {
            if (fieldReference is null)
            {
                throw new ArgumentNullException(nameof(fieldReference));
            }

            _context.MocksUsed = true;
            return SyntaxFactory.MemberAccessExpression(
                     SyntaxKind.SimpleMemberAccessExpression,
                     fieldReference,
                     SyntaxFactory.IdentifierName("Object"));
        }

        public ExpressionSyntax GetThrowawayReference(TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _context.MocksUsed = true;
            return SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    GetFieldInitializer(type),
                    SyntaxFactory.IdentifierName("Object"));
        }

        public ExpressionSyntax GetFieldInitializer(TypeSyntax type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _context.MocksUsed = true;
            return SyntaxFactory.ObjectCreationExpression(SyntaxFactory.GenericName(SyntaxFactory.Identifier(Strings.MoqMockingFramework_MockInterface_Mock))
                                                                       .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type))))
                                .WithArgumentList(SyntaxFactory.ArgumentList());
        }
    }
}