# Generic Methods
Demonstrates how Unitverse generates tests for generic methods

### Source Type(s)
``` csharp
public class GenericSource
{
    public void DoStuff<T>(Guid g, DateTime dtParam, T theThing, int? thing2)
    {

    }
}

```

### Generated Tests
``` csharp
public class GenericSourceTests
{
    private GenericSource _testClass;

    public GenericSourceTests()
    {
        _testClass = new GenericSource();
    }

    [Fact]
    public void CanCallDoStuff()
    {
        var g = new Guid("8286d046-9740-a3e4-95cf-ff46699c73c4");
        var dtParam = DateTime.UtcNow;
        var theThing = "TestValue607156385";
        var thing2 = 1321446349;
        _testClass.DoStuff<T>(g, dtParam, theThing, thing2);
        throw new NotImplementedException("Create or modify test");
    }
}

```
