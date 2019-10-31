Feature: StandardClassGeneration
	I am checking the Standard Class Generation strategy

Scenario: Standard Class Generation
	Given I have a class defined as 
		"""
namespace TestNamespace.SubNameSpace
{

    public interface ITest
    {
        int ThisIsAProperty {get;set;}
    }

    public class TestClass
    {
        public TestClass(string stringProp, ITest iTest)
        {

        }
 
        public TestClass(int? nullableIntProp, ITest iTest)
        {

        }
 
        public TestClass(int thisIsAProperty, ITest iTest)
        {

        }
		public void ThisIsAMethod(string methodName, int methodValue)
	    {
		    System.Console.WriteLine("Testing this");
	    }

        public string WillReturnAString()
        {
            return "Hello";
        }

        private string _thisIsAString = string.Empty;
        public string ThisIsAWriteOnlyString { set { _thisIsAString = value; }}

        public int ThisIsAProperty { get;set;}

        public ITest GetITest { get; }

        public TestClass ThisClass {get;set;}
    }
}
        """
	And I set my test framework to 'NUnit3'
	And I set my mock framework to 'NSubstitute'
	When I generate tests for the class using strategy 'StandardClassGenerationStrategy'
	Then I expect a field of type 'string' with name '_stringProp'
	And I expect a field of type 'int' with name '_thisIsAProperty'
	And I expect a field of type 'int?' with name '_nullableIntProp'
	And I expect a field of type 'ITest' with name '_iTest'
	And I expect a field of type 'TestClass' with name '_testClass'
	And I expect the method 'SetUp'
		And I expect it to have the attribute 'SetUp'
		And I expect it to contain an assignment for '_stringProp'
		And I expect it to contain an assignment for '_thisIsAProperty'
		And I expect it to contain an assignment for '_nullableIntProp'
		And I expect it to contain an assignment for '_iTest'
		And I expect it to contain an assignment for '_testClass'