# Nullable Reference Types
Shows how Unitverse will omit `null` tests for parameters declared to explicitly accept null

### Source Type(s)
``` csharp
public interface ITest { }

public class TestClass
{
    public TestClass(string notNullable, string? nullable)
    {
    }

    public TestClass(ITest? test)
    {
    }

    public TestClass(ITest test, string someOtherThing)
    {
    }

    public string GetFullName(string first, string? middle, string last) => middle != null ? $"{first} {middle} {last}" : $"{first} {last}";

    public void SomeMethod(ITest test, string someOtherThing)
    { }

    public void MethodForWhichNoNullabilityTestShouldBeEmitted(ITest? test)
    { }
}

```

### Generated Tests
``` csharp
public class TestClassTests
{
    private TestClass _testClass;
    private string _notNullable;
    private string _nullable;
    private ITest _testITest;
    private string _someOtherThing;

    public TestClassTests()
    {
        _notNullable = "TestValue534011718";
        _nullable = "TestValue237820880";
        _testITest = Substitute.For<ITest>();
        _someOtherThing = "TestValue1002897798";
        _testClass = new TestClass(_notNullable, _nullable);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new TestClass(_notNullable, _nullable);

        // Assert
        instance.Should().NotBeNull();

        // Act
        instance = new TestClass(_testITest);

        // Assert
        instance.Should().NotBeNull();

        // Act
        instance = new TestClass(_testITest, _someOtherThing);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullTest()
    {
        FluentActions.Invoking(() => new TestClass(default(ITest), _someOtherThing)).Should().Throw<ArgumentNullException>().WithParameterName("test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotConstructWithInvalidNotNullable(string value)
    {
        FluentActions.Invoking(() => new TestClass(value, _nullable)).Should().Throw<ArgumentNullException>().WithParameterName("notNullable");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotConstructWithInvalidNullable(string value)
    {
        FluentActions.Invoking(() => new TestClass(_notNullable, value)).Should().Throw<ArgumentNullException>().WithParameterName("nullable");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotConstructWithInvalidSomeOtherThing(string value)
    {
        FluentActions.Invoking(() => new TestClass(_testITest, value)).Should().Throw<ArgumentNullException>().WithParameterName("someOtherThing");
    }

    [Fact]
    public void CanCallGetFullName()
    {
        // Arrange
        var first = "TestValue1657007234";
        var middle = "TestValue1412011072";
        var last = "TestValue929393559";

        // Act
        var result = _testClass.GetFullName(first, middle, last);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallGetFullNameWithInvalidFirst(string value)
    {
        FluentActions.Invoking(() => _testClass.GetFullName(value, "TestValue760389092", "TestValue2026928803")).Should().Throw<ArgumentNullException>().WithParameterName("first");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallGetFullNameWithInvalidMiddle(string value)
    {
        FluentActions.Invoking(() => _testClass.GetFullName("TestValue217468053", value, "TestValue1379662799")).Should().Throw<ArgumentNullException>().WithParameterName("middle");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallGetFullNameWithInvalidLast(string value)
    {
        FluentActions.Invoking(() => _testClass.GetFullName("TestValue61497087", "TestValue532638534", value)).Should().Throw<ArgumentNullException>().WithParameterName("last");
    }

    [Fact]
    public void CanCallSomeMethod()
    {
        // Arrange
        var test = Substitute.For<ITest>();
        var someOtherThing = "TestValue687431273";

        // Act
        _testClass.SomeMethod(test, someOtherThing);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallSomeMethodWithNullTest()
    {
        FluentActions.Invoking(() => _testClass.SomeMethod(default(ITest), "TestValue2125508764")).Should().Throw<ArgumentNullException>().WithParameterName("test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallSomeMethodWithInvalidSomeOtherThing(string value)
    {
        FluentActions.Invoking(() => _testClass.SomeMethod(Substitute.For<ITest>(), value)).Should().Throw<ArgumentNullException>().WithParameterName("someOtherThing");
    }

    [Fact]
    public void CanCallMethodForWhichNoNullabilityTestShouldBeEmitted()
    {
        // Arrange
        var test = Substitute.For<ITest>();

        // Act
        _testClass.MethodForWhichNoNullabilityTestShouldBeEmitted(test);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }
}

```
