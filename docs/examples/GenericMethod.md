## GenericMethod
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
        var g = new Guid("5f5453de-2157-4f3d-9f12-2bb6a7ca08d5");
        var dtParam = DateTime.UtcNow;
        var theThing = "TestValue534011718";
        var thing2 = 237820880;
        _testClass.DoStuff<T>(g, dtParam, theThing, thing2);
        throw new NotImplementedException("Create or modify test");
    }
}

```
