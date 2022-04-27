namespace Unitverse.Core.Frameworks
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public interface ITestFramework : IAssertionFramework
    {
        bool SupportsStaticTestClasses { get; }

        AttributeSyntax SingleThreadedApartmentAttribute { get; }

        string TestClassAttribute { get; }

        BaseMethodDeclarationSyntax CreateSetupMethod(string targetTypeName);

        SectionedMethodHandler CreateTestCaseMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object> testValues, string description);

        SectionedMethodHandler CreateTestMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic, string description);
    }
}