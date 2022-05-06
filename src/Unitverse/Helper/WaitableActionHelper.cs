namespace Unitverse.Helper
{
    using System;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Unitverse.Core.Helpers;

    internal static class WaitableActionHelper
    {
        public static async System.Threading.Tasks.Task RunWaitableActionAsync(IUnitTestGeneratorPackage package, IMessageLogger messageLogger, string title, Func<Action<string>, System.Threading.Tasks.Task> function)
        {
            await package.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (package.GetService(typeof(SVsThreadedWaitDialogFactory)) is IVsThreadedWaitDialogFactory dialogFactory)
            {
                dialogFactory.CreateInstance(out var dialog);
                if (dialog != null)
                {
                    dialog.StartWaitDialog(title, title, string.Empty, null, title, 0, false, true);
                    
                    void LogMessageAction(string message)
                    {
                        ThreadHelper.ThrowIfNotOnUIThread();
                        dialog.StartWaitDialog(title, message, string.Empty, null, message, 0, false, true);
                        messageLogger.LogMessage(message);
                    }

                    try
                    {
                        await function(LogMessageAction);
                    }
                    finally
                    {
                        dialog.EndWaitDialog(out _);
                    }
                    return;
                }
            }

            await function(messageLogger.LogMessage);
        }
    }
}
