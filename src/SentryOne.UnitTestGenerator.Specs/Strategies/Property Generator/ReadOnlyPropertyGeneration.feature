Feature: ReadOnlyPropertyGeneration
	I am checking the Read Only Property Generation strategy


Scenario: Read Only Property Generation
	Given I have a class defined as 
	"""
namespace SentryOne.Document.Web.Services.Workflow.BackgroundTaskExecutors.AzureFunctions
{
    using System;

    public abstract class AzureFunctionsBackgroundTaskExecutor
    {
        public AzureFunctionsBackgroundTaskExecutor()
        {
        }

        public abstract string SupportedStage { get; }

        public abstract string SupportedStage2 { get; }

		protected abstract string ProtectedProperty { get; }

		public string SomeOtherProperty { get; set; }

        public abstract string CreateMessage(string item, long tenantId);

        protected abstract string CreateMessage2(string item, long tenantId);
    }

	public abstract class AzureFunctionsBackgroundTaskExecutor2 : AzureFunctionsBackgroundTaskExecutor
    {
        public AzureFunctionsBackgroundTaskExecutor2()
        {
        }

        public override string SupportedStage => "hello";

        public override string CreateMessage(string item, long tenantId) { return "hello"; }
    }
}
	"""
	And I set my test framework to 'NUnit3'
	And I set my mock framework to 'FakeItEasy'
	When I generate tests for the property using the strategy 'ReadOnlyPropertyGenerationStrategy'
	Then I expect a method called 'CanGetSupportedStage'
		And I expect it to contain the statement 'Assert.That(_testClass.SupportedStage,Is.InstanceOf<string>());'
		And I expect it to contain the statement 'Assert.Fail("Create or modify test");'
	And I expect a method called 'CanGetSupportedStage2'
		And I expect it to have the attribute 'Test'
	And I expect a method called 'CanGetProtectedProperty'
		And I expect it to contain the statement 'Assert.That(_testClass.ProtectedProperty, Is.InstanceOf<string>());'
		And I expect it to contain the statement 'Assert.Fail("Create or modify test");'
    And I expect no method with a name like '.*SomeOtherProperty.*'