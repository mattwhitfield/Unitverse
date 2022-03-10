using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Unitverse.Core.Options;
using Unitverse.Helper;
using Unitverse.Options;

namespace Unitverse.Views
{
    /// <summary>
    /// Interaction logic for ConfigEditorControl.xaml
    /// </summary>
    public partial class ConfigEditorControl : UserControl
    {
        private ConfigEditorControlViewModel _viewModel;

        int _scale;

        public ConfigEditorControl(IUnitTestGeneratorPackage package, string filename, Action onModified)
        {
            // this line seems pointless, but it forces Unitverse.Core to be loaded if async package loading hasn't completed
            var x = new MutableGenerationOptions(new GenerationOptions());

            try
            {
                InitializeComponent();
            }
            catch (XamlParseException ex) when (ex.InnerException is FileNotFoundException)
            {
                var message = "The Unitverse package has not yet completely loaded by Visual Studio. Please wait for the package to be loaded and try again. You can force the package to load by using one of the 'Generate tests' or 'Go to tests' menu items.";
                VsMessageBox.Show(message, true, package);
                throw new InvalidOperationException(message);
            }

            DataContext = _viewModel = new ConfigEditorControlViewModel(package, filename, onModified);

            PreviewMouseWheel += ConfigEditorControl_PreviewMouseWheel;
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);

            _scale = ZoomTracker.Get();
            RootScale.ScaleY = RootScale.ScaleX = 1 + (_scale / 100.0);
        }

        private void ConfigEditorControl_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
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

        internal void SaveFile(string fileName)
        {
            ConfigExporter.WriteTo(fileName, _viewModel.Options);
        }
    }
}
