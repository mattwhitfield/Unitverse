# Mapping Methods
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
        // Arrange
        var inputClass = new InputClass
        {
            SomeOtherProperty = "TestValue534011718",
            InputOnlyProperty = "TestValue237820880"
        };

        // Act
        var result = _testClass.Map(inputClass);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCallMapWithNullInputClass()
    {
        FluentActions.Invoking(() => _testClass.Map(default(InputClass))).Should().Throw<ArgumentNullException>().WithParameterName("inputClass");
    }

    [Fact]
    public void MapPerformsMapping()
    {
        // Arrange
        var inputClass = new InputClass
        {
            SomeOtherProperty = "TestValue1002897798",
            InputOnlyProperty = "TestValue1657007234"
        };

        // Act
        var result = _testClass.Map(inputClass);

        // Assert
        result.SomeProperty.Should().BeSameAs(inputClass.SomeProperty);
        result.SomeOtherProperty.Should().BeSameAs(inputClass.SomeOtherProperty);
    }
}

```
