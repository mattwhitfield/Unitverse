namespace Unitverse.Helper
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Unitverse.Core.Helpers;

    public sealed class OutputWindowMessageLogger : IMessageLogger
    {
        private IVsOutputWindow _outputWindow;
        private IVsOutputWindowPane _testingOutputPane;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "External exception types unknown")]
        public void Initialize()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                _outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            }
            catch (Exception)
            {
                _outputWindow = null;
            }

            if (_outputWindow == null)
            {
                return;
            }

            var generatorOutputWindowPaneId = new Guid("D6002C1E-2DB8-4C9D-996D-A29364FB8DAC");

            ErrorHandler.ThrowOnFailure(_outputWindow.CreatePane(ref generatorOutputWindowPaneId, "Unitverse", 0, 1));
            ErrorHandler.ThrowOnFailure(_outputWindow.GetPane(ref generatorOutputWindowPaneId, out _testingOutputPane));

            if (_testingOutputPane != null)
            {
                ErrorHandler.ThrowOnFailure(_testingOutputPane.Clear());
                ErrorHandler.ThrowOnFailure(_testingOutputPane.Activate());
            }
        }

        public void LogMessage(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_testingOutputPane != null)
            {
                ErrorHandler.ThrowOnFailure(_testingOutputPane.OutputStringThreadSafe(message + Environment.NewLine));
            }
        }
    }
}