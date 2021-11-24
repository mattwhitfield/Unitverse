namespace Unitverse.Core.Helpers
{
    using System;

    public static class NamespaceTransform
    {
        public static Func<string, string> Create(string sourceNameSpaceRoot, string targetNameSpaceRoot)
        {
            if (string.IsNullOrWhiteSpace(sourceNameSpaceRoot))
            {
                throw new ArgumentNullException(nameof(sourceNameSpaceRoot));
            }

            if (string.IsNullOrWhiteSpace(targetNameSpaceRoot))
            {
                throw new ArgumentNullException(nameof(targetNameSpaceRoot));
            }

            return sourceNameSpace => Transform(sourceNameSpaceRoot, targetNameSpaceRoot, sourceNameSpace);
        }

        private static string Transform(string sourceNameSpaceRoot, string targetNameSpaceRoot, string sourceNameSpace)
        {
            if (string.IsNullOrWhiteSpace(sourceNameSpace))
            {
                return targetNameSpaceRoot;
            }

            if (sourceNameSpace.StartsWith(sourceNameSpaceRoot, StringComparison.OrdinalIgnoreCase) && sourceNameSpace.Length > sourceNameSpaceRoot.Length)
            {
                return targetNameSpaceRoot + sourceNameSpace.Substring(sourceNameSpaceRoot.Length);
            }

            return targetNameSpaceRoot;
        }
    }
}
