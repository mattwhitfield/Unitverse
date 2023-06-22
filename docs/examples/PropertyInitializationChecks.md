# Property Initialization Checks
Demonstrates how properties that have matching constructor parameters are checked that they are initialized automatically

### Source Type(s)
``` csharp
public class ExampleClass
{
    public ExampleClass(int identity, string description, Guid uniqueCode)
    {
        Identity = identity;
        Description = description;
        UniqueCode = uniqueCode;
    }

    public int Identity { get; }
    public string Description { get; }
    public Guid UniqueCode { get; }
}

```

### Generated Tests
``` csharp
public class ExampleClassTests
{
    private ExampleClass _testClass;
    private int _identity;
    private string _description;
    private Guid _uniqueCode;

    public ExampleClassTests()
    {
        _identity = 534011718;
        _description = "TestValue237820880";
        _uniqueCode = new Guid("97408286-a3e4-cf95-ff46-699c73c4a1cd");
        _testClass = new ExampleClass(_identity, _description, _uniqueCode);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new ExampleClass(_identity, _description, _uniqueCode);

        // Assert
        instance.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotConstructWithInvalidDescription(string value)
    {
        FluentActions.Invoking(() => new ExampleClass(_identity, value, _uniqueCode)).Should().Throw<ArgumentNullException>().WithParameterName("description");
    }

    [Fact]
    public void IdentityIsInitializedCorrectly()
    {
        _testClass.Identity.Should().Be(_identity);
    }

    [Fact]
    public void DescriptionIsInitializedCorrectly()
    {
        _testClass.Description.Should().Be(_description);
    }

    [Fact]
    public void UniqueCodeIsInitializedCorrectly()
    {
        _testClass.UniqueCode.Should().Be(_uniqueCode);
    }
}

```
