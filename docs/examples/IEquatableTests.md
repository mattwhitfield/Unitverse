# IEquatable
Demonstrates the tests generated for a type that implements IEquatable

### Source Type(s)
``` csharp
public struct Complex : IEquatable<Complex>
{
    public double RealPart { get; set; }
    public double ImaginaryPart { get; set; }

    public bool Equals(Complex other)
    {
        return (this.RealPart == other.RealPart)
            && (this.ImaginaryPart == other.ImaginaryPart);
    }

    public override bool Equals(object other)
    {
        if (other is Complex)
            return this.Equals((Complex)other);
        else
            return false;
    }

    public override int GetHashCode()
    {
        return this.RealPart.GetHashCode() ^ this.ImaginaryPart.GetHashCode();
    }

    public static bool operator ==(Complex term1, Complex term2)
    {
        return term1.Equals(term2);
    }

    public static bool operator !=(Complex term1, Complex term2)
    {
        return !term1.Equals(term2);
    }
}

```

### Generated Tests
``` csharp
public class ComplexTests
{
    private Complex _testClass;

    public ComplexTests()
    {
        _testClass = new Complex();
    }

    [Fact]
    public void ImplementsIEquatable_Complex()
    {
        // Arrange
        var same = new Complex();
        var different = new Complex();

        // Assert
        _testClass.Equals(default(object)).Should().BeFalse();
        _testClass.Equals(new object()).Should().BeFalse();
        _testClass.Equals((object)same).Should().BeTrue();
        _testClass.Equals((object)different).Should().BeFalse();
        _testClass.Equals(same).Should().BeTrue();
        _testClass.Equals(different).Should().BeFalse();
        _testClass.GetHashCode().Should().Be(same.GetHashCode());
        _testClass.GetHashCode().Should().NotBe(different.GetHashCode());
        (_testClass == same).Should().BeTrue();
        (_testClass == different).Should().BeFalse();
        (_testClass != same).Should().BeFalse();
        (_testClass != different).Should().BeTrue();
    }

    [Fact]
    public void CanCallEqualsWithComplex()
    {
        // Arrange
        var other = new Complex();

        // Act
        var result = _testClass.Equals(other);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallEqualsWithObject()
    {
        // Arrange
        var other = new object();

        // Act
        var result = _testClass.Equals(other);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallEqualsWithObjectWithNullOther()
    {
        FluentActions.Invoking(() => _testClass.Equals(default(object))).Should().Throw<ArgumentNullException>().WithParameterName("other");
    }

    [Fact]
    public void CanCallGetHashCode()
    {
        // Act
        var result = _testClass.GetHashCode();

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallEqualityOperator()
    {
        // Arrange
        var term1 = new Complex();
        var term2 = new Complex();

        // Act
        var result = term1 == term2;

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallInequalityOperator()
    {
        // Arrange
        var term1 = new Complex();
        var term2 = new Complex();

        // Act
        var result = term1 != term2;

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanSetAndGetRealPart()
    {
        // Arrange
        var testValue = 528671600.82;

        // Act
        _testClass.RealPart = testValue;

        // Assert
        _testClass.RealPart.Should().Be(testValue);
    }

    [Fact]
    public void CanSetAndGetImaginaryPart()
    {
        // Arrange
        var testValue = 235442671.2;

        // Act
        _testClass.ImaginaryPart = testValue;

        // Assert
        _testClass.ImaginaryPart.Should().Be(testValue);
    }
}

```
