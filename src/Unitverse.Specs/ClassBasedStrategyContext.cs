﻿namespace Unitverse.Specs
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

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

        public SemanticModel SemanticModel => BaseContext.SemanticModel;
    }
}