Feature: StaticClassGeneration
	I am checking the Static Class Generation strategy

Scenario: Static Class Generation
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
	When I generate tests for the class using strategy 'StaticClassGenerationStrategy'
	Then I expect the class to have the modifier 'static'