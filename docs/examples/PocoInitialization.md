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

public class TestClass
{
    SomePoco _poco;

    public TestClass(SomePoco poco)
    {
        _poco = poco;
    }

    public SomePoco Poco => _poco;
}

```

### Generated Tests
``` csharp
public class TestClassTests
{
    private TestClass _testClass;
    private SomePoco _poco;

    public TestClassTests()
    {
        _poco = new SomePoco { Identity = 1896117029, Description = "TestValue377514313", UniqueCode = new Guid("8a91a0d6-994b-4156-a72b-e781ca0845da") };
        _testClass = new TestClass(_poco);
    }

    [Fact]
    public void CanConstruct()
    {
        var instance = new TestClass(_poco);
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullPoco()
    {
        FluentActions.Invoking(() => new TestClass(default(SomePoco))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PocoIsInitializedCorrectly()
    {
        _testClass.Poco.Should().BeSameAs(_poco);
    }
}

```
