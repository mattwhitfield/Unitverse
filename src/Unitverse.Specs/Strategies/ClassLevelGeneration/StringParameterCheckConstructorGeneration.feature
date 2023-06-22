Feature: StringParameterCheckConstructorGeneration
	I am checking the String Parameter Check Constructor Generation strategy


Scenario: String Parameter Check Constructor Generation
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
	And I set my mock framework to 'NSubstitute'
	When I generate unit tests for the class using strategy 'StringParameterCheckConstructorGenerationStrategy'
	Then I expect a method called 'CannotConstructWithInvalidStringProp'
		And I expect it to have the attribute 'DataTestMethod'
		And I expect it to have the attribute 'DataRow(null)'
		And I expect it to have the attribute 'DataRow("")'
		And I expect it to have the attribute 'DataRow("   ")'
		And I expect it to contain the statement 'Assert.ThrowsException<ArgumentNullException>(() => newTestClass(value, _iTest));'