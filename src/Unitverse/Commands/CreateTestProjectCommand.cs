namespace Unitverse.Commands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Interop;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using NuGet.VisualStudio;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Helper;
    using Unitverse.Options;
    using Unitverse.Views;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Command handler.
    /// </summary>
    internal sealed class CreateTestProjectCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        private const int CommandId = 0x0106;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        private static readonly Guid CommandSet = new Guid("63d6b7b1-4f20-4519-9f56-09f9e220fd1b");

        private static DTE2 _dte;

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        private static CreateTestProjectCommand _instance;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly IUnitTestGeneratorPackage _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTestProjectCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file).
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CreateTestProjectCommand(IUnitTestGeneratorPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(Execute, menuCommandId);
            menuItem.BeforeQueryStatus += (s, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                menuItem.Visible = SolutionUtilities.GetSupportedProject(_dte) != null;
            };

            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task InitializeAsync(IUnitTestGeneratorPackage package)
        {
            _dte = (DTE2)await package.GetServiceAsync(typeof(DTE)).ConfigureAwait(true);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)).ConfigureAwait(true) as OleMenuCommandService;
            _instance = new CreateTestProjectCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            Attempt.Action(
                () =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();

                    var project = SolutionUtilities.GetSupportedProject(_dte);
                    if (project == null)
                    {
                        throw new InvalidOperationException("Cannot create test project because the selected project is not supported");
                    }

                    var logger = new AggregateLogger();
                    logger.Initialize();

                    var options = UnitTestGeneratorOptionsFactory.Create(_package.Workspace?.CurrentSolution?.FilePath, _package.Options);
                    var window = new NewProjectDialog(project, options);
                    var helper = new WindowInteropHelper(window);
#if VS2022
                    helper.Owner = project.DTE.MainWindow.HWnd;
#elif VS2019
                    helper.Owner = new System.IntPtr(project.DTE.MainWindow.HWnd);
#endif

                    var result = window.ShowDialog();
                    if (result.HasValue && result.Value)
                    {
                        var moniker = project.Properties.Item("TargetFrameworkMoniker")?.Value?.ToString();
                        string shortFrameworkName = "net6.0";
                        if (!string.IsNullOrWhiteSpace(moniker))
                        {
                            try
                            {
                                var frameworkName = _package.FrameworkParser.ParseFrameworkName(moniker);
                                shortFrameworkName = _package.FrameworkParser.GetShortFrameworkName(frameworkName);
                            }
                            catch (ArgumentException)
                            {
                            }
                        }

                        if (shortFrameworkName.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase))
                        {
                            shortFrameworkName = "netcoreapp3.1";
                        }

                        var manifest = window.Manifest;

                        // write out project file
                        var projectFileName = Path.Combine(manifest.FolderName, manifest.Name, manifest.Name + ".csproj");
                        var directory = Path.GetDirectoryName(projectFileName);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        File.WriteAllText(projectFileName, Unitverse.Properties.Strings.DefaultProjectContent.Replace("$$TARGETFRAMEWORK$$", shortFrameworkName), Encoding.UTF8);
                        
                        // add the project
                        var newProject = _dte.Solution.AddFromFile(projectFileName);

                        // wait for project load and add references
                        var packages = FrameworkPackageProvider.Get(manifest.GenerationOptions).ToList();
                        ReferencesHelper.AddNugetPackagesAndProjectReferences(_package, newProject, packages, new[] { project });
                    }
                }, _package);
        }
    }
}