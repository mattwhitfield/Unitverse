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
        var testValue = 1381233515;
        _testClass.Identity = testValue;
        _testClass.Identity.Should().Be(testValue);
    }

    [Fact]
    public void CanSetAndGetDescription()
    {
        var testValue = "TestValue116732967";
        _testClass.Description = testValue;
        _testClass.Description.Should().BeSameAs(testValue);
    }

    [Fact]
    public void CanSetAndGetUniqueCode()
    {
        var testValue = new Guid("fd302cdf-4a85-4e26-8fa7-e6af33fe1d3f");
        _testClass.UniqueCode = testValue;
        _testClass.UniqueCode.Should().Be(testValue);
    }
}

```
