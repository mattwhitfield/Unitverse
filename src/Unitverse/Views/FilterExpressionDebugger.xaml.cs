using EnvDTE;
using Microsoft.VisualStudio.Shell;
using SequelFilter;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Unitverse.Core.Options;
using Unitverse.Core.Options.Editing;
using Unitverse.Helper;
using Unitverse.Core.Templating.Model;

namespace Unitverse.Views
{
    /// <summary>
    /// Interaction logic for FilterExpressionDebugger.xaml
    /// </summary>
    public partial class FilterExpressionDebugger : System.Windows.Window
    {
        int _scale;

        public FilterExpressionDebugger(IOwningType owningType)
        {
            InitializeComponent();
            DataContext = _viewModel = new FilterExpressionDebuggerViewModel(owningType);

            PreviewMouseWheel += FilterExpressionDebugger_PreviewMouseWheel;
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);

            _scale = ZoomTracker.Get();
            RootScale.ScaleY = RootScale.ScaleX = 1 + (_scale / 100.0);
        }

        private void FilterExpressionDebugger_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                _scale += e.Delta / 12;
                _scale = Math.Max(0, _scale);
                _scale = Math.Min(100, _scale);

                RootScale.ScaleY = RootScale.ScaleX = 1 + (_scale / 100.0);
                ZoomTracker.Save(_scale);
            }
        }

        private FilterExpressionDebuggerViewModel _viewModel;

        private void OnOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
