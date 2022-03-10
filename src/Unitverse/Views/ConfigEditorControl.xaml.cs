using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unitverse.Core.Options;

namespace Unitverse.Views
{
    /// <summary>
    /// Interaction logic for ConfigEditorControl.xaml
    /// </summary>
    public partial class ConfigEditorControl : UserControl
    {
        private ConfigEditorControlViewModel _viewModel;

        public ConfigEditorControl(IUnitTestGeneratorPackage package, string filename, Action onModified)
        {
            InitializeComponent();
            DataContext = _viewModel = new ConfigEditorControlViewModel(package, filename, onModified);
        }

        internal void SaveFile(string fileName)
        {
            ConfigExporter.WriteTo(fileName, _viewModel.Options);
        }
    }
}
