namespace Unitverse.Core.Strategies
{
    using System;
    using System.Collections.Generic;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public interface IGenerationStrategy<in T>
    {
        bool IsExclusive { get; }

        int Priority { get; }

        Func<IStrategyOptions, bool> IsEnabled { get; }

        bool CanHandle(T member, ClassModel model);

        IEnumerable<SectionedMethodHandler> Create(T member, ClassModel model, NamingContext namingContext);
    }
}