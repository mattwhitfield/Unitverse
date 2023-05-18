namespace Unitverse.Core.Templating
{
    using System.Collections.Generic;
    using Unitverse.Core.Models;
    using Unitverse.Core.Templating.Model;

    public interface ITemplatingContext
    {
        IOwningType ClassModel { get; }

        ModelGenerationContext ModelGenerationContext { get; }

        IList<ITemplate> Templates { get; }
    }
}