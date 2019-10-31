Feature: NullParameterCheckOperatorGeneration
	I am checking the Null Parameter Check Operator Generation strategy


Scenario: Null Parameter Check Operator Generation
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
	When I generate tests for the operator using the strategy 'NullParameterCheckOperatorGenerationStrategy'
	Then I expect methods with statements like:
	| methodName                                    | methodStatement                                                                                                               |
	| CannotCallPlusOperatorWithNullCalc1           | Assert.ThrowsException<ArgumentNullException>(() => { var result = default(Calculator) + new Calculator({{{AnyInteger}}});}); |
	| CannotCallPlusOperatorWithNullCalc2           | Assert.ThrowsException<ArgumentNullException>(() => { var result = new Calculator({{{AnyInteger}}}) + default(Calculator);}); |
	| CannotCallMinusOperatorWithNullCalc1          | Assert.ThrowsException<ArgumentNullException>(() => { var result = default(Calculator) - new Calculator({{{AnyInteger}}});}); |
	| CannotCallMinusOperatorWithNullCalc2          | Assert.ThrowsException<ArgumentNullException>(() => { var result = new Calculator({{{AnyInteger}}}) - default(Calculator);}); |
	| CannotCallUnaryMinusOperatorWithNullCalc1     | Assert.ThrowsException<ArgumentNullException>(() => { var result = -default(Calculator);});                                   |
	| CannotCallMultiplicationOperatorWithNullCalc1 | Assert.ThrowsException<ArgumentNullException>(() => { var result = default(Calculator) * new Calculator({{{AnyInteger}}});}); |
	| CannotCallMultiplicationOperatorWithNullCalc2 | Assert.ThrowsException<ArgumentNullException>(() => { var result = new Calculator({{{AnyInteger}}}) * default(Calculator);}); |
	| CannotCallDivisionOperatorWithNullCalc1       | Assert.ThrowsException<ArgumentNullException>(() => { var result = default(Calculator) / new Calculator({{{AnyInteger}}});}); |
	| CannotCallDivisionOperatorWithNullCalc2       | Assert.ThrowsException<ArgumentNullException>(() => { var result = new Calculator({{{AnyInteger}}}) / default(Calculator);}); |
	And I expect no method with a name like '.*UnaryMinusOperatorWithNullCalc2.*'