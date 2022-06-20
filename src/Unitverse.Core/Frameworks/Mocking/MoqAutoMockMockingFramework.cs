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
    using Unitverse.Core.Options;

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
                classModel.DefaultConstructor.Parameters.All(x => x.TypeInfo.IsInterface());
        }

        public override IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            if (IsActive)
            {
                yield return Generate.UsingDirective("Moq.AutoMock");
            }

            foreach (var usingStatement in base.GetUsings())
            {
                yield return usingStatement;
            }
        }

        public override ExpressionSyntax GetFieldInitializer(TypeSyntax type)
        {
            if (IsActive)
            {
                _context.MocksUsed = true;
                _context.CurrentMethod.AddRequirement(Requirements.AutoMocker);
                return Generate.MemberInvocation("mocker", Generate.GenericName("GetMock", type));
            }

            return base.GetFieldInitializer(type);
        }

        public override ExpressionSyntax? GetObjectCreationExpression(TypeSyntax type)
        {
            if (IsActive)
            {
                _context.MocksUsed = true;
                _context.CurrentMethod.AddRequirement(Requirements.AutoMocker);
                return Generate.MemberInvocation("mocker", Generate.GenericName("CreateInstance", type));
            }

            return base.GetObjectCreationExpression(type);
        }
    }
}