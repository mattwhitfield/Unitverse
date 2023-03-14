namespace Unitverse.Core.Templating
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SequelFilter;
    using SequelFilter.Resolvers;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating.Model;

    public class Template : ITemplate
    {
        public Template(
            string content,
            string testMethodName,
            string target,
            IList<ExecutableExpression> includeExpressions,
            IList<ExecutableExpression> excludeExpressions,
            bool isExclusive,
            bool stopMatching,
            int priority,
            bool isAsync,
            bool isStatic,
            string description)
        {
            Content = content;
            TestMethodName = new NameResolver(testMethodName);
            Target = target;
            IncludeExpressions = includeExpressions;
            ExcludeExpressions = excludeExpressions;
            IsExclusive = isExclusive;
            StopMatching = stopMatching;
            Priority = priority;
            IsAsync = isAsync;
            IsStatic = isStatic;
            Description = description;
        }

        public string Content { get; }

        public NameResolver TestMethodName { get; }

        public string Target { get; }

        public IList<ExecutableExpression> IncludeExpressions { get; }

        public IList<ExecutableExpression> ExcludeExpressions { get; }

        public bool IsExclusive { get; }

        public bool StopMatching { get; }

        public int Priority { get; }

        public bool IsAsync { get; }

        public bool IsStatic { get; }

        public string Description { get; }

        public bool AppliesTo(ITemplateTarget model, IClass owningType)
        {
            if (!string.Equals(Target, model.TemplateType, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var fieldReferenceResolver = new MultiObjectResolver(new Dictionary<string, object> { { "model", model }, { "owningType", owningType } });

            var included = !IncludeExpressions.Any() || IncludeExpressions.Any(expr => expr(fieldReferenceResolver));
            if (!included)
            {
                return false;
            }

            var excluded = ExcludeExpressions.Any(expr => expr(fieldReferenceResolver));
            if (excluded)
            {
                return false;
            }

            return true;
        }

        public MethodDeclarationSyntax Create(IFrameworkSet frameworkSet, ITemplateTarget model, IClass owningType, NamingContext namingContext)
        {
            var fieldReferenceResolver = new MultiObjectResolver(new Dictionary<string, object> { { "model", model }, { "owningType", owningType } });
            var content = new ObjectPathResolver(Content).Resolve(fieldReferenceResolver);

            var methodHandler = frameworkSet.CreateTestMethod(TestMethodName, namingContext, IsAsync, IsStatic, Description);
            var body = (BlockSyntax)SyntaxFactory.ParseStatement("{" + content + "}");

            return (MethodDeclarationSyntax)methodHandler.Method.WithBody(body);
        }
    }
}
