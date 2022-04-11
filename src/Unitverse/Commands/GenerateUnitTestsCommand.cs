namespace Unitverse.Commands
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
    using Unitverse.Helper;
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
                return SolutionUtilities.GetSupportedFiles(_dte, true).Any();
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

            var messageLogger = new AggregateLogger();
            messageLogger.Initialize();

            bool isSingleCreation = false;

            Attempt.Action(
                () =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
#pragma warning disable VSTHRD010

                var sources = SolutionUtilities.GetSupportedFiles(_dte, true).ToList();
                if (sources.Count == 0)
                {
                    throw new InvalidOperationException("Cannot generate unit tests for this item because no supported files were found");
                }

                var baseOptions = _package.Options;
                isSingleCreation = sources.Count == 1;

                var sourceProjects = sources.Select(x => x.Project).Distinct().ToList();
                if (sourceProjects.Count > 1)
                {
                    throw new InvalidOperationException("Cannot generate unit tests for multiple projects at the same time, please select a single project");
                }

                var mapping = ProjectMappingFactory.CreateMappingFor(sourceProjects.Single(), baseOptions, true, false, messageLogger);
                if (mapping == null)
                {
                    return;
                }

                if (mapping.TargetProject == null && !mapping.Options.GenerationOptions.AllowGenerationWithoutTargetProject)
                {
                    throw new InvalidOperationException("Cannot create tests for '" + Path.GetFileName(sources.First().FilePath) + "' because there is no project '" + mapping.TargetProjectName + "'");
                }

                foreach (var source in sources)
                {
                    var projectItem = source.Item;

                    if (!withRegeneration && !mapping.Options.GenerationOptions.PartialGenerationAllowed && TargetFinder.FindExistingTargetItem(null, source, mapping, _package, messageLogger, out _) == FindTargetStatus.Found)
                    {
                        if (isSingleCreation)
                        {
                            throw new InvalidOperationException("Cannot create tests for '" + Path.GetFileName(source.FilePath) + "' because tests already exist. If you want to re-generate tests, hold down the left Shift key while opening the context menu and select the 'Regenerate tests' option. If you want to add new tests for any new code, enable the 'Partial Generation' option.");
                        }
                        else
                        {
                            messageLogger.LogMessage("Cannot create tests for '" + Path.GetFileName(source.FilePath) + "' because tests already exist.");
                        }
                        continue;
                    }

                    generationItems.Add(new GenerationItem(source, mapping));
                }

                if (generationItems.Any())
                {
                    _ = _package.JoinableTaskFactory.RunAsync(() => Attempt.ActionAsync(() => CodeGenerator.GenerateCodeAsync(generationItems, withRegeneration, _package, messageLogger), _package));
                }
                else
                {
                    var message = "No tests could be created because tests already exist for all selected source items. If you want to re-generate tests, hold down the left Shift key while opening the context menu and select the 'Regenerate tests' option. If you want to add new tests for any new code, enable the 'Partial Generation' option.";

                    if (!isSingleCreation)
                    {
                        VsMessageBox.Show(message, true, _package);
                    }

                    messageLogger.LogMessage(message);
                }
#pragma warning restore VSTHRD010
            }, _package);
        }
    }
}