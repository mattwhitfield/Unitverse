# Frameworks - XUnit & JustMock
Demonstrates how tests are generated using XUnit for the test framework and JustMock for the mocking framework

### Source Type(s)
``` csharp
public interface IDependency
{
    void Method();
}

public class TestClass
{
    IDependency _dependency;

    public TestClass(IDependency dependency)
    {
        _dependency = dependency;
    }

    public void SomeMethod(string methodName, int methodValue)
    {
        _dependency.Method();
        System.Console.WriteLine("Testing this");
    }

    public System.Threading.Tasks.Task<int> SomeAsyncMethod(string methodName, int methodValue)
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
    private IDependency _dependency;

    public TestClassTests()
    {
        _dependency = Mock.Create<IDependency>();
        _testClass = new TestClass(_dependency);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new TestClass(_dependency);

        // Assert
        Assert.NotNull(instance);
    }

    [Fact]
    public void CannotConstructWithNullDependency()
    {
        Assert.Throws<ArgumentNullException>(() => new TestClass(default(IDependency)));
    }

    [Fact]
    public void CanCallSomeMethod()
    {
        // Arrange
        var methodName = "TestValue534011718";
        var methodValue = 237820880;

        // Act
        _testClass.SomeMethod(methodName, methodValue);

        // Assert
        Mock.Assert(() => _dependency.Method());

        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallSomeMethodWithInvalidMethodName(string value)
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.SomeMethod(value, 1002897798));
    }

    [Fact]
    public async Task CanCallSomeAsyncMethod()
    {
        // Arrange
        var methodName = "TestValue1657007234";
        var methodValue = 1412011072;

        // Act
        var result = await _testClass.SomeAsyncMethod(methodName, methodValue);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CannotCallSomeAsyncMethodWithInvalidMethodName(string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SomeAsyncMethod(value, 929393559));
    }
}

```
