# Frameworks - NUnit 3 & FakeItEasy
Demonstrates how tests are generated using NUnit 3 for the test framework and FakeItEasy for the mocking framework

### Source Type(s)
``` csharp
public interface IDependency
{
    int Method();
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
        var x = _dependency.Method();
        System.Console.WriteLine("Testing this" + x);
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
[TestFixture]
public class TestClassTests
{
    private TestClass _testClass;
    private IDependency _dependency;

    [SetUp]
    public void SetUp()
    {
        _dependency = A.Fake<IDependency>();
        _testClass = new TestClass(_dependency);
    }

    [Test]
    public void CanConstruct()
    {
        // Act
        var instance = new TestClass(_dependency);

        // Assert
        Assert.That(instance, Is.Not.Null);
    }

    [Test]
    public void CannotConstructWithNullDependency()
    {
        Assert.Throws<ArgumentNullException>(() => new TestClass(default(IDependency)));
    }

    [Test]
    public void CanCallSomeMethod()
    {
        // Arrange
        var methodName = "TestValue237820880";
        var methodValue = 1002897798;

        A.CallTo(() => _dependency.Method()).Returns(534011718);

        // Act
        _testClass.SomeMethod(methodName, methodValue);

        // Assert
        A.CallTo(() => _dependency.Method()).MustHaveHappened();

        Assert.Fail("Create or modify test");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void CannotCallSomeMethodWithInvalidMethodName(string value)
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.SomeMethod(value, 1657007234));
    }

    [Test]
    public async Task CanCallSomeAsyncMethod()
    {
        // Arrange
        var methodName = "TestValue1412011072";
        var methodValue = 929393559;

        // Act
        var result = await _testClass.SomeAsyncMethod(methodName, methodValue);

        // Assert
        Assert.Fail("Create or modify test");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void CannotCallSomeAsyncMethodWithInvalidMethodName(string value)
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SomeAsyncMethod(value, 760389092));
    }
}

```
