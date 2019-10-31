Feature: NullParameterCheckMethodGeneration
	I am checking the Null Parameter Check Method Generation strategy


Scenario: Null Parameter Check Method Generation
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

    public static IList<string> Arguments(IEnumerable<string> expressions, IEnumerable<string> expressions2)
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
	And I set my test framework to 'XUnit'
	And I set my mock framework to 'Moq'
	When I generate tests for the method using the strategy 'NullParameterCheckMethodGenerationStrategy'
	Then I expect a method called 'CannotCallArgumentsWithExpressionsWithNullExpressions'
		And I expect it to contain 1 statements called 'Assert.Throws<ArgumentNullException>(() => Generate.Arguments(default(string[])));'
		And I expect it to have the attribute 'Fact'
	And I expect a method called 'CannotCallArgumentsWithExpressionsAndExpressions2WithNullExpressions'
		And I expect it to contain a statement like 'Assert.Throws<ArgumentNullException>(()=>Generate.Arguments(default(IEnumerable<string>),new[]{{{{AnyString}}}, {{{AnyString}}}, {{{AnyString}}}}));'
	And I expect a method called 'CannotCallArgumentsWithExpressionsAndExpressions2WithNullExpressions2'
		And I expect it to contain a statement like 'Assert.Throws<ArgumentNullException>(()=>Generate.Arguments(new[]{{{{AnyString}}}, {{{AnyString}}}, {{{AnyString}}}}, default(IEnumerable<string>)));'
	And I expect a method called 'CannotCallTest2WithNullTesting'
		And I expect it to contain the statement ' Assert.Throws<ArgumentNullException>(()=>Generate.Test2(reftesting));'
		And I expect it to contain the variable 'testing'
	And I expect no method with a name like '.*Test1.*'