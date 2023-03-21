using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.ComponentModel;

namespace Unitverse.Options
{
    internal class ProjectMappingOptions : DialogPage
    {
        [Category("Mappings")]
        [DisplayName("Project Mappings")]
        [Description("Manual mappings for the project in which tests should be created for a particular source project")]
        public List<ProjectMappingOption> ProjectMappings { get; set; } = new List<ProjectMappingOption>();
    }
}
