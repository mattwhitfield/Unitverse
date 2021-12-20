using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitverse.Helper
{
    public static class VsMessageBox
    {
        public static void Show(string message, bool isError, IUnitTestGeneratorPackage package)
        {
            VsShellUtilities.ShowMessageBox(
                package,
                message,
                Unitverse.Constants.ExtensionName,
                isError ? OLEMSGICON.OLEMSGICON_CRITICAL : OLEMSGICON.OLEMSGICON_WARNING,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
