namespace Unitverse.Core.Tests.Templating.Model.Implementation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using Unitverse.Core.Templating.Model.Implementation;

    [TestFixture]
    public class LazyEnumerable_1Tests
    {
        private LazyEnumerable<string> _testClass;
        private Func<IEnumerable<string>> _source;
        private bool _wasEnumerated;

        [SetUp]
        public void SetUp()
        {
            _source = () => Source;
            _testClass = new LazyEnumerable<string>(_source);
            _wasEnumerated = false;
        }

        private IEnumerable<string> Source
        {
            get
            {
                yield return "TestValue1284137666";
                yield return "TestValue748916113";
                yield return "TestValue2139457593";
                _wasEnumerated = true;
            }
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new LazyEnumerable<string>(_source);

            // Assert
            instance.Should().NotBeNull();

            _wasEnumerated.Should().BeFalse();
        }

        [Test]
        public void ImplementsIEnumerable_T()
        {
            // Arrange
            int expectedCount = 3;
            int actualCount = 0;

            // Act
            using (var enumerator = _testClass.GetEnumerator())
            {
                enumerator.Should().NotBeNull();
                while (enumerator.MoveNext())
                {
                    actualCount++;
                    enumerator.Current.As<object>().Should().BeAssignableTo<string>();
                }
            }

            // Assert
            actualCount.Should().Be(expectedCount);
            _wasEnumerated.Should().BeTrue();
        }

        [Test]
        public void CanCallGetEnumerator()
        {
            // Act
            var result = _testClass.ToList();

            // Assert
            result.Should().BeEquivalentTo(Source);
            _wasEnumerated.Should().BeTrue();
        }

        [Test]
        public void CanCallGetEnumeratorForIEnumerable()
        {
            // Act
            var result = ((IEnumerable)_testClass).OfType<string>().ToList();

            // Assert
            result.Should().BeEquivalentTo(Source);
            _wasEnumerated.Should().BeTrue();
        }
    }
}