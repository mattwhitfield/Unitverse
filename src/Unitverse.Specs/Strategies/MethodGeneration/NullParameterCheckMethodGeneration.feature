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
	Then I expect methods with statements like:
	| methodName                                                            | methodStatement                                                                                                                                       |
	| CannotCallArgumentsWithExpressionsWithNullExpressions                 | Assert.Throws<ArgumentNullException>(() => Generate.Arguments(default(string[])));                                                                    |
	| CannotCallArgumentsWithExpressionsAndExpressions2WithNullExpressions  | Assert.Throws<ArgumentNullException>(()=>Generate.Arguments(default(IEnumerable<string>),new[]{{{{AnyString}}}, {{{AnyString}}}, {{{AnyString}}}}));  |
	| CannotCallArgumentsWithExpressionsAndExpressions2WithNullExpressions2 | Assert.Throws<ArgumentNullException>(()=>Generate.Arguments(new[]{{{{AnyString}}}, {{{AnyString}}}, {{{AnyString}}}}, default(IEnumerable<string>))); |
	| CannotCallTest2WithNullTesting                                        | Assert.Throws<ArgumentNullException>(()=>Generate.Test2(reftesting));                                                                                 |
	And I expect no method with a name like '.*Test1.*'