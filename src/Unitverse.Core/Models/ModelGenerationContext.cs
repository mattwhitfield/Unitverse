namespace Unitverse.Core.Models
{
    using System;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Options;

    public class ModelGenerationContext
    {
        public ModelGenerationContext(ClassModel model, IFrameworkSet frameworkSet, bool withRegeneration, bool partialGenerationAllowed, NamingContext baseNamingContext)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            FrameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
            WithRegeneration = withRegeneration;
            PartialGenerationAllowed = partialGenerationAllowed;
            BaseNamingContext = baseNamingContext;
        }

        public ClassModel Model { get; }

        public IFrameworkSet FrameworkSet { get; }

        public bool WithRegeneration { get; }

        public bool PartialGenerationAllowed { get; }

        public NamingContext BaseNamingContext { get; }

        public int MethodsEmitted { get; set; }
    }
}
