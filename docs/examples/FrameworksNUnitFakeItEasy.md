## Frameworks - NUnit 3 & FakeItEasy
Demonstrates how tests are generated using NUnit 3 for the test framework and FakeItEasy for the mocking framework

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
        var instance = new TestClass(_dependency);
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
        var methodName = "TestValue534011718";
        var methodValue = 237820880;
        _testClass.SomeMethod(methodName, methodValue);
        Assert.Fail("Create or modify test");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void CannotCallSomeMethodWithInvalidMethodName(string value)
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.SomeMethod(value, 1002897798));
    }

    [Test]
    public async Task CanCallSomeAsyncMethod()
    {
        var methodName = "TestValue1657007234";
        var methodValue = 1412011072;
        var result = await _testClass.SomeAsyncMethod(methodName, methodValue);
        Assert.Fail("Create or modify test");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void CannotCallSomeAsyncMethodWithInvalidMethodName(string value)
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SomeAsyncMethod(value, 929393559));
    }
}

```
