Feature: CanConstructMultiConstructorGeneration
	I am checking the Can Construct Multi Constructor Generation strategy


Scenario: Can Construct Multi Constructor Generation
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
	And I set my test framework to 'XUnit'
	And I set my mock framework to 'RhinoMocks'
	When I generate unit tests for the class using strategy 'CanConstructMultiConstructorGenerationStrategy'
	Then I expect a method called 'CanConstruct'
		And I expect it to contain the statement 'var instance = new TestClass(_stringProp, _iTest);'
		And I expect it to contain the statement 'instance = new TestClass(_nullableIntProp, _iTest);'
		And I expect it to contain the statement 'instance = new TestClass(_thisIsAProperty, _iTest);'
		And I expect it to contain 3 statements called 'Assert.NotNull(instance);'