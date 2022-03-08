namespace Unitverse.Core.Frameworks.Test
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public class NUnit3TestFramework : NUnitTestFramework
    {
        private bool _requiresSystemThreading;

        public NUnit3TestFramework(IUnitTestGeneratorOptions options)
            : base(options)
        {
        }

        public override AttributeSyntax SingleThreadedApartmentAttribute
        {
            get
            {
                _requiresSystemThreading = true;
                return Generate.Attribute("Apartment", SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("ApartmentState"), SyntaxFactory.IdentifierName("STA")));
            }
        }

        public override IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            foreach (var usingDirectiveSyntax in base.GetUsings())
            {
                yield return usingDirectiveSyntax;
            }

            if (_requiresSystemThreading)
            {
                yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Threading"));
            }
        }
    }
}