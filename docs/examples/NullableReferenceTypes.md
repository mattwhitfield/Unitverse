## NullableReferenceTypes
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
        _notNullable = "TestValue1022818041";
        _nullable = "TestValue738347598";
        _testITest = Substitute.For<ITest>();
        _someOtherThing = "TestValue150271748";
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
        FluentActions.Invoking(() => new TestClass(default(ITest), "TestValue2081973690")).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotConstructWithInvalidNotNullable(string value)
    {
        FluentActions.Invoking(() => new TestClass(value, "TestValue461917229")).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotConstructWithInvalidNullable(string value)
    {
        FluentActions.Invoking(() => new TestClass("TestValue1654698443", value)).Should().Throw<ArgumentNullException>();
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
        var first = "TestValue159692643";
        var middle = "TestValue1521802455";
        var last = "TestValue800284774";
        var result = _testClass.GetFullName(first, middle, last);
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallGetFullNameWithInvalidFirst(string value)
    {
        FluentActions.Invoking(() => _testClass.GetFullName(value, "TestValue1271344227", "TestValue457055253")).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallGetFullNameWithInvalidMiddle(string value)
    {
        FluentActions.Invoking(() => _testClass.GetFullName("TestValue1800268088", value, "TestValue2137528102")).Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallGetFullNameWithInvalidLast(string value)
    {
        FluentActions.Invoking(() => _testClass.GetFullName("TestValue839351134", "TestValue412311232", value)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallSomeMethod()
    {
        var test = Substitute.For<ITest>();
        var someOtherThing = "TestValue920131662";
        _testClass.SomeMethod(test, someOtherThing);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallSomeMethodWithNullTest()
    {
        FluentActions.Invoking(() => _testClass.SomeMethod(default(ITest), "TestValue997824401")).Should().Throw<ArgumentNullException>();
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
