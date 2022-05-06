namespace Unitverse.Core.Tests.Models
{
    using Unitverse.Core.Models;
    using System;
    using FluentAssertions;
    using FakeItEasy;
    using Unitverse.Core.Options;
    using NUnit.Framework;

    [TestFixture]
    public class ProjectCreationManifestTests
    {
        private ProjectCreationManifest _testClass;
        private string _name;
        private string _folderName;
        private IGenerationOptions _generationOptions;

        [SetUp]
        public void SetUp()
        {
            _name = "NewProject";
            _folderName = "C:\\Stuff";
            _generationOptions = A.Fake<IGenerationOptions>();
            _testClass = new ProjectCreationManifest(_name, _folderName, _generationOptions);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new ProjectCreationManifest(_name, _folderName, _generationOptions);

            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullGenerationOptions()
        {
            FluentActions.Invoking(() => new ProjectCreationManifest("TestValue1310356924", "TestValue2089427287", default(IGenerationOptions))).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            FluentActions.Invoking(() => new ProjectCreationManifest(value, "TestValue1465155088", A.Fake<IGenerationOptions>())).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidFolderName(string value)
        {
            FluentActions.Invoking(() => new ProjectCreationManifest("TestValue477371174", value, A.Fake<IGenerationOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void NameIsInitializedCorrectly()
        {
            _testClass.Name.Should().Be(_name);
        }

        [Test]
        public void FolderNameIsInitializedCorrectly()
        {
            _testClass.FolderName.Should().Be(_folderName);
        }

        [Test]
        public void GenerationOptionsIsInitializedCorrectly()
        {
            _testClass.GenerationOptions.Should().BeSameAs(_generationOptions);
        }

        [Test]
        public void CanGetProjectFileName()
        {
            // Assert
            _testClass.ProjectFileName.Should().Be("C:\\Stuff\\NewProject\\NewProject.csproj");
        }
    }
}