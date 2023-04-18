using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unitverse.Core.Helpers;

namespace Unitverse.Helper
{
    internal static class TextViewHelper
    {
        internal static IWpfTextView GetTextView(IServiceProvider serviceProvider)
        {
            var textManager = (IVsTextManager)serviceProvider.GetService(typeof(SVsTextManager));
            if (textManager != null)
            {
                textManager.GetActiveView(1, null, out var textView);

                var componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));
                var adapterService = componentModel?.GetService<Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService>();

                return adapterService?.GetWpfTextView(textView);
            }

            return null;
        }

        internal static async Task<TargetSymbol> GetTargetSymbolAsync(ITextView textView)
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
                        syntaxToken as RecordDeclarationSyntax ??
                        syntaxToken as StructDeclarationSyntax ??
                        syntaxToken as ClassDeclarationSyntax as SyntaxNode;

                    if (declaration != null)
                    {
                        return new TargetSymbol(declaration, semanticModel.GetDeclaredSymbol(declaration), default);
                    }

                    if (syntaxToken.Parent is BaseTypeSyntax)
                    {
                        var typeInfo = semanticModel.GetTypeInfo(syntaxToken);
                        return new TargetSymbol(syntaxToken, default, typeInfo);
                    }
                }
            }

            return null;
        }
    }
}
