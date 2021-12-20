namespace Unitverse.Core.Tests.Models
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Models;
    using System;
    using FluentAssertions;

    [TestFixture]
    public class GenerationResultTests
    {
        private GenerationResult _testClass;
        private string _fileContent;
        private bool _anyMethodsEmitted;

        [SetUp]
        public void SetUp()
        {
            _fileContent = "TestValue1767881884";
            _anyMethodsEmitted = false;
            _testClass = new GenerationResult(_fileContent, _anyMethodsEmitted);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new GenerationResult(_fileContent, _anyMethodsEmitted);
            instance.Should().NotBeNull();
        }

        [Test]
        public void FileContentIsInitializedCorrectly()
        {
            _testClass.FileContent.Should().BeSameAs(_fileContent);
        }

        [Test]
        public void CanGetRequiredAssets()
        {
            _testClass.RequiredAssets.Should().NotBeNull();
            _testClass.RequiredAssets.Should().BeEmpty();
        }

        [Test]
        public void AnyMethodsEmittedIsInitializedCorrectly()
        {
            _testClass.AnyMethodsEmitted.Should().Be(_anyMethodsEmitted);
        }
    }
}