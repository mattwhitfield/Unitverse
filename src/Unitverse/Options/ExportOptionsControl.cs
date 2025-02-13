using EditorConfig.Core;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Unitverse.Core;
using Unitverse.Core.Options;

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
                if (vsShell != null && vsShell.IsPackageLoaded(new Guid(Constants.ExtensionGuid), out var myPackage) == VSConstants.S_OK)
                {
                    var unitTestGeneratorPackage = (IUnitTestGeneratorPackage)myPackage;

                    using (var fbd = new FolderBrowserDialog())
                    {
                        fbd.Description = "Select a folder in which to save the options";

                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            var targetPath = Path.Combine(fbd.SelectedPath, CoreConstants.ConfigFileName);
                            if (File.Exists(targetPath))
                            {
                                if (MessageBox.Show(this, "The file '" + targetPath + "' already exists. Would you like to over-write it?", "File exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                {
                                    return;
                                }
                            }

                            ConfigExporter.WriteTo(targetPath, new object[] { unitTestGeneratorPackage.GenerationOptions, unitTestGeneratorPackage.NamingOptions, unitTestGeneratorPackage.StrategyOptions }, unitTestGeneratorPackage.ManualProjectMappings);

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

        private void Import_Click(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var vsShell = (IVsShell)ServiceProvider.GlobalProvider.GetService(typeof(IVsShell));
                if (vsShell != null && vsShell.IsPackageLoaded(new Guid(Constants.ExtensionGuid), out var myPackage) == VSConstants.S_OK)
                {
                    var unitTestGeneratorPackage = (IUnitTestGeneratorPackage)myPackage;

                    using (var ofd = new OpenFileDialog())
                    {
                        ofd.Title = "Import unitverse settings";
                        ofd.CheckFileExists = true;
                        ofd.CheckPathExists = true;

                        ofd.DefaultExt = "txt";
                        ofd.Filter = $"Unitverse config files (*{CoreConstants.ConfigFileName})|*{CoreConstants.ConfigFileName}|All files (*.*)|*.*";

                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            var generationOptionsMutators = EditorConfigFieldMapper.CreateMutatorSet<GenerationOptions>();
                            var namingOptionsMutators = EditorConfigFieldMapper.CreateMutatorSet<NamingOptions>();
                            var strategyOptionsMutators = EditorConfigFieldMapper.CreateMutatorSet<StrategyOptions>();

                            var file = new EditorConfigFile(ofd.FileName);

                            foreach (var section in file.Sections)
                            {
                                if (section.Glob.EndsWith("/Mappings", StringComparison.OrdinalIgnoreCase))
                                {
                                    foreach (var pair in section)
                                    {
                                        unitTestGeneratorPackage.ManualProjectMappings[pair.Key] = pair.Value;
                                    }
                                }
                                else
                                {
                                    foreach (var pair in section)
                                    {
                                        var applied = UnitTestGeneratorOptionsFactory.Apply(unitTestGeneratorPackage.GenerationOptions, pair, generationOptionsMutators, out _) ||
                                                      UnitTestGeneratorOptionsFactory.Apply(unitTestGeneratorPackage.NamingOptions, pair, namingOptionsMutators, out _) ||
                                                      UnitTestGeneratorOptionsFactory.Apply(unitTestGeneratorPackage.StrategyOptions, pair, strategyOptionsMutators, out _);
                                    }
                                }
                            }

                            MessageBox.Show(this, "Options imported from: " + ofd.FileName, "Unitverse", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(this, "An error occurred while importing settings: " + ex.Message, "Unitverse", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
