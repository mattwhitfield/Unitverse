namespace Unitverse
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Threading;
    using NuGet.VisualStudio;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Core.Options.Editing;
    using Unitverse.Helper;

    public interface IUnitTestGeneratorPackage : IServiceProvider, IConfigurationWriter
    {
        JoinableTaskFactory JoinableTaskFactory { get; }

        IUnitTestGeneratorOptions Options { get; }

        IGenerationOptions GenerationOptions { get; }

        INamingOptions NamingOptions { get; }

        IStrategyOptions StrategyOptions { get; }

        IDictionary<string, string> ManualProjectMappings { get; }

        VisualStudioWorkspace Workspace { get; }

        IVsPackageInstaller PackageInstaller { get; }

        IVsFrameworkParser FrameworkParser { get; }

        Task<object> GetServiceAsync(Type serviceType);

        void ShowFilterDebugger(ProjectItemModel source, Func<Task<TargetSymbol>> targetSymbolFactory);
    }
}