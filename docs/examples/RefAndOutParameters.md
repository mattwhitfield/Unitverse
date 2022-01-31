## RefAndOutParameters
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
        var stringProp = "TestValue649693393";
        var refParam = "TestValue1513152691";
        _testClass.RefParamMethodString(stringProp, ref refParam);
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallRefParamMethodStringWithInvalidStringProp(string value)
    {
        var refParam = "TestValue124253187";
        FluentActions.Invoking(() => _testClass.RefParamMethodString(value, ref refParam)).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallRefParamMethodStringWithInvalidRefParam(string value)
    {
        var refParam = default(string);
        FluentActions.Invoking(() => _testClass.RefParamMethodString("TestValue1615285520", ref refParam)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallOutParamMethodString()
    {
        var stringProp = "TestValue1319870701";
        _testClass.OutParamMethodString(stringProp, out var outParam);
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallOutParamMethodStringWithInvalidStringProp(string value)
    {
        FluentActions.Invoking(() => _testClass.OutParamMethodString(value, out _)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallRefParamMethodClass()
    {
        var stringProp = "TestValue431736428";
        var refParam = new TestClass();
        _testClass.RefParamMethodClass(stringProp, ref refParam);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallRefParamMethodClassWithNullRefParam()
    {
        var refParam = default(TestClass);
        FluentActions.Invoking(() => _testClass.RefParamMethodClass("TestValue1327345765", ref refParam)).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallRefParamMethodClassWithInvalidStringProp(string value)
    {
        var refParam = new TestClass();
        FluentActions.Invoking(() => _testClass.RefParamMethodClass(value, ref refParam)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallOutParamMethodClass()
    {
        var stringProp = "TestValue883647018";
        _testClass.OutParamMethodClass(stringProp, out var outParam);
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallOutParamMethodClassWithInvalidStringProp(string value)
    {
        FluentActions.Invoking(() => _testClass.OutParamMethodClass(value, out _)).Should().Throw<ArgumentNullException>();
    }
}

```
