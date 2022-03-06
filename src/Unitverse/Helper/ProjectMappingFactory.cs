namespace Unitverse.Helper
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    internal class ProjectMappingFactory
    {
        public static ProjectMapping CreateMappingFor(Project sourceProject, IUnitTestGeneratorOptions baseOptions)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // resolve the options for this project
            var projectOptions = UnitTestGeneratorOptionsFactory.Create(sourceProject.FileName, baseOptions.GenerationOptions, baseOptions.NamingOptions, baseOptions.StrategyOptions, baseOptions.StatisticsCollectionEnabled);

            // derive the target project name
            var targetProjectName = projectOptions.GenerationOptions.GetTargetProjectName(sourceProject.Name);
            
            // find the target project
            var targetProject = VsProjectHelper.FindProject(sourceProject.DTE.Solution, targetProjectName);

            // now resolve the frameworks in use by the target project (if target project is not null and options allow)
            var generationOptions = OptionsResolver.DetectFrameworks(targetProject, projectOptions.GenerationOptions);

            // now create the final options, including resolved frameworks
            var finalOptions = new UnitTestGeneratorOptions(generationOptions, projectOptions.NamingOptions, projectOptions.StrategyOptions, projectOptions.StatisticsCollectionEnabled);

            // now return the mapping from source to target, along with the per-project options
            return new ProjectMapping(sourceProject, targetProject, targetProjectName, finalOptions);
        }
    }
}
