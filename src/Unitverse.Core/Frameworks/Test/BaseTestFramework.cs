namespace Unitverse.Core.Frameworks.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public abstract class BaseTestFramework
    {
        protected BaseTestFramework(IUnitTestGeneratorOptions options)
        {
            Options = options;
        }

        public IUnitTestGeneratorOptions Options { get; }

        protected MethodDeclarationSyntax AddXmlCommentsIfConfigured(MethodDeclarationSyntax method, string description, string parameterName = null, string parameterDescription = null)
        {
            if (Options.GenerationOptions.EmitXmlDocumentation)
            {
                IEnumerable<XmlElementSyntax> Elements()
                {
                    yield return XmlCommentHelper.Summary(XmlCommentHelper.TextLiteral(description));
                    if (!string.IsNullOrWhiteSpace(parameterName) && !string.IsNullOrWhiteSpace(parameterDescription))
                    {
                        yield return XmlCommentHelper.Param(parameterName, parameterDescription);
                    }

                    if (method.Modifiers.Any(x => x.Kind() == SyntaxKind.AsyncKeyword))
                    {
                        yield return XmlCommentHelper.Returns("A task that represents the running test");
                    }
                }

                var documentation = XmlCommentHelper.DocumentationComment(Elements());
                return method.WithXmlDocumentation(documentation);
            }

            return method;
        }
    }
}
