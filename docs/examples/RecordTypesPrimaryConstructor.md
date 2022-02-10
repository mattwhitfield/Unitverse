## Record Types (Primary Constructor)
Demonstrates the tests generated for a simple primary constructor record type

### Source Type(s)
``` csharp
public record RecordType(string StringProperty, int IntProperty);

```

### Generated Tests
``` csharp
public class RecordTypeTests
{
    private RecordType _testClass;
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
        var instance = new RecordType(_stringProperty, _intProperty);
        instance.Should().NotBeNull();
    }

    [Fact]
    public void StringPropertyIsInitializedCorrectly()
    {
        _testClass.StringProperty.Should().BeSameAs(_stringProperty);
    }

    [Fact]
    public void IntPropertyIsInitializedCorrectly()
    {
        _testClass.IntProperty.Should().Be(_intProperty);
    }
}

```
