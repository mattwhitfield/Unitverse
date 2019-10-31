Feature: StringParameterCheckMethodGeneration
	I am checking the String Parameter Check Method Generation strategy


Scenario: String Parameter Check Method Generation
	Given I have a class defined as 
	"""
public class TestClass
{
	public void Test1(out string tester)
	{
		tester = "test"
	}

	public void Test2(ref string tester)
	{
		tester = "test"
	}

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
	And I set my test framework to 'MSTest'
	And I set my mock framework to 'RhinoMocks'
	When I generate tests for the method using the strategy 'StringParameterCheckMethodGenerationStrategy'
	Then I expect a method called 'CannotCallThisIsAMethodWithInvalidMethodName'
		And I expect it to have the attribute 'DataTestMethod'
		And I expect it to have the attribute 'DataRow(null)'
		And I expect it to have the attribute 'DataRow("")'
		And I expect it to have the attribute 'DataRow("   ")'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => _testClass.ThisIsAMethod(value, {{{AnyInteger}}}));'
	And I expect a method called 'CannotCallTest2WithInvalidTester'
		And I expect it to contain the variable 'tester'
		And I expect it to contain the statement 'Assert.ThrowsException<ArgumentNullException>(()=>_testClass.Test2(reftester));'
	And I expect no method with a name like '.*MethodValue.*'
	And I expect no method with a name like 'Test1'