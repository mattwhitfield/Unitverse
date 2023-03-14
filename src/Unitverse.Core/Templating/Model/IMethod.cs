namespace Unitverse.Core.Templating.Model
{
    using System.Collections.Generic;

    public interface IMethod : ITemplateTarget
    {
        bool IsAsync { get; }

        bool IsStatic { get; }

        bool IsVoid { get; }

        string Name { get; }

        IEnumerable<IParameter> Parameters { get; }

        IType? ReturnType { get; }
    }
}
