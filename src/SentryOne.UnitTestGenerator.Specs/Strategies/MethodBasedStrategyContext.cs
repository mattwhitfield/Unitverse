namespace SentryOne.UnitTestGenerator.Specs.Strategies
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    public class MethodBasedStrategyContext
    {
        public MethodBasedStrategyContext(BaseContext baseContext)
        {
            BaseContext = baseContext ?? throw new ArgumentNullException(nameof(baseContext));
        }

        public IEnumerable<MethodDeclarationSyntax> Result { get; set; }

        private BaseContext BaseContext { get; }

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
    }
}