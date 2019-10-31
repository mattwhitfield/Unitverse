Feature: MultiConstructorInitializedPropertyGeneration
	I am checking the Multi Constructor Initialized  Property Generation strategy


Scenario: Multi Constructor Initialized Property Generation
	Given I have a class defined as 
	"""
namespace TestNamespace.SubNameSpace
{

    public interface ITest
    {
        int ThisIsAProperty {get;set;}
		string StringProp {get;set;}
    }

    public class TestClass
    {
        public TestClass(string stringProp, ITest iTest)
        {

        }
 
        public TestClass(string stringProp)
        {

        }
 
        public TestClass(int? nullableIntProp, ITest iTest)
        {

        }
 
        public TestClass(int thisIsAProperty, ITest iTest)
        {

        }
 
        public TestClass(int thisIsAProperty)
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

		public string StringProp { get;set;}

        public ITest GetITest { get; }

        public TestClass ThisClass {get;set;}
    }
}
	"""
	And I set my test framework to 'NUnit3'
	And I set my mock framework to 'FakeItEasy'
	When I generate tests for the property using the strategy 'MultiConstructorInitializedPropertyGenerationStrategy'
	Then I expect a method called 'ThisIsAPropertyIsInitializedCorrectly'
		And I expect it to contain the statement 'Assert.That(_testClass.ThisIsAProperty, Is.EqualTo(_thisIsAProperty));'
	And I expect a method called 'StringPropIsInitializedCorrectly'
	And I expect no method with a name like '.*nullableIntProp.*'