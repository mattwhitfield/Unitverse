using System.ComponentModel;

namespace Unitverse.Options
{
    public class ProjectMappingOption
    {
        [Category("Mapping")]
        [DisplayName("Source project name")]
        [Description("The name of the source project to associate with the target project")]
        public string SourceProject { get; set; }

        [Category("Mapping")]
        [DisplayName("Target project name")]
        [Description("The name of the target project in which tests will be created when generating for code within the source project")]
        public string TargetProject { get; set; }

        public override string ToString()
        {
            return (SourceProject ?? string.Empty) + " -> " + (TargetProject ?? string.Empty);
        }
    }
}
