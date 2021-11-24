namespace Unitverse.Commands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Strategies.InterfaceGeneration;
    using Unitverse.Helper;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Command handler.
    /// </summary>
    internal sealed class GenerateTestForSymbolCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        private const int CommandId = 258;

        private const int RegenerateCommandId = 259;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        private static readonly Guid CommandSet = new Guid("63d6b7b1-4f20-4519-9f56-09f9e220fd1b");

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        private static GenerateTestForSymbolCommand _instance;

        private static DTE2 _dte;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly IUnitTestGeneratorPackage _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateTestForSymbolCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file).
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GenerateTestForSymbolCommand(IUnitTestGeneratorPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var regenerationMenuCommandId = new CommandID(CommandSet, RegenerateCommandId);

            var menuItem = new OleMenuCommand(Execute, menuCommandId);
            var regenerationMenuItem = new OleMenuCommand(ExecuteRegeneration, regenerationMenuCommandId);

            menuItem.BeforeQueryStatus += (s, e) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var textView = GetTextView();

                var methodTask = package.JoinableTaskFactory.RunAsync(async () => await GetTargetSymbolAsync(textView).ConfigureAwait(true));
                var tuple = methodTask.Join();
                var symbol = tuple?.Item2;
                var baseType = tuple?.Item3;
                menuItem.Visible = false;
                regenerationMenuItem.Visible = false;
                if (symbol != null)
                {
                    menuItem.Visible = true;
                    regenerationMenuItem.Visible = Keyboard.IsKeyDown(Key.LeftShift);
                    if (symbol.Kind == SymbolKind.NamedType)
                    {
                        menuItem.Text = "Generate tests for type '" + symbol.Name + "'...";
                        regenerationMenuItem.Text = "Regenerate tests for type '" + symbol.Name + "'...";
                    }
                    else if (string.Equals(symbol.Name, ".ctor", StringComparison.OrdinalIgnoreCase))
                    {
                        menuItem.Text = "Generate test for all constructors...";
                        regenerationMenuItem.Text = "Regenerate test for all constructors...";
                    }
                    else
                    {
                        menuItem.Text = "Generate test for " + symbol.Kind.ToString().ToLower(CultureInfo.CurrentCulture) + " '" + symbol.Name + "'...";
                        regenerationMenuItem.Text = "Regenerate test for " + symbol.Kind.ToString().ToLower(CultureInfo.CurrentCulture) + " '" + symbol.Name + "'...";
                    }
                }
                else if (baseType.HasValue)
                {
                    if (InterfaceGenerationStrategyFactory.Supports(baseType.Value))
                    {
                        menuItem.Visible = true;
                        regenerationMenuItem.Visible = Keyboard.IsKeyDown(Key.LeftShift);
                        menuItem.Text = "Generate tests for base type '" + baseType.Value.Type.Name + "'...";
                        regenerationMenuItem.Text = "Regenerate tests for base type '" + baseType.Value.Type.Name + "'...";
                    }
                }
            };

            commandService.AddCommand(menuItem);
            commandService.AddCommand(regenerationMenuItem);
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task InitializeAsync(UnitTestGeneratorPackage package)
        {
            await package.JoinableTaskFactory.SwitchToMainThreadAsync();
#pragma warning disable VSSDK006 // these services are always available
            _dte = (DTE2)await package.GetServiceAsync(typeof(DTE)).ConfigureAwait(true);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)).ConfigureAwait(true) as OleMenuCommandService;
            _instance = new GenerateTestForSymbolCommand(package, commandService);
        }

        internal static async Task<Tuple<SyntaxNode, ISymbol, TypeInfo>> GetTargetSymbolAsync(ITextView textView)
        {
            var caretPosition = textView.Caret.Position.BufferPosition;

            var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document != null)
            {
                var syntaxNode = await document.GetSyntaxRootAsync().ConfigureAwait(true);
                var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(true);

                var syntaxToken = syntaxNode.FindToken(caretPosition).Parent;

                if (syntaxToken != null)
                {
                    var declaration =
                        syntaxToken.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault() ??
                        syntaxToken.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault() ??
                        syntaxToken.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().FirstOrDefault() ??
                        syntaxToken.AncestorsAndSelf().OfType<IndexerDeclarationSyntax>().FirstOrDefault() ??
                        syntaxToken as StructDeclarationSyntax ??
                        syntaxToken as ClassDeclarationSyntax as SyntaxNode;

                    if (declaration != null)
                    {
                        return Tuple.Create(declaration, semanticModel.GetDeclaredSymbol(declaration), default(TypeInfo));
                    }

                    if (syntaxToken.Parent is BaseTypeSyntax)
                    {
                        var symbol = semanticModel.GetTypeInfo(syntaxToken);
                        return Tuple.Create(syntaxToken, default(ISymbol), symbol);
                    }
                }
            }

            return null;
        }

