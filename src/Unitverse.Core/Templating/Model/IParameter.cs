namespace Unitverse.Core.Templating.Model
{
    using System.Collections.Generic;

    public interface IParameter
    {
        IEnumerable<IAttribute> Attributes { get; }

        string Name { get; }

        IType? Type { get; }
    }
}
