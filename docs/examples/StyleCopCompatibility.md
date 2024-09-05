# StyleCop Compatbility
Demonstrates how tests can be generated with XML documentation and `this.` prefixed to test class fields

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
/// <summary>
/// Unit tests for the type <see cref="TestClass"/>.
/// </summary>
public class TestClassTests
{
    private readonly TestClass _testClass;
    private readonly IDependency _dependency;

    /// <summary>
    /// Sets up the dependencies required for the tests for <see cref="TestClass"/>.
    /// </summary>
    public TestClassTests()
    {
        this._dependency = Substitute.For<IDependency>();
        this._testClass = new TestClass(this._dependency);
    }

    /// <summary>
    /// Checks that instance construction works.
    /// </summary>
    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new TestClass(this._dependency);

        // Assert
        instance.Should().NotBeNull();
    }

    /// <summary>
    /// Checks that instance construction throws when the dependency parameter is null.
    /// </summary>
    [Fact]
    public void CannotConstructWithNullDependency()
    {
        FluentActions.Invoking(() => new TestClass(default(IDependency))).Should().Throw<ArgumentNullException>().WithParameterName("dependency");
    }

    /// <summary>
    /// Checks that the SomeMethod method functions correctly.
    /// </summary>
    [Fact]
    public void CanCallSomeMethod()
    {
        // Arrange
        var methodName = "TestValue534011718";
        var methodValue = 237820880;

        // Act
        this._testClass.SomeMethod(methodName, methodValue);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    /// <summary>
    /// Checks that the SomeMethod method throws when the methodName parameter is null, empty or white space.
    /// </summary>
    /// <param name="value">The parameter that receives the test case values.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallSomeMethodWithInvalidMethodName(string value)
    {
        FluentActions.Invoking(() => this._testClass.SomeMethod(value, 1002897798)).Should().Throw<ArgumentNullException>().WithParameterName("methodName");
    }

    /// <summary>
    /// Checks that the SomeAsyncMethod method functions correctly.
    /// </summary>
    /// <returns>A task that represents the running test.</returns>
    [Fact]
    public async Task CanCallSomeAsyncMethod()
    {
        // Arrange
        var methodName = "TestValue1657007234";
        var methodValue = 1412011072;

        // Act
        var result = await this._testClass.SomeAsyncMethod(methodName, methodValue);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    /// <summary>
    /// Checks that the SomeAsyncMethod method throws when the methodName parameter is null, empty or white space.
    /// </summary>
    /// <param name="value">The parameter that receives the test case values.</param>
    /// <returns>A task that represents the running test.</returns>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CannotCallSomeAsyncMethodWithInvalidMethodName(string value)
    {
        await FluentActions.Invoking(() => this._testClass.SomeAsyncMethod(value, 929393559)).Should().ThrowAsync<ArgumentNullException>().WithParameterName("methodName");
    }
}

```
