namespace Unitverse.Core.Frameworks.Test
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;

    public class NUnit2TestFramework : NUnitTestFramework
    {
        public override AttributeSyntax SingleThreadedApartmentAttribute => Generate.Attribute("RequiresSTA");
    }
}