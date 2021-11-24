namespace Unitverse.Helper
{
    using System.Linq;
    using System.Management.Instrumentation;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Properties;
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
