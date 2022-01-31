## SimplePoco
Demonstrates how tests are generated for a simple POCO type

### Source Type(s)
``` csharp
public class SomePoco
{
    public int Identity { get; set; }
    public string Description { get; set; }
    public Guid UniqueCode { get; set; }
}

```

### Generated Tests
``` csharp
public class SomePocoTests
{
    private SomePoco _testClass;

    public SomePocoTests()
    {
        _testClass = new SomePoco();
    }

    [Fact]
    public void CanSetAndGetIdentity()
    {
        var testValue = 534011718;
        _testClass.Identity = testValue;
        _testClass.Identity.Should().Be(testValue);
    }

    [Fact]
    public void CanSetAndGetDescription()
    {
        var testValue = "TestValue237820880";
        _testClass.Description = testValue;
        _testClass.Description.Should().BeSameAs(testValue);
    }

    [Fact]
    public void CanSetAndGetUniqueCode()
    {
        var testValue = new Guid("97408286-a3e4-cf95-ff46-699c73c4a1cd");
        _testClass.UniqueCode = testValue;
        _testClass.UniqueCode.Should().Be(testValue);
    }
}

```
