using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitverse.Helper
{
    public static class GoToTestsHelper
    {
        public static void FindTestsFor(ProjectItemModel source, IUnitTestGeneratorPackage package)
        {
            var mapping = ProjectMappingFactory.CreateMappingFor(source.Project, package.Options, false, false);

            var status = TargetFinder.FindExistingTargetItem(source, mapping, out var targetItem);

            // retry the find without taking into account manually selected mappings if we didn't find it
            if (status != FindTargetStatus.Found)
            {
                mapping = ProjectMappingFactory.CreateMappingFor(source.Project, package.Options, false, true);
                status = TargetFinder.FindExistingTargetItem(source, mapping, out targetItem);
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
