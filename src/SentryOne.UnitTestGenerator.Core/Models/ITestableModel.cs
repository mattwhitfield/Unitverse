namespace SentryOne.UnitTestGenerator.Core.Models
{
    public interface ITestableModel<out T>
    {
        string Name { get; }

        string OriginalName { get; }

        T Node { get; }

        void MutateName(string newName);
    }
}
