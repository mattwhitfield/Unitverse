namespace SentryOne.UnitTestGenerator.Core.Frameworks
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    public interface IMockingFramework
    {
        IEnumerable<UsingDirectiveSyntax> GetUsings();

        ExpressionSyntax MockInterface(TypeSyntax interfaceName);

        IEnumerable<INugetPackageReference> ReferencedNugetPackages(IVersioningOptions options);
    }
}