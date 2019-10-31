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
	Then I expect a method called 'CannotCallPlusOperatorWithNullCalc1'
		And I expect it to have the attribute 'TestMethod'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => { var result = default(Calculator) + new Calculator({{{AnyInteger}}});});'
	And I expect a method called 'CannotCallPlusOperatorWithNullCalc2'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => { var result = new Calculator({{{AnyInteger}}}) + default(Calculator);});'
	And I expect a method called 'CannotCallMinusOperatorWithNullCalc1'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => { var result = default(Calculator) - new Calculator({{{AnyInteger}}});});'
	And I expect a method called 'CannotCallMinusOperatorWithNullCalc2'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => { var result = new Calculator({{{AnyInteger}}}) - default(Calculator);});'
	And I expect a method called 'CannotCallUnaryMinusOperatorWithNullCalc1'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => { var result = -default(Calculator);});'
	And I expect a method called 'CannotCallMultiplicationOperatorWithNullCalc1'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => { var result = default(Calculator) * new Calculator({{{AnyInteger}}});});'
	And I expect a method called 'CannotCallMultiplicationOperatorWithNullCalc2'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => { var result = new Calculator({{{AnyInteger}}}) * default(Calculator);});'
	And I expect a method called 'CannotCallDivisionOperatorWithNullCalc1'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => { var result = default(Calculator) / new Calculator({{{AnyInteger}}});});'
	And I expect a method called 'CannotCallDivisionOperatorWithNullCalc2'
		And I expect it to contain a statement like 'Assert.ThrowsException<ArgumentNullException>(() => { var result = new Calculator({{{AnyInteger}}}) / default(Calculator);});'
	And I expect no method with a name like '.*UnaryMinusOperatorWithNullCalc2.*'