namespace Unitverse.Core.Templating
{
    using System.Collections.Generic;
    using System.Linq;
    using Unitverse.Core.Models;
    using Unitverse.Core.Templating.Model;

    public class SpecificTemplatingContext : ITemplatingContext
    {
        public SpecificTemplatingContext(ModelGenerationContext modelGenerationContext, IList<ITemplate> templates, IClass classModel, IEnumerable<ITemplateTarget> targets)
        {
            ModelGenerationContext = modelGenerationContext;
            Templates = templates;
            ClassModel = classModel;
            Targets = targets.ToList();
        }

        public ModelGenerationContext ModelGenerationContext { get; }

        public IList<ITemplate> Templates { get; }

        public IClass ClassModel { get; }

        public IList<ITemplateTarget> Targets { get; }
    }
}
