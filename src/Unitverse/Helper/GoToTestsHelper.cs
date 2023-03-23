using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System;

namespace Unitverse.Helper
{
    public static class GoToTestsHelper
    {
        public static void FindTestsFor(ISymbol symbol, ProjectItemModel source, IUnitTestGeneratorPackage package, AggregateLogger logger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var mapping = ProjectMappingFactory.CreateMappingFor(source.Project, package.Options, false, false, null, package);

            var status = TargetFinder.FindExistingTargetItem(symbol, source, mapping, package, logger, out var targetItem, out _);

            // retry the find without taking into account manually selected mappings if we didn't find it
            if (status != FindTargetStatus.Found)
            {
                mapping = ProjectMappingFactory.CreateMappingFor(source.Project, package.Options, false, true, null, package);
                status = TargetFinder.FindExistingTargetItem(symbol, source, mapping, package, logger, out targetItem, out _);
            }

            switch (status)
            {
                case FindTargetStatus.FileNotFound:
                case FindTargetStatus.FolderNotFound:
                    throw new InvalidOperationException("No unit tests were found for the selected file.");
                case FindTargetStatus.ProjectNotFound:
                    throw new InvalidOperationException("Cannot go to tests for this item because there is no project '" + mapping.TargetProjectName + "'");
            }

            VsProjectHelper.ActivateItem(targetItem);
        }
    }
}
