namespace Unitverse.Core.Templating
{
    using System.Collections.Generic;
    using System.Linq;
    using Unitverse.Core.Templating.Model.Implementation;

    public static class TemplateListExtensions
    {
        public static IList<ITemplate> ForProperties(this IList<ITemplate> templates)
        {
            return templates.Where(x => x.Target == PropertyFilterModel.Target).ToList();
        }

        public static IList<ITemplate> ForMethods(this IList<ITemplate> templates)
        {
            return templates.Where(x => x.Target == MethodFilterModel.Target).ToList();
        }

        public static IList<ITemplate> ForConstructors(this IList<ITemplate> templates)
        {
            return templates.Where(x => x.Target == ConstructorFilterModel.Target).ToList();
        }
    }
}
