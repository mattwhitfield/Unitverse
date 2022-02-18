# POCO Initialization
Demonstrates how test values are produced to initialize POCO members when the type is consumed

### Source Type(s)
``` csharp
public class SomePoco
{
    public int Identity { get; set; }
    public string Description { get; set; }
    public Guid UniqueCode { get; set; }
}

public class ConsumingClass
{
    SomePoco _poco;

    public ConsumingClass(SomePoco poco)
    {
        _poco = poco;
    }

    public SomePoco Poco => _poco;
}

```

### Generated Tests
``` csharp
public class ConsumingClassTests
{
    private ConsumingClass _testClass;
    private SomePoco _poco;

    public ConsumingClassTests()
    {
        _poco = new SomePoco { Identity = 534011718, Description = "TestValue237820880", UniqueCode = new Guid("97408286-a3e4-cf95-ff46-699c73c4a1cd") };
        _testClass = new ConsumingClass(_poco);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new ConsumingClass(_poco);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullPoco()
    {
        FluentActions.Invoking(() => new ConsumingClass(default(SomePoco))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PocoIsInitializedCorrectly()
    {
        _testClass.Poco.Should().BeSameAs(_poco);
    }
}

```
