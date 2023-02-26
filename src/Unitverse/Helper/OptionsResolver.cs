namespace Unitverse.Helper
{
    using System.Linq;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using VSLangProj;
    using VSLangProj80;

    internal class OptionsResolver
    {
        public static IGenerationOptions DetectFrameworks(Project targetProject, IGenerationOptions baseOptions, IMessageLogger messageLogger = null)
        {
            if (!baseOptions.AutoDetectFrameworkTypes || targetProject == null)
            {
                return baseOptions;
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            var vsLangProj = targetProject.Object as VSProject;
            if (vsLangProj == null)
            {
                return baseOptions;
            }

            var modifiedOptions = FrameworkDetection.ResolveTargetFrameworks(vsLangProj.References.OfType<Reference3>().Select(x => new ReferencedAssembly(x.Name, x.MajorVersion)), baseOptions);

            if (messageLogger != null)
            {
                if (modifiedOptions.FrameworkType != baseOptions.FrameworkType)
                {
                    messageLogger.LogMessage("Test framework type '" + modifiedOptions.FrameworkType + "' detected for project '" + targetProject.Name + "'.");
                }
                if (modifiedOptions.MockingFrameworkType != baseOptions.MockingFrameworkType)
                {
                    messageLogger.LogMessage("Mocking framework type '" + modifiedOptions.FrameworkType + "' detected for project '" + targetProject.Name + "'.");
                }
                if (modifiedOptions.UseFluentAssertions != baseOptions.UseFluentAssertions)
                {
                    messageLogger.LogMessage("Fluent assertions detected for project '" + targetProject.Name + "'.");
                }
                if (modifiedOptions.UseAutoFixture != baseOptions.UseAutoFixture)
                {
                    messageLogger.LogMessage("AutoFixture detected for project '" + targetProject.Name + "'.");
                }
                if (modifiedOptions.UseAutoFixtureForMocking != baseOptions.UseAutoFixtureForMocking)
                {
                    messageLogger.LogMessage("AutoFixture mocking detected for project '" + targetProject.Name + "'.");
                }
            }

            return modifiedOptions;
        }
    }
}
