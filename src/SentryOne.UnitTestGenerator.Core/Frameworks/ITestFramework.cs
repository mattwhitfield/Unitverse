namespace SentryOne.UnitTestGenerator.Core.Frameworks
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    public interface ITestFramework
    {
        bool SupportsStaticTestClasses { get; }

        bool AssertThrowsAsyncIsAwaitable { get; }

        AttributeSyntax SingleThreadedApartmentAttribute { get; }

        string TestClassAttribute { get; }

        StatementSyntax AssertEqual(ExpressionSyntax actual, ExpressionSyntax expected);

        StatementSyntax AssertFail(string message);

        StatementSyntax AssertGreaterThan(ExpressionSyntax actual, ExpressionSyntax expected);

        StatementSyntax AssertIsInstanceOf(ExpressionSyntax value, TypeSyntax type);

        StatementSyntax AssertLessThan(ExpressionSyntax actual, ExpressionSyntax expected);

        StatementSyntax AssertNotNull(ExpressionSyntax value);

        StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall);

        StatementSyntax AssertThrowsAsync(TypeSyntax exceptionType, ExpressionSyntax methodCall);

        BaseMethodDeclarationSyntax CreateSetupMethod(string targetTypeName);

        MethodDeclarationSyntax CreateTestCaseMethod(string name, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object> testValues);

        MethodDeclarationSyntax CreateTestMethod(string name, bool isAsync, bool isStatic);

        IEnumerable<UsingDirectiveSyntax> GetUsings();
    }
}