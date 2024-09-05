# Record Types (Primary Constructor)
Demonstrates the tests generated for a simple primary constructor record type

### Source Type(s)
``` csharp
public record RecordType(string StringProperty, int IntProperty);

```

### Generated Tests
``` csharp
public class RecordTypeTests
{
    private readonly RecordType _testClass;
    private string _stringProperty;
    private int _intProperty;

    public RecordTypeTests()
    {
        _stringProperty = "TestValue534011718";
        _intProperty = 237820880;
        _testClass = new RecordType(_stringProperty, _intProperty);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new RecordType(_stringProperty, _intProperty);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void ImplementsIEquatable_RecordType()
    {
        // Arrange
        var same = new RecordType(_stringProperty, _intProperty);
        var different = new RecordType("TestValue1002897798", 1657007234);

        // Assert
        _testClass.Equals(default(object)).Should().BeFalse();
        _testClass.Equals(new object()).Should().BeFalse();
        _testClass.Equals((object)same).Should().BeTrue();
        _testClass.Equals((object)different).Should().BeFalse();
        _testClass.Equals(same).Should().BeTrue();
        _testClass.Equals(different).Should().BeFalse();
        _testClass.GetHashCode().Should().Be(same.GetHashCode());
        _testClass.GetHashCode().Should().NotBe(different.GetHashCode());
        (_testClass == same).Should().BeTrue();
        (_testClass == different).Should().BeFalse();
        (_testClass != same).Should().BeFalse();
        (_testClass != different).Should().BeTrue();
    }

    [Fact]
    public void StringPropertyIsInitializedCorrectly()
    {
        _testClass.StringProperty.Should().Be(_stringProperty);
    }

    [Fact]
    public void IntPropertyIsInitializedCorrectly()
    {
        _testClass.IntProperty.Should().Be(_intProperty);
    }
}

```
