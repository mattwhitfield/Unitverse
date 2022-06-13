namespace Unitverse.Specs
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class MethodBasedStrategyContext
    {
        public MethodBasedStrategyContext(BaseContext baseContext)
        {
            BaseContext = baseContext ?? throw new ArgumentNullException(nameof(baseContext));
        }

        public IEnumerable<BaseMethodDeclarationSyntax> Result { get; set; }

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