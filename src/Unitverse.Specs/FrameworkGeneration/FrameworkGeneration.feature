Feature: Framework Generation
	I am checking ITestFramework for different frameworks

Scenario Outline: Framework Generation Test - AssertThrows
	Given I have a class defined as 
		"""
using System;
using System.Collections.Generic;
using System.Linq;

public static class Generate
{
	public static IList<string> Test1(out IEnumerable<string> testing)
	{
		testing = 1
	}

	public static IList<string> Test2(ref IEnumerable<string> testing)
	{
		testing = 1
	}

    public static IList<string> Arguments(params string[] expressions)
    {
        return ArgumentList(expressions);
    }

    public static IList<string> Arguments(IEnumerable<string> expressions)
    {
        return ArgumentList(expressions);
    }

    private static IList<string> ArgumentList(params string[] expressions)
    {
        return ArgumentList(expressions.AsEnumerable());
    }

    private static IList<string> ArgumentList(IEnumerable<string> expressions)
    {
        var tokens = new List<string>();
        foreach (var expression in expressions)
        {
            if (tokens.Count > 0)
            {
                tokens.Add(",");
            }

            tokens.Add(expression);
        }

        return tokens;
    }
}
	"""
	And I set my test framework to '<framework>'
	And I set my mock framework to 'Moq'
	When I generate tests for the method using the strategy 'NullParameterCheckMethodGenerationStrategy'
	Then I expect a method called 'CannotCallArgumentsWithArrayOfStringWithNullExpressions'
		And I expect it to contain the statement '<statement>'

	Examples:
	| framework | statement                                                                                   |
	| MsTest    | Assert.ThrowsException<ArgumentNullException>(() => Generate.Arguments(default(string[]))); |
	| NUnit3    | Assert.Throws<ArgumentNullException>(() => Generate.Arguments(default(string[])));          |
	| XUnit     | Assert.Throws<ArgumentNullException>(() => Generate.Arguments(default(string[])));          |

Scenario Outline: Framework Generation Test - AssertFail
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
	And I set my test framework to '<framework>'
	And I set my mock framework to 'Moq'
	When I generate tests for the method using the strategy 'CanCallMethodGenerationStrategy'
	Then I expect a method called 'CanCallWillReturnAString'
		And I expect it to contain the statement '<statement>'

	Examples:
	| framework | statement                                                   |
	| MsTest    | Assert.Fail("Create or modify test");                       |
	| NUnit3    | Assert.Fail("Create or modify test");                       |
	| XUnit     | throw new NotImplementedException("Create or modify test"); |

Scenario Outline: Framework Generation Test - AssertEqual
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
	And I set my test framework to '<framework>'
	And I set my mock framework to 'Moq'
	When I generate unit tests for the class using strategy 'ComparableGenerationStrategy'
	Then I expect a method called 'ImplementsIComparable'
		And I expect it to contain the statement '<statement>'

	Examples:
	| framework | statement                                                               |
	| MsTest    | Assert.AreEqual(0, baseValue.CompareTo(equalToBaseValue));              |
	| NUnit3    | Assert.That(baseValue.CompareTo(equalToBaseValue), Is.EqualTo(0));      |
	| XUnit     | Assert.Equal(0, baseValue.CompareTo(equalToBaseValue));                 |

Scenario Outline: Framework Generation Test - AssertLesser
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
	And I set my test framework to '<framework>'
	And I set my mock framework to 'Moq'
	When I generate unit tests for the class using strategy 'ComparableGenerationStrategy'
	Then I expect a method called 'ImplementsIComparable'
		And I expect it to contain the statement '<statement>'

	Examples:
	| framework | statement                                                               |
	| MsTest    | Assert.IsTrue(baseValue.CompareTo(greaterThanBaseValue) < 0);           |
	| NUnit3    | Assert.That(baseValue.CompareTo(greaterThanBaseValue), Is.LessThan(0)); |
	| XUnit     | Assert.True(baseValue.CompareTo(greaterThanBaseValue) < 0);             |

