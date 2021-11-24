namespace SentryOne.UnitTestGenerator.Core.Frameworks.Test
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Helpers;

    public class NUnit2TestFramework : NUnitTestFramework
    {
        public override AttributeSyntax SingleThreadedApartmentAttribute => Generate.Attribute("RequiresSTA");
    }
}