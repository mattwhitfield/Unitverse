namespace Unitverse.Core.Helpers
{
    public interface IMessageLogger
    {
        void Initialize();

        void LogMessage(string message);
    }
}