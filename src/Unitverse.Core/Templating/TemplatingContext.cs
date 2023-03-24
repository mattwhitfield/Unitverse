namespace Unitverse.Core.Templating
{
    using System.Collections.Generic;
    using Unitverse.Core.Models;
    using Unitverse.Core.Templating.Model;
    using Unitverse.Core.Templating.Model.Implementation;

    public class TemplatingContext : ITemplatingContext
    {
        public TemplatingContext(ModelGenerationContext modelGenerationContext, IList<ITemplate> templates)
        {
            ModelGenerationContext = modelGenerationContext;
            Templates = templates;
            ClassModel = new ClassFilterModel(modelGenerationContext.Model);
        }

        public ModelGenerationContext ModelGenerationContext { get; }

        public IList<ITemplate> Templates { get; }

        public IClass ClassModel { get; }

        public SpecificTemplatingContext ForConstructors()
        {
            return new SpecificTemplatingContext(ModelGenerationContext, Templates.ForConstructors(), ClassModel, ClassModel.Constructors);
        }

        public SpecificTemplatingContext ForMethods()
        {
            return new SpecificTemplatingContext(ModelGenerationContext, Templates.ForMethods(), ClassModel, ClassModel.Methods);
        }

        public SpecificTemplatingContext ForProperties()
        {
            return new SpecificTemplatingContext(ModelGenerationContext, Templates.ForProperties(), ClassModel, ClassModel.Properties);
        }
    }
}
