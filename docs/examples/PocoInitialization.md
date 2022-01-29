## PocoInitialization
Description

### Source Type
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
        _poco = new SomePoco { Identity = 427266051, Description = "TestValue166470585", UniqueCode = new Guid("1b64e1ac-afdd-4692-9d56-aa3f4467ce74") };
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
