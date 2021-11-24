namespace Unitverse.Core.Options
{
    using System;

    public class MutableGenerationOptions : IGenerationOptions
    {
        public MutableGenerationOptions(IGenerationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            FrameworkType = options.FrameworkType;
            MockingFrameworkType = options.MockingFrameworkType;
            AllowGenerationWithoutTargetProject = options.AllowGenerationWithoutTargetProject;
            TestProjectNaming = options.TestProjectNaming;
            TestFileNaming = options.TestFileNaming;
            TestTypeNaming = options.TestTypeNaming;
            UseFluentAssertions = options.UseFluentAssertions;
            AutoDetectFrameworkTypes = options.AutoDetectFrameworkTypes;
        }

        public TestFrameworkTypes FrameworkType { get; set; }

        public MockingFrameworkType MockingFrameworkType { get; set; }

        public bool AllowGenerationWithoutTargetProject { get; set; }

        public string TestProjectNaming { get; set; }

        public string TestFileNaming { get; set; }

        public string TestTypeNaming { get; set; }

        public bool UseFluentAssertions { get; set; }

        public bool AutoDetectFrameworkTypes { get; set; }
    }
}