Scenario Outline: Framework Generation Test - AssertGreater
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
	And I set my test framework to '<framework>'
	And I set my mock framework to 'Moq'
	When I generate unit tests for the class using strategy 'ComparableGenerationStrategy'
	Then I expect a method called 'ImplementsIComparable'
		And I expect it to contain the statement '<statement>'

	Examples:
	| framework | statement                                                                  |
	| MsTest    | Assert.IsTrue(greaterThanBaseValue.CompareTo(baseValue) > 0);              |
	| NUnit3    | Assert.That(greaterThanBaseValue.CompareTo(baseValue), Is.GreaterThan(0)); |
	| XUnit     | Assert.True(greaterThanBaseValue.CompareTo(baseValue) > 0);                |

Scenario Outline: Framework Generation Test - AssertNotNull
	Given I have a class defined as 
		"""
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
	
namespace WindowsFormsApp1
{
    public class Form1 : IEnumerable<int>
    {
        public Form1()
        {
             
        }
		
		public IEnumerator<int> GetEnumerator()
        {
            return Enumerable.Empty<int>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
	"""
	And I set my test framework to '<framework>'
	And I set my mock framework to 'Moq'
	When I generate unit tests for the class using strategy 'CanConstructSingleConstructorGenerationStrategy'
	Then I expect a method called 'CanConstruct'
		And I expect it to contain the statement '<statement>'

	Examples:
	| framework | statement                           |
	| MsTest    | Assert.IsNotNull(instance);         |
	| NUnit3    | Assert.That(instance, Is.Not.Null); |
	| XUnit     | Assert.NotNull(instance);           |

Scenario Outline: Framework Generation Test - AssertIsInstanceOf
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
	And I set my test framework to '<framework>'
	And I set my mock framework to 'Moq'
	When I generate tests for the property using the strategy 'ReadOnlyPropertyGenerationStrategy'
	Then I expect a method called 'CanGetGetITest'
		And I expect it to contain the statement '<statement>'

	Examples:
	| framework | statement                                                    |
	| MsTest    | Assert.IsInstanceOfType(_testClass.GetITest, typeof(ITest)); |
	| NUnit3    | Assert.That(_testClass.GetITest, Is.InstanceOf<ITest>());    |
	| XUnit     | Assert.IsType<ITest>(_testClass.GetITest);                   |

Scenario Outline: Framework Generation Test - AssertThrowsAsync
	Given I have a class defined as 
		"""
namespace TestNamespace.SubNameSpace
{
    public class TestClass
    {
	    public System.Threading.Tasks.Task ThisIsAnAsyncMethod(string methodName, int methodValue)
	    {
		    System.Console.WriteLine("Testing this");
			return System.Threading.Tasks.Task.CompletedTask;
	    }

	    public System.Threading.Tasks.Task<int> ThisIsAnAsyncMethodWithReturnType(string methodName, int methodValue)
	    {
		    System.Console.WriteLine("Testing this");
			return System.Threading.Tasks.Task.FromResult(0);
	    }
    }
}
	"""
	And I set my test framework to '<framework>'
	And I set my mock framework to 'moq'
	When I generate tests for the method using the strategy 'StringParameterCheckMethodGenerationStrategy'
	Then I expect a method called 'CannotCallThisIsAnAsyncMethodWithInvalidMethodName'
		And I expect it to contain a statement like '<statement>'

	Examples:
	| framework | statement                                                                                                          |
	| MsTest    | Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _testClass.ThisIsAnAsyncMethod(value, {{{AnyInteger}}})); |
	| NUnit3    | Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ThisIsAnAsyncMethod(value, {{{AnyInteger}}}));          |
	| XUnit     | Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ThisIsAnAsyncMethod(value, {{{AnyInteger}}}));          |