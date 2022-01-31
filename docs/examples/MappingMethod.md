## MappingMethod
Shows how unitverse generates a test to verify mappings between input parameter type and return type where the types share property names

### Source Type(s)
``` csharp
public class InputClass
{
    public string SomeProperty { get; }
    public string SomeOtherProperty { get; set; }
    public string InputOnlyProperty { get; set; }
}

public class OutputClass
{
    public string SomeProperty { get; set; }
    public string SomeOtherProperty { get; set; }
    public string OutputOnlyProperty { get; set; }
}

public class MappingClass
{
    public OutputClass Map(InputClass inputClass)
    {
        return null;
    }
}

```

### Generated Tests
``` csharp
public class MappingClassTests
{
    private MappingClass _testClass;

    public MappingClassTests()
    {
        _testClass = new MappingClass();
    }

    [Fact]
    public void CanCallMap()
    {
        var inputClass = new InputClass { SomeOtherProperty = "TestValue451370593", InputOnlyProperty = "TestValue1727968365" };
        var result = _testClass.Map(inputClass);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallMapWithNullInputClass()
    {
        FluentActions.Invoking(() => _testClass.Map(default(InputClass))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapPerformsMapping()
    {
        var inputClass = new InputClass { SomeOtherProperty = "TestValue1758450055", InputOnlyProperty = "TestValue66790043" };
        var result = _testClass.Map(inputClass);
        result.SomeProperty.Should().BeSameAs(inputClass.SomeProperty);
        result.SomeOtherProperty.Should().BeSameAs(inputClass.SomeOtherProperty);
    }
}

```
