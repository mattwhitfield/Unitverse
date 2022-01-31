## AsyncMethod
Demonstrates how tests are generated for async methods, as well as showing how the assertion framework is driven differently for async methods

### Source Type(s)
``` csharp
public class TestClass
{
    public System.Threading.Tasks.Task ThisIsAnAsyncMethod(string methodName, int methodValue)
    {
        System.Console.WriteLine("Testing this");
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public System.Threading.Tasks.Task<int> ThisIsAnAsyncMethodWithReturnType(string methodName, int methodValue)
    {
        System.Console.WriteLine("Testing this");
        return System.Threading.Tasks.Task.FromResult(0);
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
    public async Task CanCallThisIsAnAsyncMethod()
    {
        var methodName = "TestValue106791138";
        var methodValue = 1017938334;
        await _testClass.ThisIsAnAsyncMethod(methodName, methodValue);
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CannotCallThisIsAnAsyncMethodWithInvalidMethodName(string value)
    {
        await FluentActions.Invoking(() => _testClass.ThisIsAnAsyncMethod(value, 107939520)).Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CanCallThisIsAnAsyncMethodWithReturnType()
    {
        var methodName = "TestValue298751031";
        var methodValue = 471194639;
        var result = await _testClass.ThisIsAnAsyncMethodWithReturnType(methodName, methodValue);
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CannotCallThisIsAnAsyncMethodWithReturnTypeWithInvalidMethodName(string value)
    {
        await FluentActions.Invoking(() => _testClass.ThisIsAnAsyncMethodWithReturnType(value, 1900855920)).Should().ThrowAsync<ArgumentNullException>();
    }
}

```
