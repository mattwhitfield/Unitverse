Feature: AbstractClassGeneration
	I am checking the Abstract Class Generation strategy

Scenario: Abstract Class Generation
	Given I have a class defined as 
		"""
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
	And I set my mock framework to 'NSubstitute'
	When I generate tests for the class using strategy 'AbstractClassGenerationStrategy'
	Then I expect the method 'SetUp'
		And I expect it to have the attribute 'SetUp'
		And I expect it to contain an assignment for '_intProperty'
		And I expect it to contain an assignment for '_value'
		And I expect it to contain an assignment for '_targetStream'
		And I expect it to contain an assignment for '_testClass'
	And I expect the class 'TestTestClass'
		And I expect the class to have the modifier 'private'
		And I expect it to have the property 'PublicTargetStream' of type 'Stream'
		And I expect it to have the property 'PublicAbstractValue2' of type 'string'
		And I expect it to have the property 'PublicIntGetOnlyProperty' of type 'int'
		And I expect the method 'PublicSomeMethodVoid'
		And I expect the method 'SomeMethodVoid3'
			And I expect it to have the modifier 'protected'
			And I expect it to have the modifier 'internal'
		And I expect the method 'SomeMethodVoid4'
			And I expect it to have the modifier 'protected'
			And I expect it to have the modifier 'private'

Scenario: Abstract Class Generation 2
	Given I have a class defined as 
		"""
using System.IO;
using System.Windows;

namespace TestNamespace {

	public abstract class TestClass3 : TestClass2
	{
		protected TestClass3()
		{

		}
 
		public abstract int SomeMethodShould2();

		public override SomeMethodShouldNot2() {return 1; }
		public override SomeMethodShouldNot3() {return 1; }
	}

	public abstract class TestClass1
	{
		protected TestClass1()
		{

		}
 
		public abstract int SomeMethodShouldNot();

		public abstract int SomeMethodMaybe1(int i);

		public abstract int SomeMethodShouldNot4(int i, int j);

		public abstract int SomeMethodMaybe2<T>(int i);

		public abstract int SomeMethodMaybe3<T>(int i, int j);

		public abstract SomeMethodShouldNot2()

	}

	public abstract class TestClass2 : TestClass1
	{
		protected TestClass2()
		{

		}
 
		public override int SomeMethodShouldNot() { return 1; }

		public override int SomeMethodShouldNot4(int i, int j) { return 1; }

		public abstract int SomeMethodShould();

		public abstract SomeMethodShouldNot3()
	}
}
        """
	And I set my test framework to 'MS Test'
	And I set my mock framework to 'FakeItEasy'
	When I generate tests for the class using strategy 'AbstractClassGenerationStrategy'
	Then I expect the method 'SetUp'
		And I expect it to have the attribute 'TestInitialize'
		And I expect it to contain the statement '_testClass = new TestTestClass3();'
	And I expect the class 'TestTestClass3'
		And I expect the class to have the modifier 'private'
		And I expect the method 'SomeMethodShould'
			And I expect it to contain the statement 'return default(int);'
		And I expect the method 'SomeMethodMaybe1'
			And I expect it to contain the parameter 'i'
			And I expect it to contain the statement 'return default(int);'
		And I expect the method 'SomeMethodMaybe2'
		And I expect the method 'SomeMethodMaybe3'
			And I expect it to contain the parameter 'j'
		And I expect the method 'SomeMethodShould2'
		And I expect no method with name like '.*ShouldNot.*'