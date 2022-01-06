namespace Unitverse.Options
{
    using Microsoft.VisualStudio.Shell;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    [Guid("0a1c8c15-6cdb-4497-8f13-7b47c8fd8779")]
    public class StatisticsOptions : DialogPage
    {
        public bool Enabled { get; set; } = true;

        protected override IWin32Window Window
        {
            get
            {
                var control = new StatisticsOptionsControl();
                control.Associate(this);
                return control;
            }
        }
    }
}