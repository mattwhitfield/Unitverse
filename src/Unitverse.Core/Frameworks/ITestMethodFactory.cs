namespace Unitverse.Core.Frameworks
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public interface ITestMethodFactory
    {
        SectionedMethodHandler CreateSetupMethod(string targetTypeName, string className);

        SectionedMethodHandler CreateTestCaseMethod(NameResolver nameResolver, NamingContext namingContext, IGenerationContext generationContext, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object?> testValues, string description);

        SectionedMethodHandler CreateTestMethod(NameResolver nameResolver, NamingContext namingContext, IGenerationContext generationContext, bool isAsync, bool isStatic, string description);
    }
}