namespace Unitverse.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Unitverse.Core.Helpers;

    internal static class VsProjectHelper
    {
        private static readonly Guid FolderProject = new Guid("{66A26720-8FB5-11D2-AA7E-00C04F688DDE}");

        private static IEnumerable<IVsProject> LoadedProjects
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));

                var guid = Guid.Empty;
                solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref guid, out var enumerator);
                var hierarchy = new IVsHierarchy[] { null };
                for (enumerator.Reset(); enumerator.Next(1, hierarchy, out var fetched) == VSConstants.S_OK && fetched == 1; /*nothing*/)
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    if (hierarchy[0] is IVsProject project)
                    {
                        yield return project;
                    }
                }
            }
        }

        public static Project FindProject(Solution solution, string projectName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            return FindProjects(solution.Projects.OfType<Project>()).FirstOrDefault(x => string.Equals(x.Name, projectName, StringComparison.OrdinalIgnoreCase));
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
        }

        public static IEnumerable<Project> FindProjects(Solution solution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return FindProjects(solution.Projects.OfType<Project>());
        }

        public static string SafeFileName(this Project project)
        {
            string projectFileName;
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                projectFileName = project.FileName;
            }
            catch
            {
                projectFileName = string.Empty;
            }

            return projectFileName;
        }

        public static Guid GetProjectId(this Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            IVsHierarchy hierarchy;

            solution.GetProjectOfUniqueName(project.FullName, out hierarchy);

            if (hierarchy != null)
            {
                hierarchy.GetGuidProperty(
                            VSConstants.VSITEMID_ROOT,
                            (int)__VSHPROPID.VSHPROPID_ProjectIDGuid,
                            out var projectGuid);

                return projectGuid;
            }

            return Guid.Empty;
        }

        private static IEnumerable<Project> FindProjects(IEnumerable<Project> currentProjects)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (var project in currentProjects)
            {
                if (project.SafeFileName().EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                {
                    yield return project;
                }

                if (Guid.TryParse(project.Kind, out var projectKindGuid) && projectKindGuid == FolderProject)
                {
                    var subProjects = new List<Project>();
                    foreach (var projectItem in project.ProjectItems.OfType<ProjectItem>())
                    {
                        if (projectItem.SubProject != null)
                        {
                            subProjects.Add(projectItem.SubProject);
                        }
                    }

                    foreach (var subProject in FindProjects(subProjects))
                    {
                        yield return subProject;
                    }
                }
            }
        }

        public static List<string> GetNameParts(ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var nameParts = new List<string>();
            var current = projectItem;
            while (current != null)
            {
                nameParts.Add(current.Name);
                current = current.Collection.Parent as ProjectItem;
            }

            return nameParts;
        }

        public static void ActivateItem(ProjectItem testProjectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            const string vsViewKindCode = "{7651A701-06E5-11D1-8EBD-00A0C90F26EA}";

            try
            {
                if (!testProjectItem.IsOpen[vsViewKindCode])
                {
                    testProjectItem.Open(vsViewKindCode);
                }

                testProjectItem.Document.Activate();
            }

            // ReSharper disable once EmptyGeneralCatchClause - extensibility means exception types are unknown
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
            }
        }

        public static ProjectItem GetProjectItem(string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var hierarchy = GetVsHierarchyFromFilepath(filePath, out var itemId);

            return GetProjectItem(hierarchy, itemId);
        }

        public static string GetProjectRootNamespace(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var defaultNamespaceProperty = project.Properties.Item("DefaultNamespace");

            if (defaultNamespaceProperty?.Value == null)
            {
                return "Testing";
            }

            return defaultNamespaceProperty.Value.ToString();
        }

        private static IVsHierarchy GetVsHierarchyFromFilepath(string filepath, out uint itemId)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrWhiteSpace(filepath))
            {
                throw new ArgumentNullException(nameof(filepath));
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            foreach (var project in LoadedProjects)
            {
                if (project is IVsHierarchy hierarchy)
                {
                    if (project.IsDocumentInProject(filepath, out var found, new VSDOCUMENTPRIORITY[1], out itemId) == VSConstants.S_OK && found != 0)
                    {
                        // ReSharper disable once SuspiciousTypeConversion.Global
                        return hierarchy;
                    }
                }
            }

            itemId = VSConstants.VSITEMID_ROOT;
            return null;
        }

        private static ProjectItem GetProjectItem(IVsHierarchy hierarchy, uint itemId)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (hierarchy == null)
            {
                return null;
            }

            Ignore.HResult(hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out var objProj));
            return objProj as ProjectItem;
        }
    }
}