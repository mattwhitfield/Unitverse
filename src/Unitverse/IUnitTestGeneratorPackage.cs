namespace Unitverse
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Threading;
    using Unitverse.Core.Options;

    public interface IUnitTestGeneratorPackage : IServiceProvider
    {
        JoinableTaskFactory JoinableTaskFactory { get; }

        IUnitTestGeneratorOptions Options { get; }

        VisualStudioWorkspace Workspace { get; }

        Task<object> GetServiceAsync(Type serviceType);
    }
}