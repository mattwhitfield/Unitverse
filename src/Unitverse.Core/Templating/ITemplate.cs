namespace Unitverse.Core.Templating
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating.Model;

    public interface ITemplate
    {
        bool AppliesTo(ITemplateTarget model, IClass owningType);

        MethodDeclarationSyntax Create(IFrameworkSet frameworkSet, ITemplateTarget model, IClass owningType, NamingContext namingContext);
    }
}
