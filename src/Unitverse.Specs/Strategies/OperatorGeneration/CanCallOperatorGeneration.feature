Feature: CanCallOperatorGeneration
	I am checking the Can Call Operator Generation strategy


Scenario: Can Call Operator Generation
	Given I have a class defined as 
	"""
namespace TestNamespace.SubNameSpace
{
	using System;

	class Calculator { 
      
		public int number = 0; 
      
		// parameterized constructor 
		public Calculator(int n) 
		{ 
			number = n; 
		} 
      
		public static Calculator operator + (Calculator Calc1,  
											 Calculator Calc2) 
		{ 
			Calculator Calc3 = new Calculator(0); 
			Calc3.number = Calc2.number + Calc1.number; 
			return Calc3; 
		} 

		public static Calculator operator - (Calculator Calc1,  
											 Calculator Calc2) 
		{ 
			Calculator Calc3 = new Calculator(0); 
			Calc3.number = Calc2.number - Calc1.number; 
			return Calc3; 
		} 

		public static Calculator operator - (Calculator Calc1) 
		{ 
			Calculator Calc3 = new Calculator(0); 
			Calc3.number = Calc1.number * -1; 
			return Calc3; 
		} 

		public static Calculator operator * (Calculator Calc1,  
											 Calculator Calc2) 
		{ 
			Calculator Calc3 = new Calculator(0); 
			Calc3.number = Calc2.number * Calc1.number; 
			return Calc3; 
		} 

		public static Calculator operator / (Calculator Calc1,  
											 Calculator Calc2) 
		{ 
			Calculator Calc3 = new Calculator(0); 
			Calc3.number = Calc2.number / Calc1.number; 
			return Calc3; 
		} 

	} 
}
	"""
	And I set my test framework to 'MS Test'
	And I set my mock framework to 'Moq'
	When I generate tests for the operator using the strategy 'CanCallOperatorGenerationStrategy'
	Then I expect a method called 'CanCallPlusOperator'
		And I expect it to contain the variable 'result'
		And I expect it to contain the variable 'Calc1'
		And I expect it to contain the statement 'Assert.Fail("Create or modify test");'
	And I expect a method called 'CanCallMinusOperator'
		And I expect it to have the attribute 'TestMethod'
		And I expect it to contain a statement like 'var Calc2 = new Calculator({{{AnyInteger}}});'
	And I expect a method called 'CanCallUnaryMinusOperator'
		And I expect it to contain the statement 'var result = -Calc1;'
	And I expect a method called 'CanCallMultiplicationOperator'
		And I expect it to contain the statement 'var result = Calc1 * Calc2;'
	And I expect a method called 'CanCallDivisionOperator'
		And I expect it to contain the variable 'Calc2'