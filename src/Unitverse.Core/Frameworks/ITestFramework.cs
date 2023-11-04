namespace Unitverse.Core.Frameworks
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public interface ITestFramework : IAssertionFramework
    {
        bool SupportsStaticTestClasses { get; }

        AttributeSyntax? SingleThreadedApartmentAttribute { get; }

        IEnumerable<AttributeSyntax>? TestClassAttributes { get; }
    }
}