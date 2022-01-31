## StaticClass
Demonstrates how Unitverse generates tests when the source class is static

### Source Type(s)
``` csharp
public static class TestClass
{
    public static void ThisIsAMethod(string methodName, CultureInfo methodValue)
    {
        System.Console.WriteLine("Testing this");
    }

    public static string WillReturnAString()
    {
        return "Hello";
    }

    public static int ThisIsAProperty { get; set; }

    public static string GetITest { get; }
}

```

### Generated Tests
``` csharp
public static class TestClassTests
{
    [Fact]
    public static void CanCallThisIsAMethod()
    {
        var methodName = "TestValue534011718";
        var methodValue = CultureInfo.InvariantCulture;
        TestClass.ThisIsAMethod(methodName, methodValue);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethodWithNullMethodValue()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod("TestValue1002897798", default(CultureInfo))).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public static void CannotCallThisIsAMethodWithInvalidMethodName(string value)
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod(value, CultureInfo.InvariantCulture)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallWillReturnAString()
    {
        var result = TestClass.WillReturnAString();
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CanSetAndGetThisIsAProperty()
    {
        var testValue = 1412011072;
        TestClass.ThisIsAProperty = testValue;
        TestClass.ThisIsAProperty.Should().Be(testValue);
    }

    [Fact]
    public static void CanGetGetITest()
    {
        TestClass.GetITest.Should().BeAssignableTo<string>();
        throw new NotImplementedException("Create or modify test");
    }
}

```
