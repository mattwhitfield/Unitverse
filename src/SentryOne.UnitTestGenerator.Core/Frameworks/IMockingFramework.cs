namespace SentryOne.UnitTestGenerator.Core.Frameworks
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    public interface IMockingFramework
    {
        IEnumerable<UsingDirectiveSyntax> GetUsings();

        TypeSyntax GetFieldType(TypeSyntax type);

        ExpressionSyntax GetFieldReference(ExpressionSyntax fieldReference);

        ExpressionSyntax GetFieldInitializer(TypeSyntax type);

        ExpressionSyntax GetThrowawayReference(TypeSyntax type);
    }
}