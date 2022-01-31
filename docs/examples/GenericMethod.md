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
        var g = new Guid("a9e13980-d9c5-4f1c-b18f-45b92be401ef");
        var dtParam = DateTime.UtcNow;
        var theThing = "TestValue1923631630";
        var thing2 = 1736744011;
        _testClass.DoStuff<T>(g, dtParam, theThing, thing2);
        throw new NotImplementedException("Create or modify test");
    }
}

```
