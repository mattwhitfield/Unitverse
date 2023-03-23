using Unitverse.Core.Options.Editing;

namespace Unitverse.Options
{
    public class ConfigurationWriterFactory : IConfigurationWriterFactory
    {
        private readonly IUnitTestGeneratorPackage _package;

        public ConfigurationWriterFactory(IUnitTestGeneratorPackage package)
        {
            _package = package;
        }

        public IConfigurationWriter CreateWriterFor(SaveOption saveOption)
        {
            switch (saveOption)
            {
                case SaveOption.ThisSession:
                    return new SessionConfigurationWriter();
                case SaveOption.ConfigurationFile:
                    return new ConfigurationFileConfigWriter();
                case SaveOption.VisualStudioConfiguration:
                    return _package;
            }

            return null;
        }
    }
}
