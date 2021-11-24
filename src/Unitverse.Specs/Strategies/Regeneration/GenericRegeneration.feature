Feature: GenericRegeneration
	I am checking that regeneration does not add generic aliases that exist

Scenario: Regeneration of a constructor
	Given I have a class defined as 
		"""
namespace TestNamespace
{
    public class TestClass<T>
    {
        public TestClass()
        {
        }

        public void Method()
        { 
        }
    }
}
        """
	And an existing test class
	    """
namespace TestNamespace.Tests
{
    using T = System.String;
    using TestNamespace.SubNameSpace;
    using System;
    using NUnit.Framework;
    using NSubstitute;

    [TestFixture]
    public class TestClass_1Tests
    {
        private TestClass<T> _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new TestClass();
        }
    }
}
		"""
	And I set my test framework to 'NUnit3'
	And I set my mock framework to 'NSubstitute'
	When I regenerate tests for all constructors
	Then I expect the method 'CanConstruct'
    And I expect only one generic type alias
