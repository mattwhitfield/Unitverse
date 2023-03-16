namespace Unitverse.Core.Templating.Model
{
    using System.Collections.Generic;

    public interface IConstructor : ITemplateTarget
    {
        IEnumerable<IParameter> Parameters { get; }
    }
}
