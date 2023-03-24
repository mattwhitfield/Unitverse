namespace Unitverse
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Media.Animation;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using NuGet.VisualStudio;
    using Unitverse.Commands;
    using Unitverse.Core;
    using Unitverse.Core.Options;
    using Unitverse.Editor;
    using Unitverse.Options;
    using Task = System.Threading.Tasks.Task;

    [ProvideAutoLoad(UIContextGuids.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(Constants.ExtensionGuid)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(GenerationOptions), "Unitverse", "Generation Options", 0, 0, true)]
    [ProvideOptionPage(typeof(NamingOptions), "Unitverse", "Naming Options", 0, 0, true)]
    [ProvideOptionPage(typeof(StrategyOptions), "Unitverse", "Strategy Options", 0, 0, true)]
    [ProvideOptionPage(typeof(ExportOptions), "Unitverse", "Options Export", 0, 0, true)]
    [ProvideOptionPage(typeof(StatisticsOptions), "Unitverse", "Statistics", 0, 0, true)]
    [ProvideOptionPage(typeof(ProjectMappingOptions), "Unitverse", "Project Mappings", 0, 0, true)]
    [ProvideEditorExtension(typeof(ConfigEditorFactory), CoreConstants.ConfigFileName, 50, NameResourceID = 110)]
    [ProvideEditorLogicalView(typeof(ConfigEditorFactory), VSConstants.LOGVIEWID.TextView_string, IsTrusted = true)]
    public sealed class UnitTestGeneratorPackage : AsyncPackage, IUnitTestGeneratorPackage
    {
        public IGenerationOptions GenerationOptions => (GenerationOptions)GetDialogPage(typeof(GenerationOptions));

        public INamingOptions NamingOptions => (NamingOptions)GetDialogPage(typeof(NamingOptions));

        public IStrategyOptions StrategyOptions => (StrategyOptions)GetDialogPage(typeof(StrategyOptions));

        public IVsFrameworkParser FrameworkParser { get; private set; }

        public IVsPackageInstaller PackageInstaller { get; private set; }

        public IUnitTestGeneratorOptions Options
        {
            get
            {
                var statisticsOptions = (StatisticsOptions)GetDialogPage(typeof(StatisticsOptions));
                return new UnitTestGeneratorOptions(GenerationOptions, NamingOptions, StrategyOptions, statisticsOptions.Enabled, null, null, Workspace?.CurrentSolution?.FilePath ?? string.Empty, string.Empty);
            }
        }

        public VisualStudioWorkspace Workspace { get; private set; }

        public IDictionary<string, string> ManualProjectMappings
        {
            get
            {
                var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                var mappings = ((ProjectMappingOptions)GetDialogPage(typeof(ProjectMappingOptions))).ProjectMappings;
                if (mappings == null)
                {
                    return result;
                }

                foreach (var mapping in mappings )
                {
                    result[mapping.SourceProject.Trim()] = mapping.TargetProject.Trim();
                }

                return result;
            }
        }

        private void WriteSettings<T>(Dictionary<string, string> settings)
            where T : DialogPage
        {
            var options = (T)GetDialogPage(typeof(T));
            if (settings.ApplyTo(options))
            {
                options.SaveSettingsToStorage();
            }
        }

        public void WriteSettings(Dictionary<string, string> settings, string sourceProjectName, string targetProjectName)
        {
            WriteSettings<GenerationOptions>(settings);
            WriteSettings<NamingOptions>(settings);
            WriteSettings<StrategyOptions>(settings);

            if (!string.IsNullOrWhiteSpace(sourceProjectName) && !string.IsNullOrWhiteSpace(targetProjectName))
            {
                var mappingOptions = (ProjectMappingOptions)GetDialogPage(typeof(ProjectMappingOptions));
                if (mappingOptions.ProjectMappings == null)
                {
                    mappingOptions.ProjectMappings = new List<ProjectMappingOption>();
                }

                var existing = mappingOptions.ProjectMappings.FirstOrDefault(x => string.Equals(x.SourceProject, sourceProjectName));
                if (existing != null)
                {
                    existing.TargetProject = targetProjectName;
                }
                else
                {
                    mappingOptions.ProjectMappings.Add(new ProjectMappingOption { SourceProject = sourceProjectName, TargetProject = targetProjectName });
                }
                mappingOptions.SaveSettingsToStorage();
            }
        }

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

            PackageInstaller = componentModel.GetService<IVsPackageInstaller>();
            FrameworkParser = componentModel.GetService<IVsFrameworkParser>();
            Workspace = componentModel.GetService<VisualStudioWorkspace>();

            RegisterEditorFactory(new ConfigEditorFactory(this));

            await GoToUnitTestsCommand.InitializeAsync(this).ConfigureAwait(true);
            await GenerateUnitTestsCommand.InitializeAsync(this).ConfigureAwait(true);
            await GenerateTestForSymbolCommand.InitializeAsync(this).ConfigureAwait(true);
            await GoToUnitTestsForSymbolCommand.InitializeAsync(this).ConfigureAwait(true);
            await CreateTestProjectCommand.InitializeAsync(this).ConfigureAwait(true);
        }
    }
}