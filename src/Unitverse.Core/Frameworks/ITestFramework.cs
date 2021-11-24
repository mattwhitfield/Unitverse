namespace Unitverse.Core.Frameworks
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public interface ITestFramework : IAssertionFramework
    {
        bool SupportsStaticTestClasses { get; }

        AttributeSyntax SingleThreadedApartmentAttribute { get; }

        string TestClassAttribute { get; }

        BaseMethodDeclarationSyntax CreateSetupMethod(string targetTypeName);

        MethodDeclarationSyntax CreateTestCaseMethod(string name, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object> testValues);

        MethodDeclarationSyntax CreateTestMethod(string name, bool isAsync, bool isStatic);
    }
}