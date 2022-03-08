namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public class ExcludedFromUserInterfaceAttributeTests
    {
        private ExcludedFromUserInterfaceAttribute _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new ExcludedFromUserInterfaceAttribute();
        }
    }
}