namespace Unitverse.Core.Frameworks
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Options;

    public interface ITestFramework : IAssertionFramework
    {
        bool SupportsStaticTestClasses { get; }

        AttributeSyntax SingleThreadedApartmentAttribute { get; }

        string TestClassAttribute { get; }

        BaseMethodDeclarationSyntax CreateSetupMethod(string targetTypeName);

        MethodDeclarationSyntax CreateTestCaseMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object> testValues);

        MethodDeclarationSyntax CreateTestMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic);
    }
}