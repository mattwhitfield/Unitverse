namespace Unitverse.Core.Templating
{
    public static class TemplateHeaders
    {
        public const string TestMethodName = "TestMethodName";
        public const string Target = "Target";
        public const string Include = "Include";
        public const string Exclude = "Exclude";
        public const string IsAsync = "IsAsync";
        public const string IsStatic = "IsStatic";
        public const string Description = "Description";

        public const string IsExclusive = "IsExclusive"; // can only be matched if no other templates have been matched for the current item
        public const string StopMatching = "StopMatching"; // should stop looking for templates that apply to the current item after this is matched
        public const string Priority = "Priority"; // numeric priority - 1 comes first
    }
}
