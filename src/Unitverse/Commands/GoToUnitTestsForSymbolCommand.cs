namespace Unitverse.Commands
{
    using System;
    using System.ComponentModel.Design;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Unitverse.Helper;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Command handler.
    /// </summary>
    internal sealed class GoToUnitTestsForSymbolCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        private const int CommandId = 0x0105;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        private static readonly Guid CommandSet = new Guid("63d6b7b1-4f20-4519-9f56-09f9e220fd1b");

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        private static GoToUnitTestsForSymbolCommand _instance;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly IUnitTestGeneratorPackage _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoToUnitTestsForSymbolCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file).
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GoToUnitTestsForSymbolCommand(IUnitTestGeneratorPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);

            var menuItem = new OleMenuCommand(Execute, menuCommandId);

            menuItem.BeforeQueryStatus += (s, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var textView = GetTextView();

                var methodTask = package.JoinableTaskFactory.RunAsync(async () => await GenerateTestForSymbolCommand.GetTargetSymbolAsync(textView).ConfigureAwait(true));
                var tuple = methodTask.Join();
                var symbol = tuple?.Item3;
                menuItem.Visible = symbol != null;
            };

            commandService.AddCommand(menuItem);
        }

        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task InitializeAsync(IUnitTestGeneratorPackage package)
        {
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)).ConfigureAwait(true) as OleMenuCommandService;
            _instance = new GoToUnitTestsForSymbolCommand(package, commandService);
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

                var textView = GetTextView();
                if (textView == null)
                {
                    throw new InvalidOperationException("Could not find the text view");
                }

                var caretPosition = textView.Caret.Position.BufferPosition;
                var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

                var item = VsProjectHelper.GetProjectItem(document.FilePath);

                var options = _package.Options;
                var source = new ProjectItemModel(item, options.GenerationOptions);

                var status = TargetFinder.FindExistingTargetItem(source, options.GenerationOptions, out var targetItem);
                switch (status)
                {
                    case FindTargetStatus.FileNotFound:
                    case FindTargetStatus.FolderNotFound:
                        throw new InvalidOperationException("No unit tests were found for the selected file.");
                    case FindTargetStatus.ProjectNotFound:
                        throw new InvalidOperationException("Cannot go to tests for this item because there is no project '" + source.TargetProjectName + "'");
                }

                VsProjectHelper.ActivateItem(targetItem);
            }, _package);
        }

        private IWpfTextView GetTextView()
        {
            var textManager = (IVsTextManager)ServiceProvider.GetService(typeof(SVsTextManager));
            if (textManager != null)
            {
                textManager.GetActiveView(1, null, out var textView);

                var componentModel = (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));
                var adapterService = componentModel?.GetService<Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService>();

                return adapterService?.GetWpfTextView(textView);
            }

            return null;
        }
    }
}