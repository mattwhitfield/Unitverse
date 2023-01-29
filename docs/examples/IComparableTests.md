# IComparable
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
        // Act
        var instance = new TestComparableGeneric(_value);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void ImplementsIComparable_TestComparableGeneric()
    {
        // Arrange
        TestComparableGeneric baseValue = default(TestComparableGeneric);
        TestComparableGeneric equalToBaseValue = default(TestComparableGeneric);
        TestComparableGeneric greaterThanBaseValue = default(TestComparableGeneric);

        // Assert
        baseValue.CompareTo(equalToBaseValue).Should().Be(0);
        baseValue.CompareTo(greaterThanBaseValue).Should().BeLessThan(0);
        greaterThanBaseValue.CompareTo(baseValue).Should().BeGreaterThan(0);
    }

    [Fact]
    public void ImplementsIComparable_Int32()
    {
        // Arrange
        TestComparableGeneric baseValue = default(TestComparableGeneric);
        int equalToBaseValue = default(int);
        int greaterThanBaseValue = default(int);

        // Assert
        baseValue.CompareTo(equalToBaseValue).Should().Be(0);
        baseValue.CompareTo(greaterThanBaseValue).Should().BeLessThan(0);
        greaterThanBaseValue.CompareTo(baseValue).Should().BeGreaterThan(0);
    }

    [Fact]
    public void ImplementsIComparable()
    {
        // Arrange
        TestComparableGeneric baseValue = default(TestComparableGeneric);
        TestComparableGeneric equalToBaseValue = default(TestComparableGeneric);
        TestComparableGeneric greaterThanBaseValue = default(TestComparableGeneric);

        // Assert
        baseValue.CompareTo(equalToBaseValue).Should().Be(0);
        baseValue.CompareTo(greaterThanBaseValue).Should().BeLessThan(0);
        greaterThanBaseValue.CompareTo(baseValue).Should().BeGreaterThan(0);
    }

    [Fact]
    public void CanCallCompareToWithTestComparableGeneric()
    {
        // Arrange
        var obj = new TestComparableGeneric(237820880);

        // Act
        var result = _testClass.CompareTo(obj);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallCompareToWithTestComparableGenericWithNullObj()
    {
        FluentActions.Invoking(() => _testClass.CompareTo(default(TestComparableGeneric))).Should().Throw<ArgumentNullException>().WithParameterName("obj");
    }

    [Fact]
    public void CanCallCompareToWithValue()
    {
        // Arrange
        var value = 1002897798;

        // Act
        var result = _testClass.CompareTo(value);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallCompareToWithObject()
    {
        // Arrange
        var obj = new object();

        // Act
        var result = _testClass.CompareTo(obj);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallCompareToWithObjectWithNullObj()
    {
        FluentActions.Invoking(() => _testClass.CompareTo(default(object))).Should().Throw<ArgumentNullException>().WithParameterName("obj");
    }

    [Fact]
    public void ValueIsInitializedCorrectly()
    {
        _testClass.Value.Should().Be(_value);
    }
}

```
