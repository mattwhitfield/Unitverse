Feature: CanCallMethodGeneration
	I am checking the Can Call Method Generation strategy


Scenario: Can Call Method Generation
	Given I have a class defined as 
	"""
using System.Threading;

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

	public void Test3(CancellationToken token)
	{
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
	And I set my test framework to 'MS Test'
	And I set my mock framework to 'FakeItEasy'
	When I generate tests for the method using the strategy 'CanCallMethodGenerationStrategy'
	Then I expect a method called 'CanCallThisIsAMethod'
		And I expect it to contain the variable 'methodName'
		And I expect it to contain the statement '_testClass.ThisIsAMethod(methodName, methodValue);'
		And I expect it to contain the statement 'Assert.Fail("Create or modify test");'
	And I expect a method called 'CanCallWillReturnAString'
		And I expect it to have the attribute 'TestMethod'
		And I expect it to contain the statement 'var result = _testClass.WillReturnAString();'
		And I expect it to contain the statement 'Assert.Fail("Create or modify test");'
	And I expect a method called 'CanCallTest1'
		And I expect it to contain the statement '_testClass.Test1(outvartester);'
		And I expect it to contain the statement 'Assert.Fail("Create or modify test");'
	And I expect a method called 'CanCallTest2'
		And I expect it to contain the variable 'tester'
		And I expect it to contain the statement '_testClass.Test2(reftester);'
		And I expect it to contain the statement 'Assert.Fail("Create or modify test");'
	And I expect a method called 'CanCallTest3'
		And I expect it to contain the variable 'token'
		And I expect it to contain the statement 'var token = CancellationToken.None;'
		And I expect it to contain the statement '_testClass.Test3(token);'
		And I expect it to contain the statement 'Assert.Fail("Create or modify test");'
