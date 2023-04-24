namespace Unitverse.Core.Templating.Model
{
    using System.Collections.Generic;

    public interface IParameter : INameProvider
    {
        IEnumerable<IAttribute> Attributes { get; }

        IType? Type { get; }
    }
}
