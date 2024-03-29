﻿# Frameworks - MSTest & Moq (with Moq.AutoMock)
Demonstrates how tests are generated using MsTest for the test framework and Moq for the mocking framework, using Moq.AutoMock for object creation

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
[TestClass]
public class TestClassTests
{
    private TestClass _testClass;
    private Mock<IDependency> _dependency;

    [TestInitialize]
    public void SetUp()
    {
        var mocker = new AutoMocker();
        _dependency = mocker.GetMock<IDependency>();
        _testClass = mocker.CreateInstance<TestClass>();
    }

    [TestMethod]
    public void CanConstruct()
    {
        // Act
        var instance = new TestClass(_dependency.Object);

        // Assert
        Assert.IsNotNull(instance);
    }

    [TestMethod]
    public void CannotConstructWithNullDependency()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new TestClass(default(IDependency)));
    }

    [TestMethod]
    public void CanCallSomeMethod()
    {
        // Arrange
        var methodName = "TestValue237820880";
        var methodValue = 1002897798;

        _dependency.Setup(mock => mock.Method()).Returns(534011718);

        // Act
        _testClass.SomeMethod(methodName, methodValue);

        // Assert
        _dependency.Verify(mock => mock.Method());

        Assert.Fail("Create or modify test");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void CannotCallSomeMethodWithInvalidMethodName(string value)
    {
        Assert.ThrowsException<ArgumentNullException>(() => _testClass.SomeMethod(value, 1657007234));
    }

    [TestMethod]
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

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public async Task CannotCallSomeAsyncMethodWithInvalidMethodName(string value)
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _testClass.SomeAsyncMethod(value, 760389092));
    }
}

```
