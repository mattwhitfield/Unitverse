namespace SentryOne.UnitTestGenerator.Helper
{
    using System.Linq;
    using System.Management.Instrumentation;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;
    using SentryOne.UnitTestGenerator.Properties;
    using VSLangProj;
    using VSLangProj80;

    internal class OptionsResolver
    {
        public static IGenerationOptions DetectFrameworks(Project targetProject, IGenerationOptions baseOptions)
        {
            if (!baseOptions.AutoDetectFrameworkTypes)
            {
                return baseOptions;
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            var vsLangProj = targetProject.Object as VSProject;
            if (vsLangProj == null)
            {
                throw new InstanceNotFoundException(Strings.ReferencesHelper_AddReferencesToProject_The_VSLangProj_VSProject_instance_could_not_be_found_);
            }

            return FrameworkDetection.ResolveTargetFrameworks(vsLangProj.References.OfType<Reference3>().Select(x => new ReferencedAssembly(x.Name, x.MajorVersion)), baseOptions);
        }
    }
}
