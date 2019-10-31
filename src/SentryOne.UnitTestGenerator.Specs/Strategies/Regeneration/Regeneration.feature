Feature: Regeneration
	I am checking that regeneration adds fields

Scenario: Regeneration of a constructor
	Given I have a class defined as 
		"""
namespace TestNamespace
{
    public interface ITest
    {
        int ThisIsAProperty {get;set;}
    }

    public interface ITest2
    {
        int ThisIsAProperty {get;set;}
    }

    public class TestClass
    {
        public TestClass(ITest test, ITest2 otherTest)
        {

        }

        public ITest Test { get; }

        public ITest2 OtherTest { get; }
    }
}
        """
	And an existing test class
	    """
namespace TestNamespace.Tests
{
    using TestNamespace.SubNameSpace;
    using System;
    using NUnit.Framework;
    using NSubstitute;

    [TestFixture]
    public class TestClassTests
    {
        private TestClass _testClass;
        private ITest _test;

        [SetUp]
        public void SetUp()
        {
            _test = Substitute.For<ITest>();
            _testClass = new TestClass(_test);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new TestClass(_test);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullITest()
        {
            Assert.Throws<ArgumentNullException>(() => new TestClass(default(ITest)));
        }
		
        [Test]
        public void TestIsInitializedCorrectly()
        {
            Assert.That(_testClass.Test, Is.EqualTo(_test));
        }
    }
}
		"""
	And I set my test framework to 'NUnit3'
	And I set my mock framework to 'NSubstitute'
	When I regenerate tests for all constructors
	Then I expect a field of type 'ITest2' with name '_otherTest'
	And I expect the method 'SetUp'
		And I expect it to contain an assignment for '_otherTest'
