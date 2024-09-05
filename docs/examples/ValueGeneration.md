# Value Generation
Demonstrates how random values are created when required

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
        var c = 'C';

        // Act
        _testClass.Write(c);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithB()
    {
        // Arrange
        var b = (byte)119;

        // Act
        _testClass.Write(b);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithShort()
    {
        // Arrange
        var s = (short)25283;

        // Act
        _testClass.Write(s);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithI()
    {
        // Arrange
        var i = 1412011072;

        // Act
        _testClass.Write(i);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithL()
    {
        // Arrange
        var l = 929393559L;

        // Act
        _testClass.Write(l);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithDouble()
    {
        // Arrange
        var d = 752785201.08;

        // Act
        _testClass.Write(d);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithDecimal()
    {
        // Arrange
        var d = 2006659514.97M;

        // Act
        _testClass.Write(d);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithDateTime()
    {
        // Arrange
        var d = DateTime.UtcNow;

        // Act
        _testClass.Write(d);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithString()
    {
        // Arrange
        var s = "TestValue217468053";

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
        var t = TimeSpan.FromSeconds(321);

        // Act
        _testClass.Write(t);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallWriteWithG()
    {
        // Arrange
        var g = new Guid("9c6946ff-c473-cda1-1034-135b4ea36f84");

        // Act
        _testClass.Write(g);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }
}

```
