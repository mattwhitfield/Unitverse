namespace Unitverse.Core.Frameworks
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public interface IFrameworkSet : IClassModelEvaluator
    {
        IUnitTestGeneratorOptions Options { get; }

        ITestFramework TestFramework { get; }

        IMockingFramework MockingFramework { get; }

        IAssertionFramework AssertionFramework { get; }

        INamingProvider NamingProvider { get; }

        IGenerationContext Context { get; }

        SectionedMethodHandler CreateSetupMethod(string targetTypeName, string className);

        SectionedMethodHandler CreateTestCaseMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object?> testValues, string description);

        SectionedMethodHandler CreateTestMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic, string description);
    }
}