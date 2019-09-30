namespace SentryOne.UnitTestGenerator
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Threading;
    using NuGet.VisualStudio;
    using SentryOne.UnitTestGenerator.Core.Options;

    public interface IUnitTestGeneratorPackage : IServiceProvider
    {
        JoinableTaskFactory JoinableTaskFactory { get; }

        IUnitTestGeneratorOptions Options { get; }

        IVsPackageInstaller PackageInstaller { get; }

        IVsPackageInstallerServices PackageInstallerServices { get; }

        VisualStudioWorkspace Workspace { get; }

        Task<object> GetServiceAsync(Type serviceType);
    }
}