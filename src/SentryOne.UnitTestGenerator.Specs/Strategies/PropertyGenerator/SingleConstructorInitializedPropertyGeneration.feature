Feature: SingleConstructorInitializedPropertyGeneration
	I am checking the Single Constructor Initialized Property Generation strategy


Scenario: Single Constructor Initialized Property Generation
	Given I have a class defined as 
	"""
public class TestClass
{
	string _str1;

    public TestClass(string str1)
    {
		_str1 = str1;
    }

    public string Str1 => _str1;
}
	"""
	And I set my test framework to 'NUnit3'
	And I set my mock framework to 'FakeItEasy'
	When I generate tests for the property using the strategy 'SingleConstructorInitializedPropertyGenerationStrategy'
	Then I expect a method called 'Str1IsInitializedCorrectly'
		And I expect it to have the attribute 'Test'
		And I expect it to contain 1 statements called 'Assert.That(_testClass.Str1, Is.EqualTo(_str1));'