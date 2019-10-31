namespace SentryOne.UnitTestGenerator.Specs.Strategies
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    public class ClassBasedStrategyContext
    {
        public ClassBasedStrategyContext(BaseContext baseContext)
        {
            BaseContext = baseContext ?? throw new ArgumentNullException(nameof(baseContext));
        }
        public ClassDeclarationSyntax Result { get; set; }
        public ClassDeclarationSyntax CurrentClass { get; set; }
        private BaseContext BaseContext { get; }
        public SemanticModel TestModel { get; set; }
        public TestFrameworkTypes TargetFramework => BaseContext.TargetFramework;
        public MockingFrameworkType MockFramework => BaseContext.MockFramework;
        public ClassModel ClassModel => BaseContext.ClassModel;
        public MethodDeclarationSyntax CurrentMethod
        {
            get
            {
                return BaseContext.CurrentMethod;
            }
            set
            {
                BaseContext.CurrentMethod = value;
            }
        }
        public SemanticModel SemanticModel
        {
            get
            {
                return BaseContext.SemanticModel;
            }
            set
            {
                BaseContext.SemanticModel = value;
            }
        }
    }
}