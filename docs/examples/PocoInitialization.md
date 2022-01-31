## PocoInitialization
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
        _poco = new SomePoco { Identity = 1002897798, Description = "TestValue1657007234", UniqueCode = new Guid("c9cc8d7a-6091-4509-9e97-98baef4f5262") };
        _testClass = new ConsumingClass(_poco);
    }

    [Fact]
    public void CanConstruct()
    {
        var instance = new ConsumingClass(_poco);
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
