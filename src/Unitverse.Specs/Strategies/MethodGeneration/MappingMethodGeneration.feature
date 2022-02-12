Feature: MappingMethodGeneration
	I am checking the Mapping Method Generation strategy


Scenario: Can Call Method Generation
	Given I have a class defined as 
	"""
public class C3
{
    public OutputClass Map(InputClass inputClass)
    {
        return null;
    }
}
public class InputClass
{
    public string SomeProperty { get; }
    public string SomeOtherProperty { get; set; }
    public string WriteOnlyProperty { set { } }
}

public class OutputClass
{
    public string SomeProperty { get; set; }
    public string SomeOtherProperty { get; set; }
    public string WriteOnlyProperty { get; set; }
}
	"""
	And I set my test framework to 'NUnit3'
	And I set my mock framework to 'FakeItEasy'
	When I generate tests for the method using the strategy 'MappingMethodGenerationStrategy'
	Then I expect a method called 'MapPerformsMapping'
		And I expect it to contain the variable 'inputClass'
		And I expect it to contain the statement 'var result = _testClass.Map(inputClass);'
		And I expect it to contain the statement 'Assert.That(result.SomeProperty, Is.SameAs(inputClass.SomeProperty));'
		And I expect it to contain the statement 'Assert.That(result.SomeOtherProperty, Is.SameAs(inputClass.SomeOtherProperty));'