#pragma warning disable VSTHRD010 // checks are present in called member
        private void Execute(object sender, EventArgs e)
        {
            Execute(false);
        }

        private void ExecuteRegeneration(object sender, EventArgs e)
        {
            Execute(true);
        }
#pragma warning restore VSTHRD010

        private void Execute(bool withRegeneration)
        {
            GenerationItem generationItem;
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

                var messageLogger = new AggregateLogger();
                messageLogger.Initialize();

                var item = VsProjectHelper.GetProjectItem(document.FilePath);

                var options = _package.Options;
                var source = new ProjectItemModel(item, options.GenerationOptions);

                var projectItem = source.Item;

                var nameParts = VsProjectHelper.GetNameParts(projectItem);

                var targetProject = source.TargetProject;
                if (targetProject == null && !options.GenerationOptions.AllowGenerationWithoutTargetProject)
                {
                    throw new InvalidOperationException("Cannot create tests for '" + Path.GetFileName(source.FilePath) + "' because there is no project '" + source.TargetProjectName + "'");
                }

                var generationOptions = OptionsResolver.DetectFrameworks(targetProject, options.GenerationOptions);

                var projectDictionary = new Dictionary<EnvDTE.Project, HashSet<TargetAsset>>();
                var set = new HashSet<TargetAsset>();
                if (targetProject != null)
                {
                    projectDictionary[targetProject] = set;
                }

                var targetProjectItems = TargetFinder.FindTargetFolder(targetProject, nameParts, true, out var targetPath);
                if (targetProjectItems == null && !options.GenerationOptions.AllowGenerationWithoutTargetProject)
                {
                    // we asked to create targetProjectItems - so if it's null we effectively had a problem getting to the target project
                    throw new InvalidOperationException("Cannot create tests for '" + Path.GetFileName(source.FilePath) + "' because there is no project '" + source.TargetProjectName + "'");
                }

                _ = _package.JoinableTaskFactory.RunAsync(
                    () => Attempt.ActionAsync(
                        async () =>
                    {
                        var methodSymbol = await GetTargetSymbolAsync(textView).ConfigureAwait(true);

                        if (methodSymbol != null)
                        {
                            var sourceNameSpaceRoot = VsProjectHelper.GetProjectRootNamespace(source.Project);

                            Func<string, string> namespaceTransform;
                            if (source.TargetProject != null)
                            {
                                var targetNameSpaceRoot = VsProjectHelper.GetProjectRootNamespace(source.TargetProject);
                                namespaceTransform = NamespaceTransform.Create(sourceNameSpaceRoot, targetNameSpaceRoot);
                            }
                            else
                            {
                                namespaceTransform = x => x + ".Tests";
                            }

                            generationItem = new GenerationItem(source, methodSymbol.Item1, targetProjectItems, targetPath, set, namespaceTransform, generationOptions);

                            await CodeGenerator.GenerateCodeAsync(new[] { generationItem }, withRegeneration, _package, projectDictionary, messageLogger).ConfigureAwait(true);
                        }
                    }, _package));
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
