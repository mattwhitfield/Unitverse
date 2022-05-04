namespace Unitverse.Core.Tests.Models
{
    using Unitverse.Core.Models;
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using FakeItEasy;
    using Unitverse.Core.Options;

    [TestClass]
    public class ProjectCreationManifestTests
    {
        private ProjectCreationManifest _testClass;
        private string _name;
        private string _folderName;
        private IGenerationOptions _generationOptions;

        [TestInitialize]
        public void SetUp()
        {
            _name = "TestValue1063717306";
            _folderName = "TestValue589327833";
            _generationOptions = A.Fake<IGenerationOptions>();
            _testClass = new ProjectCreationManifest(_name, _folderName, _generationOptions);
        }

        [TestMethod]
        public void CanConstruct()
        {
            // Act
            var instance = new ProjectCreationManifest(_name, _folderName, _generationOptions);

            // Assert
            instance.Should().NotBeNull();
        }

        [TestMethod]
        public void CannotConstructWithNullGenerationOptions()
        {
            FluentActions.Invoking(() => new ProjectCreationManifest("TestValue1310356924", "TestValue2089427287", default(IGenerationOptions))).Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            FluentActions.Invoking(() => new ProjectCreationManifest(value, "TestValue1465155088", A.Fake<IGenerationOptions>())).Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void CannotConstructWithInvalidFolderName(string value)
        {
            FluentActions.Invoking(() => new ProjectCreationManifest("TestValue477371174", value, A.Fake<IGenerationOptions>())).Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void NameIsInitializedCorrectly()
        {
            _testClass.Name.Should().Be(_name);
        }

        [TestMethod]
        public void FolderNameIsInitializedCorrectly()
        {
            _testClass.FolderName.Should().Be(_folderName);
        }

        [TestMethod]
        public void GenerationOptionsIsInitializedCorrectly()
        {
            _testClass.GenerationOptions.Should().BeSameAs(_generationOptions);
        }
    }
}