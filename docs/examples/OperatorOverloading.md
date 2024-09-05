# Operator Overloading
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
    private readonly Calculator _testClass;
    private int _n;

    public CalculatorTests()
    {
        _n = 534011718;
        _testClass = new Calculator(_n);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new Calculator(_n);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CanCallPlusOperator()
    {
        // Arrange
        var Calc1 = new Calculator(237820880);
        var Calc2 = new Calculator(1002897798);

        // Act
        var result = Calc1 + Calc2;

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallPlusOperatorWithNullCalc1()
    {
        FluentActions.Invoking(() => { var result = default(Calculator) + new Calculator(1657007234); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CannotCallPlusOperatorWithNullCalc2()
    {
        FluentActions.Invoking(() => { var result = new Calculator(1412011072) + default(Calculator); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallMinusOperator()
    {
        // Arrange
        var Calc1 = new Calculator(929393559);
        var Calc2 = new Calculator(760389092);

        // Act
        var result = Calc1 - Calc2;

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallMinusOperatorWithNullCalc1()
    {
        FluentActions.Invoking(() => { var result = default(Calculator) - new Calculator(2026928803); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CannotCallMinusOperatorWithNullCalc2()
    {
        FluentActions.Invoking(() => { var result = new Calculator(217468053) - default(Calculator); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallUnaryMinusOperator()
    {
        // Arrange
        var Calc1 = new Calculator(1379662799);

        // Act
        var result = -Calc1;

        // Assert
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
        // Arrange
        var Calc1 = new Calculator(61497087);
        var Calc2 = new Calculator(532638534);

        // Act
        var result = Calc1 * Calc2;

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallMultiplicationOperatorWithNullCalc1()
    {
        FluentActions.Invoking(() => { var result = default(Calculator) * new Calculator(687431273); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CannotCallMultiplicationOperatorWithNullCalc2()
    {
        FluentActions.Invoking(() => { var result = new Calculator(2125508764) * default(Calculator); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallDivisionOperator()
    {
        // Arrange
        var Calc1 = new Calculator(1464848243);
        var Calc2 = new Calculator(1406361028);

        // Act
        var result = Calc1 / Calc2;

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallDivisionOperatorWithNullCalc1()
    {
        FluentActions.Invoking(() => { var result = default(Calculator) / new Calculator(607156385); }).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CannotCallDivisionOperatorWithNullCalc2()
    {
        FluentActions.Invoking(() => { var result = new Calculator(1321446349) / default(Calculator); }).Should().Throw<ArgumentNullException>();
    }
}

```
