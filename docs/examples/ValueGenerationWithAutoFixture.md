# Value Generation (with AutoFixture)
Demonstrates how Unitverse can be configured to work with AutoFixture for test value generation

### Source Type(s)
``` csharp
public class Writer
{
    public void Write(char c);
    public void Write(byte b);
    public void Write(short s);
    public void Write(int i);
    public void Write(long l);
    public void Write(double d);
    public void Write(decimal d);
    public void Write(DateTime d);
    public void Write(string s);
    public void Write(TimeSpan t);
    public void Write(Guid g);
}

```

### Generated Tests
``` csharp
public class WriterTests
{
    private readonly Writer _testClass;

    public WriterTests()
    {
        _testClass = new Writer();
    }

    [Fact]
    public void CanCallWriteWithC()
    {
        // Arrange
        var fixture = new Fixture();
        var c = fixture.Create<char>();

        // Act
        _testClass.Write(c);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithB()
    {
        // Arrange
        var fixture = new Fixture();
        var b = fixture.Create<byte>();

        // Act
        _testClass.Write(b);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithShort()
    {
        // Arrange
        var fixture = new Fixture();
        var s = fixture.Create<short>();

        // Act
        _testClass.Write(s);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithI()
    {
        // Arrange
        var fixture = new Fixture();
        var i = fixture.Create<int>();

        // Act
        _testClass.Write(i);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithL()
    {
        // Arrange
        var fixture = new Fixture();
        var l = fixture.Create<long>();

        // Act
        _testClass.Write(l);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithDouble()
    {
        // Arrange
        var fixture = new Fixture();
        var d = fixture.Create<double>();

        // Act
        _testClass.Write(d);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithDecimal()
    {
        // Arrange
        var fixture = new Fixture();
        var d = fixture.Create<decimal>();

        // Act
        _testClass.Write(d);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithDateTime()
    {
        // Arrange
        var fixture = new Fixture();
        var d = fixture.Create<DateTime>();

        // Act
        _testClass.Write(d);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithString()
    {
        // Arrange
        var fixture = new Fixture();
        var s = fixture.Create<string>();

        // Act
        _testClass.Write(s);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallWriteWithStringWithInvalidS(string value)
    {
        FluentActions.Invoking(() => _testClass.Write(value)).Should().Throw<ArgumentNullException>().WithParameterName("s");
    }

    [Fact]
    public void CanCallWriteWithT()
    {
        // Arrange
        var fixture = new Fixture();
        var t = fixture.Create<TimeSpan>();

        // Act
        _testClass.Write(t);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithG()
    {
        // Arrange
        var fixture = new Fixture();
        var g = fixture.Create<Guid>();

        // Act
        _testClass.Write(g);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }
}

```
