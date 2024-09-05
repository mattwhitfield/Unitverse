# Async Methods
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
    private readonly TestClass _testClass;

    public TestClassTests()
    {
        _testClass = new TestClass();
    }

    [Fact]
    public async Task CanCallThisIsAnAsyncMethod()
    {
        // Arrange
        var methodName = "TestValue534011718";
        var methodValue = 237820880;

        // Act
        await _testClass.ThisIsAnAsyncMethod(methodName, methodValue);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CannotCallThisIsAnAsyncMethodWithInvalidMethodName(string value)
    {
        await FluentActions.Invoking(() => _testClass.ThisIsAnAsyncMethod(value, 1002897798)).Should().ThrowAsync<ArgumentNullException>().WithParameterName("methodName");
    }

    [Fact]
    public async Task CanCallThisIsAnAsyncMethodWithReturnType()
    {
        // Arrange
        var methodName = "TestValue1657007234";
        var methodValue = 1412011072;

        // Act
        var result = await _testClass.ThisIsAnAsyncMethodWithReturnType(methodName, methodValue);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CannotCallThisIsAnAsyncMethodWithReturnTypeWithInvalidMethodName(string value)
    {
        await FluentActions.Invoking(() => _testClass.ThisIsAnAsyncMethodWithReturnType(value, 929393559)).Should().ThrowAsync<ArgumentNullException>().WithParameterName("methodName");
    }
}

```
