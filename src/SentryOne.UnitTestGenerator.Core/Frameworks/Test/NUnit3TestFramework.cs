namespace SentryOne.UnitTestGenerator.Core.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class NUnit3TestFramework : NUnitTestFramework
    {
        private bool _requiresSystemThreading;

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
                yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.NUnit3TestFramework_GetUsings_System_Threading));
            }
        }

        public override IEnumerable<INugetPackageReference> ReferencedNugetPackages(IVersioningOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            yield return new NugetPackageReference("NUnit", options.NUnit3NugetPackageVersion);
        }
    }
}