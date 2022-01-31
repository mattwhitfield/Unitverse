## OperatorOverloading
Shows how Unitverse emits tests for declared unary and binary operators

### Source Type(s)
``` csharp
class Calculator
{
    public int number = 0;

    public Calculator(int n)
    {
        number = n;
    }

    public static Calculator operator +(Calculator Calc1, Calculator Calc2)
    {
        Calculator Calc3 = new Calculator(0);
        Calc3.number = Calc2.number + Calc1.number;
        return Calc3;
    }

    public static Calculator operator -(Calculator Calc1, Calculator Calc2)
    {
        Calculator Calc3 = new Calculator(0);
        Calc3.number = Calc2.number - Calc1.number;
        return Calc3;
    }

    public static Calculator operator -(Calculator Calc1)
    {
        Calculator Calc3 = new Calculator(0);
        Calc3.number = Calc1.number * -1;
        return Calc3;
    }

    public static Calculator operator *(Calculator Calc1, Calculator Calc2)
    {
        Calculator Calc3 = new Calculator(0);
        Calc3.number = Calc2.number * Calc1.number;
        return Calc3;
    }

    public static Calculator operator /(Calculator Calc1, Calculator Calc2)
    {
        Calculator Calc3 = new Calculator(0);
        Calc3.number = Calc2.number / Calc1.number;
        return Calc3;
    }
}

```

### Generated Tests
``` csharp
public class CalculatorTests
{
    private Calculator _testClass;
    private int _n;

    public CalculatorTests()
    {
        _n = 1662458408;
        _testClass = new Calculator(_n);
    }

    [Fact]
    public void CanConstruct()
    {
        var instance = new Calculator(_n);
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CanCallPlusOperator()
    {
        var Calc1 = new Calculator(72883637);
        var Calc2 = new Calculator(1721488486);
        var result = Calc1 + Calc2;
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallPlusOperatorWithNullCalc1()
    {
        FluentActions.Invoking(() => { var result = default(Calculator) + new Calculator(809386036); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CannotCallPlusOperatorWithNullCalc2()
    {
        FluentActions.Invoking(() => { var result = new Calculator(887471499) + default(Calculator); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallMinusOperator()
    {
        var Calc1 = new Calculator(924537961);
        var Calc2 = new Calculator(739146100);
        var result = Calc1 - Calc2;
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallMinusOperatorWithNullCalc1()
    {
        FluentActions.Invoking(() => { var result = default(Calculator) - new Calculator(1818006218); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CannotCallMinusOperatorWithNullCalc2()
    {
        FluentActions.Invoking(() => { var result = new Calculator(972655142) - default(Calculator); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallUnaryMinusOperator()
    {
        var Calc1 = new Calculator(421780883);
        var result = -Calc1;
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallUnaryMinusOperatorWithNullCalc1()
    {
        FluentActions.Invoking(() => { var result = -default(Calculator); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallMultiplicationOperator()
    {
        var Calc1 = new Calculator(19213251);
        var Calc2 = new Calculator(1295958250);
        var result = Calc1 * Calc2;
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallMultiplicationOperatorWithNullCalc1()
    {
        FluentActions.Invoking(() => { var result = default(Calculator) * new Calculator(1322527255); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CannotCallMultiplicationOperatorWithNullCalc2()
    {
        FluentActions.Invoking(() => { var result = new Calculator(2004768536) * default(Calculator); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallDivisionOperator()
    {
        var Calc1 = new Calculator(1480344831);
        var Calc2 = new Calculator(1430192166);
        var result = Calc1 / Calc2;
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallDivisionOperatorWithNullCalc1()
    {
        FluentActions.Invoking(() => { var result = default(Calculator) / new Calculator(1436059913); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CannotCallDivisionOperatorWithNullCalc2()
    {
        FluentActions.Invoking(() => { var result = new Calculator(1459832762) / default(Calculator); }).Should().Throw<ArgumentNullException>();
    }
}

```
