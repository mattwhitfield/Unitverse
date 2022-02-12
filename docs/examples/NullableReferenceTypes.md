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
        var instance = new TestClass(_notNullable, _nullable);
        instance.Should().NotBeNull();
        instance = new TestClass(_testITest);
        instance.Should().NotBeNull();
        instance = new TestClass(_testITest, _someOtherThing);
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullTest()
    {
        FluentActions.Invoking(() => new TestClass(default(ITest), "TestValue1657007234")).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotConstructWithInvalidNotNullable(string value)
    {
        FluentActions.Invoking(() => new TestClass(value, "TestValue1412011072")).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotConstructWithInvalidNullable(string value)
    {
        FluentActions.Invoking(() => new TestClass("TestValue929393559", value)).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotConstructWithInvalidSomeOtherThing(string value)
    {
        FluentActions.Invoking(() => new TestClass(Substitute.For<ITest>(), value)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallGetFullName()
    {
        var first = "TestValue760389092";
        var middle = "TestValue2026928803";
        var last = "TestValue217468053";
        var result = _testClass.GetFullName(first, middle, last);
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallGetFullNameWithInvalidFirst(string value)
    {
        FluentActions.Invoking(() => _testClass.GetFullName(value, "TestValue1379662799", "TestValue61497087")).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallGetFullNameWithInvalidMiddle(string value)
    {
        FluentActions.Invoking(() => _testClass.GetFullName("TestValue532638534", value, "TestValue687431273")).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallGetFullNameWithInvalidLast(string value)
    {
        FluentActions.Invoking(() => _testClass.GetFullName("TestValue2125508764", "TestValue1464848243", value)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallSomeMethod()
    {
        var test = Substitute.For<ITest>();
        var someOtherThing = "TestValue1406361028";
        _testClass.SomeMethod(test, someOtherThing);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallSomeMethodWithNullTest()
    {
        FluentActions.Invoking(() => _testClass.SomeMethod(default(ITest), "TestValue607156385")).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallSomeMethodWithInvalidSomeOtherThing(string value)
    {
        FluentActions.Invoking(() => _testClass.SomeMethod(Substitute.For<ITest>(), value)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallMethodForWhichNoNullabilityTestShouldBeEmitted()
    {
        var test = Substitute.For<ITest>();
        _testClass.MethodForWhichNoNullabilityTestShouldBeEmitted(test);
        throw new NotImplementedException("Create or modify test");
    }
}

```
