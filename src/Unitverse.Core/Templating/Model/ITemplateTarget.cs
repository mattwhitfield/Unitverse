namespace Unitverse.Core.Templating.Model
{
    using System.Collections.Generic;
    using Unitverse.Core.Options;

    public interface ITemplateTarget
    {
        IEnumerable<IAttribute> Attributes { get; }

        string TemplateType { get; }

        bool ShouldGenerate { get; }

        NamingContext CreateNamingContext(ITemplatingContext templatingContext);
    }
}
