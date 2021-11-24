namespace Unitverse.Helper
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Helpers;

    public class AggregateLogger : IMessageLogger
    {
        private readonly List<IMessageLogger> _loggers = new List<IMessageLogger> { new StatusBarMessageLogger(), new OutputWindowMessageLogger() };

        public void Initialize()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _loggers.ForEach(x => x.Initialize());
        }

        public void LogMessage(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _loggers.ForEach(x => x.LogMessage(message));
        }
    }
}
