Feature: CanConstructNoConstructorGeneration
	I am checking the Can Construct No Constructor Generation Strategy


Scenario: Can Construct No Constructor Generation
	Given I have a class defined as 
	"""
namespace TestNamespace.SubNameSpace
{
	using System;
	
    public class SomePoco
    {
        public int ThisIsAProperty {get;set;}
        public string ThisIsAProperty2 {get;set;}
        public Guid ThisIsAProperty3 {get;set;}
        public Guid? ThisIsAProperty4 {get;set;}
        public int ThisIsAProperty5 {get;private set;}
		private int ThisIsAProperty6 {get;set;}
        public Guid ThisIsAProperty7 {get;set;}
        public Guid ThisIsAProperty8 {get;set;}
        public Guid ThisIsAProperty9 {get;set;}
    }

    public class TestClass
    {
		SomePoco _poco;

        public TestClass(SomePoco poco)
        {
			_poco = poco;
        }
 
        public SomePoco Poco => _poco;
    }
}
	"""
	And I set my test framework to 'XUnit'
	And I set my mock framework to 'FakeItEasy'
	When I generate unit tests for the class using strategy 'CanConstructNoConstructorGenerationStrategy'
	Then I expect a method called 'CanConstruct'
		And I expect it to contain the statement 'var instance = new SomePoco();'
		And I expect it to contain 1 statements called 'Assert.NotNull(instance);'