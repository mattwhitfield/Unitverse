namespace Unitverse.Core.Frameworks.Test
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public class NUnit2TestFramework : NUnitTestFramework
    {
        public NUnit2TestFramework(IUnitTestGeneratorOptions options)
            : base(options)
        {
        }

        public override AttributeSyntax SingleThreadedApartmentAttribute => Generate.Attribute("RequiresSTA");
    }
}