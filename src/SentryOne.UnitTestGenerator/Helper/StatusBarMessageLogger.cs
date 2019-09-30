namespace SentryOne.UnitTestGenerator.Helper
{
    using System;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using SentryOne.UnitTestGenerator.Core.Helpers;

    public class StatusBarMessageLogger : IMessageLogger
    {
        private IVsStatusbar _vsStatusBar;

        public void Initialize()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                _vsStatusBar = (IVsStatusbar)Package.GetGlobalService(typeof(SVsStatusbar));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _vsStatusBar = null;
            }
        }

        public void LogMessage(string message)
        {
            if (_vsStatusBar == null || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            if (_vsStatusBar.IsFrozen(out var frozen) != 0 || frozen != 0)
            {
                return;
            }

            try
            {
                Ignore.HResult(_vsStatusBar.SetText(message));
                Ignore.HResult(_vsStatusBar.FreezeOutput(1));
            }
            finally
            {
                Ignore.HResult(_vsStatusBar.FreezeOutput(0));
            }
        }
    }
}