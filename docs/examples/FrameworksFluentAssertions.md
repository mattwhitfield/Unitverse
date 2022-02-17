# Frameworks - Fluent Assertions
Demonstrates how tests are generated using XUnit for the test framework and NSubstitute for the mocking framework. Also shows using FluentAssertions for the assertion framework.

### Source Type(s)
``` csharp
public interface IDependency
{
    void Method();
}

public class TestClass
{
    public TestClass(IDependency dependency)
    { }

    public void SomeMethod(string methodName, int methodValue)
    {
        System.Console.WriteLine("Testing this");
        return System.Threading.Tasks.Task.CompletedTask;
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
        _dependency = Substitute.For<IDependency>();
        _testClass = new TestClass(_dependency);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new TestClass(_dependency);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullDependency()
    {
        FluentActions.Invoking(() => new TestClass(default(IDependency))).Should().Throw<ArgumentNullException>();
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
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallSomeMethodWithInvalidMethodName(string value)
    {
        FluentActions.Invoking(() => _testClass.SomeMethod(value, 1002897798)).Should().Throw<ArgumentNullException>();
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
        await FluentActions.Invoking(() => _testClass.SomeAsyncMethod(value, 929393559)).Should().ThrowAsync<ArgumentNullException>();
    }
}

```
