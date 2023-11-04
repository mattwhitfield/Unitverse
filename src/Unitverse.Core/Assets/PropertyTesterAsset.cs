namespace Unitverse.Core.Assets
{
    using System;
    using Unitverse.Core.Options;

    public class PropertyTesterAsset : IAsset
    {
        public string AssetFileName => "PropertyTester.cs";

        public string Content(string targetNamespace, TestFrameworkTypes testFrameworkTypes)
        {
            if (string.IsNullOrWhiteSpace(targetNamespace))
            {
                throw new ArgumentNullException(nameof(targetNamespace));
            }

            if ((testFrameworkTypes & TestFrameworkTypes.XUnit) > 0)
            {
                return AssetResources.PropertyTesterXUnit.Replace("%targetNamespace%", targetNamespace);
            }

            if ((testFrameworkTypes & (TestFrameworkTypes.NUnit2 | TestFrameworkTypes.NUnit3 | TestFrameworkTypes.NUnit3Lifecycle)) > 0)
            {
                return AssetResources.PropertyTesterNUnit.Replace("%targetNamespace%", targetNamespace);
            }

            return AssetResources.PropertyTesterMSTest.Replace("%targetNamespace%", targetNamespace);
        }
    }
}