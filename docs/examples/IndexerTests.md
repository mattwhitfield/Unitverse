# Indexers
Demonstrates the tests generated for a type that contains an indexer

### Source Type(s)
``` csharp
public class TypeWithIndexer
{
    public string this[string cookieName]
    {
        get { return "hello"; }
    }

    public string this[string cookieName, int cookieId]
    {
        get { return "hello"; }
        set { }
    }
}

```

### Generated Tests
``` csharp
public class TypeWithIndexerTests
{
    private readonly TypeWithIndexer _testClass;

    public TypeWithIndexerTests()
    {
        _testClass = new TypeWithIndexer();
    }

    [Fact]
    public void CanGetIndexerForString()
    {
        _testClass["TestValue534011718"].Should().BeAssignableTo<string>();
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanSetAndGetIndexerForStringAndInt()
    {
        var testValue = "TestValue1657007234";
        _testClass["TestValue237820880", 1002897798].As<object>().Should().BeAssignableTo<string>();
        _testClass["TestValue237820880", 1002897798] = testValue;
        _testClass["TestValue237820880", 1002897798].Should().Be(testValue);
    }
}

```
