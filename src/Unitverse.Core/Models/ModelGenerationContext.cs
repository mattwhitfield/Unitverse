namespace Unitverse.Core.Models
{
    using System;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public class ModelGenerationContext
    {
        public ModelGenerationContext(ClassModel model, IFrameworkSet frameworkSet, bool withRegeneration, NamingContext baseNamingContext, IMessageLogger messageLogger)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            FrameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
            WithRegeneration = withRegeneration;
            PartialGenerationAllowed = frameworkSet.Options.GenerationOptions.PartialGenerationAllowed;
            BaseNamingContext = baseNamingContext ?? throw new ArgumentNullException(nameof(baseNamingContext));
            MessageLogger = messageLogger;
            FrameworkSet.Context.CurrentModel = Model;
        }

        public ClassModel Model { get; }

        public IFrameworkSet FrameworkSet { get; }

        public bool WithRegeneration { get; }

        public bool PartialGenerationAllowed { get; }

        public NamingContext BaseNamingContext { get; }

        public IMessageLogger MessageLogger { get; }

        public int MethodsEmitted { get; set; }
    }
}
