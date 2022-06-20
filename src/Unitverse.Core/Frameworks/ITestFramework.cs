namespace Unitverse.Core.Frameworks
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public interface ITestFramework : IAssertionFramework
    {
        bool SupportsStaticTestClasses { get; }

        AttributeSyntax? SingleThreadedApartmentAttribute { get; }

        string TestClassAttribute { get; }
    }
}