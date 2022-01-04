using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Windows.Forms;
using Unitverse.Core.Options;
using Unitverse.Helper;

namespace Unitverse.Options
{
    public partial class ExportOptionsControl : UserControl
    {
        public ExportOptionsControl()
        {
            InitializeComponent();
        }

        private void Export_Click(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var vsShell = (IVsShell)ServiceProvider.GlobalProvider.GetService(typeof(IVsShell));
                if (vsShell != null && vsShell.IsPackageLoaded(new Guid("35B315F3-278A-4C3F-81D6-4DC2264828AA"), out var myPackage) == VSConstants.S_OK)
                {
                    var unitTestGeneratorPackage = (IUnitTestGeneratorPackage)myPackage;

                    using (var fbd = new FolderBrowserDialog())
                    {
                        fbd.Description = "Select a folder in which to save the options";

                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            var targetPath = Path.Combine(fbd.SelectedPath, ".unitTestGeneratorConfig");
                            if (File.Exists(targetPath))
                            {
                                if (MessageBox.Show(this, "The file '" + targetPath + "' already exists. Would you like to over-write it?", "File exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                {
                                    return;
                                }
                            }

                            ConfigExporter.WriteTo(targetPath, new object[] { unitTestGeneratorPackage.GenerationOptions, unitTestGeneratorPackage.NamingOptions });

                            MessageBox.Show(this, "Options written to: " + targetPath, "Unitverse", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, "The unitverse packages has not yet completely loaded. Please try again in a few moments.", "Unitverse", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "An error occurred while writing out settings: " + ex.Message, "Unitverse", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
