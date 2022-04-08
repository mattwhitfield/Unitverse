namespace Unitverse.Commands
{
    using System;
    using System.ComponentModel.Design;
    using System.Linq;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Helper;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Command handler.
    /// </summary>
    internal sealed class GoToUnitTestsCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        private const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        private static readonly Guid CommandSet = new Guid("63d6b7b1-4f20-4519-9f56-09f9e220fd1b");

        private static DTE2 _dte;

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        private static GoToUnitTestsCommand _instance;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly IUnitTestGeneratorPackage _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoToUnitTestsCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file).
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GoToUnitTestsCommand(IUnitTestGeneratorPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(Execute, menuCommandId);
            menuItem.BeforeQueryStatus += (s, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                menuItem.Visible = SolutionUtilities.GetSupportedFiles(_dte, false).Any();
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
            _instance = new GoToUnitTestsCommand(package, commandService);
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

                    var source = SolutionUtilities.GetSupportedFiles(_dte, false).FirstOrDefault();
                    if (source == null)
                    {
                        throw new InvalidOperationException("Cannot go to tests for this item because no supported files were found");
                    }

                    var logger = new AggregateLogger();
                    logger.Initialize();

                    GoToTestsHelper.FindTestsFor(null, source, _package, logger);
                }, _package);
        }
    }
}