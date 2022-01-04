namespace Unitverse.Options
{
    using Microsoft.VisualStudio.Shell;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    [Guid("8f269a78-34f5-43f7-a0ec-e3503f1651d2")]
    public class ExportOptions : DialogPage
    {
        protected override IWin32Window Window
        {
            get
            {
                return new ExportOptionsControl();
            }
        }
    }
}