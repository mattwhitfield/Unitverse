Feature: ComparableGeneration
	I am checking the Comparable Generation strategy

Scenario: Comparable Generation
	Given I have a class defined as 
		"""
using System;

namespace AssemblyCore
{
	public class TestComparableGeneric : IComparable<TestComparableGeneric>, IComparable<int>, IComparable
	{
		public TestComparableGeneric(int value)
		{
			Value = value;
		}

		public int Value { get; }

		public int CompareTo(TestComparableGeneric obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException();
			}

			return Value.CompareTo(obj.Value);
		}

		public int CompareTo(int value)
		{
			return Value.CompareTo(value);
		}

		public int CompareTo(object obj)
		{
			if (obj is null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			return Value.CompareTo(obj);
		}
	}
}
        """
	And I set my test framework to 'NUnit2'
	And I set my mock framework to 'NSubstitute'
	When I generate unit tests for the class using strategy 'ComparableGenerationStrategy'
	Then I expect a method called 'ImplementsIComparable'
		And I expect it to contain the statement 'Assert.That(baseValue.CompareTo(equalToBaseValue), Is.EqualTo(0));'
		And I expect it to have the attribute 'Test'

	And I expect a method called 'ImplementsIComparable_Int32'
		And I expect it to contain the statement 'int greaterThanBaseValue = default(int);'

	And I expect a method called 'ImplementsIComparable_TestComparableGeneric'
		And I expect it to contain the variable 'baseValue'