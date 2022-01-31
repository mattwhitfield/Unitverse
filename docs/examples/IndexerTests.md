## IndexerTests
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
    private TypeWithIndexer _testClass;

    public TypeWithIndexerTests()
    {
        _testClass = new TypeWithIndexer();
    }

    [Fact]
    public void CanGetIndexerForString()
    {
        _testClass["TestValue402395593"].Should().BeAssignableTo<string>();
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanSetAndGetIndexerForStringAndInt()
    {
        var testValue = "TestValue854389529";
        _testClass["TestValue1006944135", 2111336371].Should().BeAssignableTo<string>();
        _testClass["TestValue1006944135", 2111336371] = testValue;
        _testClass["TestValue1006944135", 2111336371].Should().BeSameAs(testValue);
    }
}

```
