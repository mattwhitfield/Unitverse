namespace Unitverse.Core.Templating.Model
{
    using System.Collections.Generic;

    public interface IConstructor
    {
        IEnumerable<IAttribute> Attributes { get; }

        IEnumerable<IParameter> Parameters { get; }
    }
}
