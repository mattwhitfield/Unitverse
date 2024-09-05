# Simple POCO
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
    private readonly SomePoco _testClass;

    public SomePocoTests()
    {
        _testClass = new SomePoco();
    }

    [Fact]
    public void CanSetAndGetIdentity()
    {
        // Arrange
        var testValue = 534011718;

        // Act
        _testClass.Identity = testValue;

        // Assert
        _testClass.Identity.Should().Be(testValue);
    }

    [Fact]
    public void CanSetAndGetDescription()
    {
        // Arrange
        var testValue = "TestValue237820880";

        // Act
        _testClass.Description = testValue;

        // Assert
        _testClass.Description.Should().Be(testValue);
    }

    [Fact]
    public void CanSetAndGetUniqueCode()
    {
        // Arrange
        var testValue = new Guid("97408286-a3e4-cf95-ff46-699c73c4a1cd");

        // Act
        _testClass.UniqueCode = testValue;

        // Assert
        _testClass.UniqueCode.Should().Be(testValue);
    }
}

```
