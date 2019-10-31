Feature: ReadWritePropertyGeneration
	I am checking the Read Write Property Generation strategy


Scenario: Read Write Property Generation
	Given I have a class defined as 
	"""
using System.IO;
using System.Windows;

namespace TestNamespace {


	public abstract class TestClass
	{
		protected TestClass(int intProperty, string value, Stream targetStream)
		{

		}
 
		public abstract int SomeMethod();

		public abstract void SomeVoidMethod();

		protected abstract void SomeProtectedAbstractMethod();

		public int IntProperty { get; set; }

		protected int IntGetOnlyProperty { get; private set; }

		protected string Value { get; set; }

		protected string Value2 { get; private set; }

		protected string Value3 { private get; set; }

		protected abstract string AbstractValue { get; set; }

		protected abstract string AbstractValue2 { get; }

		protected abstract string AbstractValue3 { set; }

		protected Stream TargetStream { get; }

		protected string SomeMethod(int i, int j) { return string.Empty; }

		protected abstract string SomeAbstractMethod(int i, int j);

		public abstract string SomePublicAbstractMethod(int p, int r);

		protected void SomeMethodVoid(int s, int t) {  }

		protected internal void SomeMethodVoid2(int s, int t) {  }

		protected internal abstract void SomeMethodVoid3(int s, int t);

		private protected abstract void SomeMethodVoid4(int s, int t);
	}
}
	"""
	And I set my test framework to 'NUnit3'
	And I set my mock framework to 'FakeItEasy'
	When I generate tests for the property using the strategy 'ReadWritePropertyGenerationStrategy'
	Then I expect a method called 'CanSetAndGetIntProperty'
		And I expect it to contain the statement '_testClass.IntProperty = testValue;'
		And I expect it to contain the statement 'Assert.That(_testClass.IntProperty, Is.EqualTo(testValue));'
		And I expect it to contain a statement like 'var testValue = {{{AnyInteger}}};'
	And I expect a method called 'CanSetAndGetAbstractValue'
		And I expect it to contain the variable 'testValue'
		And I expect it to contain the statement '_testClass.AbstractValue = testValue;'
		And I expect it to contain the statement 'Assert.That(_testClass.AbstractValue, Is.EqualTo(testValue));'
	And I expect a method called 'CanSetAndGetValue'
		And I expect it to have the attribute 'Test'
		And I expect it to contain a statement like 'var testValue = {{{AnyString}}};'
	And I expect no method with a name like '.*IntGetOnlyProperty.*'
	And I expect no method with a name like '.*AbstractValue2.*'
	And I expect no method with a name like '.*AbstractValue3.*'
	And I expect no method with a name like '.*Value2.*'
	And I expect no method with a name like '.*Value3.*'