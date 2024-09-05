# Constrained Generic Types
Demonstrates how appropriate types are selected for the generation of tests for generic types with type constraints

### Source Type(s)
``` csharp
public interface ITest
{
    int ThisIsAProperty { get; set; }
}

public interface ITest2
{
    int ThisIsAnotherProperty { get; set; }
}

public class Test : ITest
{
    public int ThisIsAProperty { get; set; }
}

public class TestBoth : ITest, ITest2
{
    public int ThisIsAProperty { get; set; }
    public int ThisIsAnotherProperty { get; set; }
}

public class TestClass<T, R>
    where T : class, ITest, new()
    where R : class, ITest, ITest2, new()
{
    public TestClass(T insta, R insta2)
    {
        Insta = insta;
        Insta2 = insta2;
    }

    public T Insta { get; }

    public R Insta2 { get; }
}

```

### Generated Tests
``` csharp
public class TestClass_2Tests
{
    private readonly TestClass<T, R> _testClass;
    private T _insta;
    private R _insta2;

    public TestClass_2Tests()
    {
        _insta = new Test { ThisIsAProperty = 534011718 };
        _insta2 = new TestBoth
        {
            ThisIsAProperty = 237820880,
            ThisIsAnotherProperty = 1002897798
        };
        _testClass = new TestClass<T, R>(_insta, _insta2);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new TestClass<T, R>(_insta, _insta2);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullInsta()
    {
        FluentActions.Invoking(() => new TestClass<T, R>(default(T), _insta2)).Should().Throw<ArgumentNullException>().WithParameterName("insta");
    }

    [Fact]
    public void CannotConstructWithNullInsta2()
    {
        FluentActions.Invoking(() => new TestClass<T, R>(_insta, default(R))).Should().Throw<ArgumentNullException>().WithParameterName("insta2");
    }

    [Fact]
    public void InstaIsInitializedCorrectly()
    {
        _testClass.Insta.Should().BeSameAs(_insta);
    }

    [Fact]
    public void Insta2IsInitializedCorrectly()
    {
        _testClass.Insta2.Should().BeSameAs(_insta2);
    }
}

```
