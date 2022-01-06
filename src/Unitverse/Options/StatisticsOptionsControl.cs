using System;
using System.Drawing;
using System.Windows.Forms;
using Unitverse.Core.Helpers;
using Unitverse.Helper;

namespace Unitverse.Options
{
    public partial class StatisticsOptionsControl : UserControl
    {
        private StatisticsOptions _options;

        public StatisticsOptionsControl()
        {
            InitializeComponent();
        }

        public void Associate(StatisticsOptions options)
        {
            EnableStatisticsCheckBox.Checked = options.Enabled;
            _options = options;

            UpdateStats();
        }

        private void UpdateStats()
        {
            var stats = StatisticsTracker.Get();
            UpdateStats(stats);
        }

        private void UpdateStats(IGenerationStatistics stats)
        {
            testClassesGeneratedLabel.Text = stats.TestClassesGenerated.ToString("N0");
            testMethodsGeneratedLabel.Text = stats.TestMethodsGenerated.ToString("N0");
            testMethodsRegeneratedLabel.Text = stats.TestMethodsRegenerated.ToString("N0");
            interfacesMockedLabel.Text = stats.InterfacesMocked.ToString("N0");
            typesConstructedLabel.Text = stats.TypesConstructed.ToString("N0");
            valuesGeneratedLabel.Text = stats.ValuesGenerated.ToString("N0");
        }

        private void EnableStatisticsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = EnableStatisticsCheckBox.Checked;
            if (_options != null)
            {
                _options.Enabled = enabled;
            }

            var color = enabled ? Color.Black : Color.Gray;
            captionLabel1.ForeColor = color;
            captionLabel2.ForeColor = color;
            captionLabel3.ForeColor = color;
            captionLabel4.ForeColor = color;
            captionLabel5.ForeColor = color;
            captionLabel6.ForeColor = color;

            testClassesGeneratedLabel.ForeColor = color;
            testMethodsGeneratedLabel.ForeColor = color;
            testMethodsRegeneratedLabel.ForeColor = color;
            interfacesMockedLabel.ForeColor = color;
            typesConstructedLabel.ForeColor = color;
            valuesGeneratedLabel.ForeColor = color;
        }

        private void StatisticsOptionsControl_VisibleChanged(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void resetStatsButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to reset statistics?", "Unitverse", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                StatisticsTracker.Reset();
                UpdateStats(new GenerationStatistics());
            }
        }
    }
}
