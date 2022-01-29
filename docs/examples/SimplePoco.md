## SimplePoco
Description

### Source Type
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
        var testValue = 601128017;
        _testClass.Identity = testValue;
        _testClass.Identity.Should().Be(testValue);
    }

    [Fact]
    public void CanSetAndGetDescription()
    {
        var testValue = "TestValue404443222";
        _testClass.Description = testValue;
        _testClass.Description.Should().BeSameAs(testValue);
    }

    [Fact]
    public void CanSetAndGetUniqueCode()
    {
        var testValue = new Guid("13abbdfb-24e1-4965-b976-6c5dd4e4f4d9");
        _testClass.UniqueCode = testValue;
        _testClass.UniqueCode.Should().Be(testValue);
    }
}

```
