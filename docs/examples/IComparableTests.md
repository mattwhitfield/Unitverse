## IComparable
Demonstrates the tests generated for a type that implements IComparable

### Source Type(s)
``` csharp
public class TestComparableGeneric : IComparable<TestComparableGeneric>, IComparable<int>, IComparable
{
    public TestComparableGeneric(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public int CompareTo(TestComparableGeneric obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException();
        }

        return Value.CompareTo(obj.Value);
    }

    public int CompareTo(int value)
    {
        return Value.CompareTo(value);
    }

    public int CompareTo(object obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return Value.CompareTo(obj);
    }
}

```

### Generated Tests
``` csharp
public class TestComparableGenericTests
{
    private TestComparableGeneric _testClass;
    private int _value;

    public TestComparableGenericTests()
    {
        _value = 534011718;
        _testClass = new TestComparableGeneric(_value);
    }

    [Fact]
    public void CanConstruct()
    {
        var instance = new TestComparableGeneric(_value);
        instance.Should().NotBeNull();
    }

    [Fact]
    public void ImplementsIComparable_TestComparableGeneric()
    {
        TestComparableGeneric baseValue = default(TestComparableGeneric);
        TestComparableGeneric equalToBaseValue = default(TestComparableGeneric);
        TestComparableGeneric greaterThanBaseValue = default(TestComparableGeneric);
        baseValue.CompareTo(equalToBaseValue).Should().Be(0);
        baseValue.CompareTo(greaterThanBaseValue).Should().BeLessThan(0);
        greaterThanBaseValue.CompareTo(baseValue).Should().BeGreaterThan(0);
    }

    [Fact]
    public void ImplementsIComparable_Int32()
    {
        TestComparableGeneric baseValue = default(TestComparableGeneric);
        int equalToBaseValue = default(int);
        int greaterThanBaseValue = default(int);
        baseValue.CompareTo(equalToBaseValue).Should().Be(0);
        baseValue.CompareTo(greaterThanBaseValue).Should().BeLessThan(0);
        greaterThanBaseValue.CompareTo(baseValue).Should().BeGreaterThan(0);
    }

    [Fact]
    public void ImplementsIComparable()
    {
        TestComparableGeneric baseValue = default(TestComparableGeneric);
        TestComparableGeneric equalToBaseValue = default(TestComparableGeneric);
        TestComparableGeneric greaterThanBaseValue = default(TestComparableGeneric);
        baseValue.CompareTo(equalToBaseValue).Should().Be(0);
        baseValue.CompareTo(greaterThanBaseValue).Should().BeLessThan(0);
        greaterThanBaseValue.CompareTo(baseValue).Should().BeGreaterThan(0);
    }

    [Fact]
    public void CanCallCompareToWithTestComparableGeneric()
    {
        var obj = new TestComparableGeneric(237820880);
        var result = _testClass.CompareTo(obj);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallCompareToWithTestComparableGenericWithNullObj()
    {
        FluentActions.Invoking(() => _testClass.CompareTo(default(TestComparableGeneric))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanCallCompareToWithValue()
    {
        var value = 1002897798;
        var result = _testClass.CompareTo(value);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallCompareToWithObject()
    {
        var obj = new object();
        var result = _testClass.CompareTo(obj);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallCompareToWithObjectWithNullObj()
    {
        FluentActions.Invoking(() => _testClass.CompareTo(default(object))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ValueIsInitializedCorrectly()
    {
        _testClass.Value.Should().Be(_value);
    }
}

```
