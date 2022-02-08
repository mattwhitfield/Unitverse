namespace Unitverse.Helper
{
    using System;
    using System.Globalization;
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
                VsMessageBox.Show(ex.Message, false, package);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                VsMessageBox.Show(string.Format(CultureInfo.CurrentCulture, "Exception raised\n{0}", ex), true, package);
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

                VsMessageBox.Show(ex.Message, false, package);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                await package.JoinableTaskFactory.SwitchToMainThreadAsync();

                VsMessageBox.Show(string.Format(CultureInfo.CurrentCulture, "Exception raised\n{0}", ex), true, package);
            }
        }
    }
}