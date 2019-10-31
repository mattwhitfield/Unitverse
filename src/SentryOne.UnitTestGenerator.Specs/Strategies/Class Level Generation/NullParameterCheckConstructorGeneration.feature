Feature: NullParameterCheckConstructorGeneration
	I am checking the Null Parameter Check Construction Generation strategy


Scenario: Null Parameter Check Constructor Generation
	Given I have a class defined as 
	"""
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
	"""
	And I set my test framework to 'MS Test'
	And I set my mock framework to 'Moq'
	When I generate unit tests for the class using strategy 'NullParameterCheckConstructorGenerationStrategy'
	Then I expect a method called 'CannotConstructWithNullITest'
		And I expect it to have the attribute 'TestMethod'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => new TestClass({{{AnyString}}}, default(ITest)));'