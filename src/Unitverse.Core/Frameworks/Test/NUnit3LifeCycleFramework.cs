namespace Unitverse.Core.Frameworks.Test
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public class NUnit3LifeCycleFramework : NUnit3TestFramework
    {
        public NUnit3LifeCycleFramework(IUnitTestGeneratorOptions options)
            : base(options)
        {
        }

        public override IEnumerable<AttributeSyntax> TestClassAttributes => new[]
        {
            Generate.Attribute("TestFixture"),
            Generate.Attribute("FixtureLifeCycle", Generate.MemberAccess("LifeCycle", "InstancePerTestCase")),
        };

        protected override BaseMethodDeclarationSyntax CreateSetupMethodSyntax(string targetTypeName)
        {
            return SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier(targetTypeName)).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        }
    }
}