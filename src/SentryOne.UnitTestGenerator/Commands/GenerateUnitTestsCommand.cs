namespace SentryOne.UnitTestGenerator.Commands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using SentryOne.UnitTestGenerator.Core.Assets;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Helper;
    using Project = EnvDTE.Project;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Command handler.
    /// </summary>
    internal sealed class GenerateUnitTestsCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        private const int CommandId = 0x0101;

        private const int RegenerateCommandId = 0x104;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        private static readonly Guid CommandSet = new Guid("63d6b7b1-4f20-4519-9f56-09f9e220fd1b");

        private static GenerateUnitTestsCommand _instance;

        private static DTE2 _dte;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly IUnitTestGeneratorPackage _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateUnitTestsCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file).
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GenerateUnitTestsCommand(IUnitTestGeneratorPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var regenerateMenuCommandId = new CommandID(CommandSet, RegenerateCommandId);

            var menuItem = new OleMenuCommand(Execute, menuCommandId);
            var regenerateMenuItem = new OleMenuCommand(ExecuteRegenerate, regenerateMenuCommandId);
            menuItem.BeforeQueryStatus += (s, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var itemVisible = IsAvailable;
                menuItem.Visible = itemVisible;
                regenerateMenuItem.Visible = Keyboard.IsKeyDown(Key.LeftShift) && itemVisible;
            };

            commandService.AddCommand(menuItem);
            commandService.AddCommand(regenerateMenuItem);
        }

        private bool IsAvailable
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return SolutionUtilities.GetSelectedFiles(_dte, true, _package.Options.GenerationOptions).Any(ProjectItemModel.IsSupported);
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task InitializeAsync(IUnitTestGeneratorPackage package)
        {
            _dte = (DTE2)await package.GetServiceAsync(typeof(DTE)).ConfigureAwait(true);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)).ConfigureAwait(true) as OleMenuCommandService;
            _instance = new GenerateUnitTestsCommand(package, commandService);
        }

#pragma warning disable VSTHRD010

        private void Execute(object sender, EventArgs e)
        {
            Execute(false);
        }

        private void ExecuteRegenerate(object sender, EventArgs e)
        {
            Execute(true);
        }

#pragma warning restore VSTHRD010

        private void Execute(bool withRegeneration)
        {
            var generationItems = new List<GenerationItem>();
            var projectDictionary = new Dictionary<Project, Tuple<HashSet<TargetAsset>, HashSet<IReferencedAssembly>>>();

            var messageLogger = new AggregateLogger();
            messageLogger.Initialize();

            Attempt.Action(
                () =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
#pragma warning disable VSTHRD010

                if (!IsAvailable)
                {
                    throw new InvalidOperationException("Cannot generate unit tests for this item because no supported files were found");
                }

                var sources = SolutionUtilities.GetSelectedFiles(_dte, true, _package.Options.GenerationOptions).Where(ProjectItemModel.IsSupported).ToList();

                var targetProjects = new Dictionary<Project, Project>();

                foreach (var source in sources)
                {
                    if (targetProjects.ContainsKey(source.Project))
                    {
                        continue;
                    }

                    if (source.TargetProject == null && _package.Options.GenerationOptions.CreateProjectAutomatically)
                    {
                        var testProject = SolutionUtilities.CreateTestProjectInCurrentSolution(_dte, source.Project, _package.Options.GenerationOptions);
                        ReferencesHelper.AddNugetPackagesToProject(testProject, StandardReferenceHelper.GetReferencedNugetPackages(_package.Options), messageLogger.LogMessage, _package);
                    }

                    var targetProject = source.TargetProject;
                    targetProjects[source.Project] = targetProject ?? throw new InvalidOperationException("Cannot create tests for '" + Path.GetFileName(source.FilePath) + "' because there is no project '" + source.TargetProjectName + "'");
                    projectDictionary[targetProject] = Tuple.Create(new HashSet<TargetAsset>(), new HashSet<IReferencedAssembly>());
                }

                foreach (var source in sources)
                {
                    var projectItem = source.Item;

                    if (!withRegeneration && TargetFinder.FindExistingTargetItem(source, _package.Options.GenerationOptions, out _) == FindTargetStatus.Found)
                    {
                        if (sources.Count == 1)
                        {
                            throw new InvalidOperationException("Cannot create tests for '" + Path.GetFileName(source.FilePath) + "' because tests already exist. If you want to re-generate tests for this item, hold down the left Shift key and right-click the item.");
                        }

                        continue;
                    }

                    var nameParts = VsProjectHelper.GetNameParts(projectItem);

                    var targetProject = targetProjects[source.Project];

                    var targetProjectItems = TargetFinder.FindTargetFolder(targetProject, nameParts, true, out var targetPath);

                    if (targetProjectItems == null)
                    {
                        // we asked to create targetProjectItems - so if it's null we effectively had a problem getting to the target project
                        throw new InvalidOperationException("Cannot create tests for '" + Path.GetFileName(source.FilePath) + "' because there is no project '" + source.TargetProjectName + "'");
                    }

                    var sourceNameSpaceRoot = VsProjectHelper.GetProjectRootNamespace(source.Project);
                    var targetNameSpaceRoot = VsProjectHelper.GetProjectRootNamespace(source.TargetProject);

                    generationItems.Add(new GenerationItem(source, null, targetProjectItems, targetPath, projectDictionary[targetProject].Item1, projectDictionary[targetProject].Item2, NamespaceTransform.Create(sourceNameSpaceRoot, targetNameSpaceRoot), _package.Options.GenerationOptions));
                }
#pragma warning restore VSTHRD010
            }, _package);

            if (generationItems.Any())
            {
                _package.JoinableTaskFactory.RunAsync(() => Attempt.ActionAsync(() => CodeGenerator.GenerateCodeAsync(generationItems, withRegeneration, _package, projectDictionary, messageLogger), _package));
            }
        }
    }
}