namespace Unitverse.Core.Helpers
{
    public static class Ignore
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "result",
            Justification = "Helper method to notify in code that we don't care about the result.")]
        public static void HResult(int? result)
        {
        }
    }
}