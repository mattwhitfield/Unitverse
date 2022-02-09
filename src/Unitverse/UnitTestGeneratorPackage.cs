namespace Unitverse
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Commands;
    using Unitverse.Core.Options;
    using Unitverse.Options;
    using Task = System.Threading.Tasks.Task;

    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(Constants.ExtensionGuid)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(GenerationOptions), "Unitverse", "Generation Options", 0, 0, true)]
    [ProvideOptionPage(typeof(NamingOptions), "Unitverse", "Naming Options", 0, 0, true)]
    [ProvideOptionPage(typeof(ExportOptions), "Unitverse", "Options Export", 0, 0, true)]
    [ProvideOptionPage(typeof(StatisticsOptions), "Unitverse", "Statistics", 0, 0, true)]
    public sealed class UnitTestGeneratorPackage : AsyncPackage, IUnitTestGeneratorPackage
    {
        public IGenerationOptions GenerationOptions => (GenerationOptions)GetDialogPage(typeof(GenerationOptions));

        public INamingOptions NamingOptions => (NamingOptions)GetDialogPage(typeof(NamingOptions));

        public IUnitTestGeneratorOptions Options
        {
            get
            {
                var solutionFilePath = Workspace?.CurrentSolution?.FilePath;
                var statisticsOptions = (StatisticsOptions)GetDialogPage(typeof(StatisticsOptions));
                return UnitTestGeneratorOptionsFactory.Create(solutionFilePath, GenerationOptions, NamingOptions, statisticsOptions.Enabled);
            }
        }

        public VisualStudioWorkspace Workspace { get; private set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress).ConfigureAwait(true);

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

#pragma warning disable VSSDK006 // null check is present
            var componentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel)).ConfigureAwait(true);
            if (componentModel == null)
            {
                throw new InvalidOperationException();
            }

            Workspace = componentModel.GetService<VisualStudioWorkspace>();

            await GoToUnitTestsCommand.InitializeAsync(this).ConfigureAwait(true);
            await GenerateUnitTestsCommand.InitializeAsync(this).ConfigureAwait(true);
            await GenerateTestForSymbolCommand.InitializeAsync(this).ConfigureAwait(true);
            await GoToUnitTestsForSymbolCommand.InitializeAsync(this).ConfigureAwait(true);
        }
    }
}