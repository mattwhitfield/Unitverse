namespace Unitverse.Helper
{
    using System;
    using System.Globalization;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Constants = Unitverse.Constants;
    using Task = System.Threading.Tasks.Task;

    public static class Attempt
    {
        public static void Action(Action action, IUnitTestGeneratorPackage package)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action();
            }
            catch (InvalidOperationException ex)
            {
                VsShellUtilities.ShowMessageBox(
                    package,
                    ex.Message,
                    Constants.ExtensionName,
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                VsShellUtilities.ShowMessageBox(
                    package,
                    string.Format(CultureInfo.CurrentCulture, "Exception raised\n{0}", ex),
                    Constants.ExtensionName,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        public static async Task ActionAsync(Func<Task> action, IUnitTestGeneratorPackage package)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                await action().ConfigureAwait(true);
            }
            catch (InvalidOperationException ex)
            {
                await package.JoinableTaskFactory.SwitchToMainThreadAsync();

                VsShellUtilities.ShowMessageBox(
                    package,
                    ex.Message,
                    Constants.ExtensionName,
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                await package.JoinableTaskFactory.SwitchToMainThreadAsync();

                VsShellUtilities.ShowMessageBox(
                    package,
                    string.Format(CultureInfo.CurrentCulture, "Exception raised\n{0}", ex),
                    Constants.ExtensionName,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
    }
}