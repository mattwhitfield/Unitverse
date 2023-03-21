namespace Unitverse.Core.Options.Editing
{
    public interface IConfigurationWriterFactory
    {
        IConfigurationWriter? CreateWriterFor(SaveOption saveOption);
    }
}
