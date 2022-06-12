namespace Unitverse.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;

    public class MoqAutoMockMockingFramework : MoqMockingFramework, IClassModelEvaluator
    {
        private readonly IGenerationContext _context;

        public MoqAutoMockMockingFramework(IGenerationContext context)
            : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool IsActive { get; private set; }

        public void EvaluateTargetModel(ClassModel classModel)
        {
            // activate if the class model primary constructor takes all interface params, or there is no primary constructor
            IsActive =
                classModel.DefaultConstructor == null ||
                classModel.DefaultConstructor.Parameters.All(x => x.TypeInfo.Type.TypeKind == TypeKind.Interface);
        }

        public override IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            if (IsActive)
            {
                yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Moq.AutoMock"));
            }

            foreach (var usingStatement in base.GetUsings())
            {
                yield return usingStatement;
            }
        }

        public override void AddSetupMethodStatements(SectionedMethodHandler setupMethod)
        {
            if (IsActive)
            {
                _context.MocksUsed = true;
                var creation = Generate.ImplicitlyTypedVariableDeclaration("mocker", Generate.ObjectCreation(SyntaxFactory.IdentifierName("AutoMocker")));
                setupMethod.Emit(creation);
            }
        }

        public override ExpressionSyntax GetFieldInitializer(TypeSyntax type)
        {
            if (IsActive)
            {
                var typeList = SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type));

                _context.MocksUsed = true;
                return SyntaxFactory.InvocationExpression(
                           SyntaxFactory.MemberAccessExpression(
                               SyntaxKind.SimpleMemberAccessExpression,
                               SyntaxFactory.IdentifierName("mocker"),
                               SyntaxFactory.GenericName(SyntaxFactory.Identifier("GetMock"))
                                            .WithTypeArgumentList(typeList)));
            }

            return base.GetFieldInitializer(type);
        }

        public override ExpressionSyntax GetObjectCreationExpression(TypeSyntax type)
        {
            if (IsActive)
            {
                var typeList = SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type));

                _context.MocksUsed = true;
                return SyntaxFactory.InvocationExpression(
                           SyntaxFactory.MemberAccessExpression(
                               SyntaxKind.SimpleMemberAccessExpression,
                               SyntaxFactory.IdentifierName("mocker"),
                               SyntaxFactory.GenericName(SyntaxFactory.Identifier("CreateInstance"))
                                            .WithTypeArgumentList(typeList)));
            }

            return base.GetObjectCreationExpression(type);
        }
    }
}