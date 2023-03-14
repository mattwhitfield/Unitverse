namespace Unitverse.Core.Templating.Model
{
    using System.Collections.Generic;

    public interface ITemplateTarget
    {
        IEnumerable<IAttribute> Attributes { get; }

        string TemplateType { get; }
    }
}
