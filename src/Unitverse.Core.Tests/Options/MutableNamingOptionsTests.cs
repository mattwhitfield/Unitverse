namespace Unitverse.Core.Tests.Options
{
    using Unitverse.Core.Options;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using NSubstitute;

    [TestFixture]
    public class MutableNamingOptionsTests
    {
        private MutableNamingOptions _testClass;
        private INamingOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = Substitute.For<INamingOptions>();

            _options.CanCallNamingPattern.Returns("CanCallNamingPattern");
            _options.CanConstructNamingPattern.Returns("CanConstructNamingPattern");
            _options.CannotConstructWithNullNamingPattern.Returns("CannotConstructWithNullNamingPattern");
            _options.CannotConstructWithInvalidNamingPattern.Returns("CannotConstructWithInvalidNamingPattern");
            _options.CanGetNamingPattern.Returns("CanGetNamingPattern");
            _options.CanSetAndGetNamingPattern.Returns("CanSetAndGetNamingPattern");
            _options.CanSetNamingPattern.Returns("CanSetNamingPattern");
            _options.ImplementsIEnumerableNamingPattern.Returns("ImplementsIEnumerableNamingPattern");
            _options.ImplementsIComparableNamingPattern.Returns("ImplementsIComparableNamingPattern");
            _options.PerformsMappingNamingPattern.Returns("PerformsMappingNamingPattern");
            _options.CannotCallWithNullNamingPattern.Returns("CannotCallWithNullNamingPattern");
            _options.CannotCallWithInvalidNamingPattern.Returns("CannotCallWithInvalidNamingPattern");
            _options.CanCallOperatorNamingPattern.Returns("CanCallOperatorNamingPattern");
            _options.CannotCallOperatorWithNullNamingPattern.Returns("CannotCallOperatorWithNullNamingPattern");
            _options.IsInitializedCorrectlyNamingPattern.Returns("IsInitializedCorrectlyNamingPattern");
            _options.TargetFieldName.Returns("TargetFieldName");
            _options.DependencyFieldName.Returns("DependencyFieldName");

            _testClass = new MutableNamingOptions(_options);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new MutableNamingOptions(_options);
            instance.Should().NotBeNull();

            instance.CanCallNamingPattern.Should().Be("CanCallNamingPattern");
            instance.CanConstructNamingPattern.Should().Be("CanConstructNamingPattern");
            instance.CannotConstructWithNullNamingPattern.Should().Be("CannotConstructWithNullNamingPattern");
            instance.CannotConstructWithInvalidNamingPattern.Should().Be("CannotConstructWithInvalidNamingPattern");
            instance.CanGetNamingPattern.Should().Be("CanGetNamingPattern");
            instance.CanSetAndGetNamingPattern.Should().Be("CanSetAndGetNamingPattern");
            instance.CanSetNamingPattern.Should().Be("CanSetNamingPattern");
            instance.ImplementsIEnumerableNamingPattern.Should().Be("ImplementsIEnumerableNamingPattern");
            instance.ImplementsIComparableNamingPattern.Should().Be("ImplementsIComparableNamingPattern");
            instance.PerformsMappingNamingPattern.Should().Be("PerformsMappingNamingPattern");
            instance.CannotCallWithNullNamingPattern.Should().Be("CannotCallWithNullNamingPattern");
            instance.CannotCallWithInvalidNamingPattern.Should().Be("CannotCallWithInvalidNamingPattern");
            instance.CanCallOperatorNamingPattern.Should().Be("CanCallOperatorNamingPattern");
            instance.CannotCallOperatorWithNullNamingPattern.Should().Be("CannotCallOperatorWithNullNamingPattern");
            instance.IsInitializedCorrectlyNamingPattern.Should().Be("IsInitializedCorrectlyNamingPattern");
            instance.TargetFieldName.Should().Be("TargetFieldName");
            instance.DependencyFieldName.Should().Be("DependencyFieldName");
        }

        [Test]
        public void CannotConstructWithNullOptions()
        {
            FluentActions.Invoking(() => new MutableNamingOptions(default(INamingOptions))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CanSetAndGetCanCallNamingPattern()
        {
            var testValue = "TestValue460699582";
            _testClass.CanCallNamingPattern = testValue;
            _testClass.CanCallNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetCanConstructNamingPattern()
        {
            var testValue = "TestValue1319470229";
            _testClass.CanConstructNamingPattern = testValue;
            _testClass.CanConstructNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetCannotConstructWithNullNamingPattern()
        {
            var testValue = "TestValue209755588";
            _testClass.CannotConstructWithNullNamingPattern = testValue;
            _testClass.CannotConstructWithNullNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetCannotConstructWithInvalidNamingPattern()
        {
            var testValue = "TestValue628823492";
            _testClass.CannotConstructWithInvalidNamingPattern = testValue;
            _testClass.CannotConstructWithInvalidNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetCanGetNamingPattern()
        {
            var testValue = "TestValue1605020025";
            _testClass.CanGetNamingPattern = testValue;
            _testClass.CanGetNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetCanSetAndGetNamingPattern()
        {
            var testValue = "TestValue547547956";
            _testClass.CanSetAndGetNamingPattern = testValue;
            _testClass.CanSetAndGetNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetCanSetNamingPattern()
        {
            var testValue = "TestValue1038548788";
            _testClass.CanSetNamingPattern = testValue;
            _testClass.CanSetNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetImplementsIEnumerableNamingPattern()
        {
            var testValue = "TestValue1880080496";
            _testClass.ImplementsIEnumerableNamingPattern = testValue;
            _testClass.ImplementsIEnumerableNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetImplementsIComparableNamingPattern()
        {
            var testValue = "TestValue809518467";
            _testClass.ImplementsIComparableNamingPattern = testValue;
            _testClass.ImplementsIComparableNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetPerformsMappingNamingPattern()
        {
            var testValue = "TestValue1156527905";
            _testClass.PerformsMappingNamingPattern = testValue;
            _testClass.PerformsMappingNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetCannotCallWithNullNamingPattern()
        {
            var testValue = "TestValue1654717623";
            _testClass.CannotCallWithNullNamingPattern = testValue;
            _testClass.CannotCallWithNullNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetCannotCallWithInvalidNamingPattern()
        {
            var testValue = "TestValue1623140815";
            _testClass.CannotCallWithInvalidNamingPattern = testValue;
            _testClass.CannotCallWithInvalidNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetCanCallOperatorNamingPattern()
        {
            var testValue = "TestValue2127746687";
            _testClass.CanCallOperatorNamingPattern = testValue;
            _testClass.CanCallOperatorNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetCannotCallOperatorWithNullNamingPattern()
        {
            var testValue = "TestValue628936274";
            _testClass.CannotCallOperatorWithNullNamingPattern = testValue;
            _testClass.CannotCallOperatorWithNullNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetIsInitializedCorrectlyNamingPattern()
        {
            var testValue = "TestValue876411311";
            _testClass.IsInitializedCorrectlyNamingPattern = testValue;
            _testClass.IsInitializedCorrectlyNamingPattern.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetTargetFieldName()
        {
            var testValue = "TestValue161296986";
            _testClass.TargetFieldName = testValue;
            _testClass.TargetFieldName.Should().BeSameAs(testValue);
        }

        [Test]
        public void CanSetAndGetDependencyFieldName()
        {
            var testValue = "TestValue1221049630";
            _testClass.DependencyFieldName = testValue;
            _testClass.DependencyFieldName.Should().BeSameAs(testValue);
        }
    }
}