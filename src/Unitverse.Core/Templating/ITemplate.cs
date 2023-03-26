namespace Unitverse.Core.Templating
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SequelFilter;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating.Model;

    public interface ITemplate
    {
        string Content { get; }

        NameResolver TestMethodName { get; }

        string Target { get; }

        IList<ExecutableExpression> IncludeExpressions { get; }

        IList<ExecutableExpression> ExcludeExpressions { get; }

        bool IsExclusive { get; }

        bool StopMatching { get; }

        int Priority { get; }

        bool IsAsync { get; }

        bool IsStatic { get; }

        string Description { get; }

        bool AppliesTo(ITemplateTarget model, IOwningType owningType);

        MethodDeclarationSyntax Create(IFrameworkSet frameworkSet, ITemplateTarget model, IOwningType owningType, NamingContext namingContext);
    }
}
