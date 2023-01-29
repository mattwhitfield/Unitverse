# ref & out Parameters
Demonstrates the tests that Unitverse emits when methods contain `ref` or `out` parameters

### Source Type(s)
``` csharp
public class TestClass
{
    public void RefParamMethodString(string stringProp, ref string refParam)
    {

    }

    public void OutParamMethodString(string stringProp, out string outParam)
    {
        outParam = "";
    }

    public void RefParamMethodClass(string stringProp, ref TestClass refParam)
    {

    }

    public void OutParamMethodClass(string stringProp, out TestClass outParam)
    {
        outParam = null;
    }
}

```

### Generated Tests
``` csharp
public class TestClassTests
{
    private TestClass _testClass;

    public TestClassTests()
    {
        _testClass = new TestClass();
    }

    [Fact]
    public void CanCallRefParamMethodString()
    {
        // Arrange
        var stringProp = "TestValue534011718";
        var refParam = "TestValue237820880";

        // Act
        _testClass.RefParamMethodString(stringProp, ref refParam);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallRefParamMethodStringWithInvalidStringProp(string value)
    {
        var refParam = "TestValue1002897798";
        FluentActions.Invoking(() => _testClass.RefParamMethodString(value, ref refParam)).Should().Throw<ArgumentNullException>().WithParameterName("stringProp");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallRefParamMethodStringWithInvalidRefParam(string value)
    {
        var refParam = default(string);
        FluentActions.Invoking(() => _testClass.RefParamMethodString("TestValue1657007234", ref refParam)).Should().Throw<ArgumentNullException>().WithParameterName("refParam");
    }

    [Fact]
    public void CanCallOutParamMethodString()
    {
        // Arrange
        var stringProp = "TestValue929393559";

        // Act
        _testClass.OutParamMethodString(stringProp, out var outParam);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallOutParamMethodStringWithInvalidStringProp(string value)
    {
        FluentActions.Invoking(() => _testClass.OutParamMethodString(value, out _)).Should().Throw<ArgumentNullException>().WithParameterName("stringProp");
    }

    [Fact]
    public void CanCallRefParamMethodClass()
    {
        // Arrange
        var stringProp = "TestValue760389092";
        var refParam = new TestClass();

        // Act
        _testClass.RefParamMethodClass(stringProp, ref refParam);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallRefParamMethodClassWithNullRefParam()
    {
        var refParam = default(TestClass);
        FluentActions.Invoking(() => _testClass.RefParamMethodClass("TestValue2026928803", ref refParam)).Should().Throw<ArgumentNullException>().WithParameterName("refParam");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallRefParamMethodClassWithInvalidStringProp(string value)
    {
        var refParam = new TestClass();
        FluentActions.Invoking(() => _testClass.RefParamMethodClass(value, ref refParam)).Should().Throw<ArgumentNullException>().WithParameterName("stringProp");
    }

    [Fact]
    public void CanCallOutParamMethodClass()
    {
        // Arrange
        var stringProp = "TestValue217468053";

        // Act
        _testClass.OutParamMethodClass(stringProp, out var outParam);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallOutParamMethodClassWithInvalidStringProp(string value)
    {
        FluentActions.Invoking(() => _testClass.OutParamMethodClass(value, out _)).Should().Throw<ArgumentNullException>().WithParameterName("stringProp");
    }
}

```
