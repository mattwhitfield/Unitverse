using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Windows.Interop;

namespace Unitverse.Views
{
    public static class OwnerHelper
    {
        public static T ApplyOwner<T>(this T window, DTE dte) where T : System.Windows.Window
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var helper = new WindowInteropHelper(window);
#if VS2022
            helper.Owner = dte.MainWindow.HWnd;
#elif VS2019
            helper.Owner = new System.IntPtr(dte.MainWindow.HWnd);
#endif

            return window;
        }
    }
}
