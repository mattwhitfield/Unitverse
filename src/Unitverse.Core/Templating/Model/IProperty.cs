namespace Unitverse.Core.Templating.Model
{
    using System.Collections.Generic;

    public interface IProperty
    {
        bool HasGet { get; }

        bool HasInit { get; }

        bool HasSet { get; }

        bool IsStatic { get; }

        string Name { get; }

        IType? Type { get; }

        IEnumerable<IAttribute> Attributes { get; }
    }
}
