# Abstract Classes
Demonstrates how Unitverse generates tests when the source class is abstract or contains protected methods, as well as how inheritance chains are accounted for

### Source Type(s)
``` csharp
public abstract class TestClass
{
    protected TestClass()
    { }

    protected abstract int AbstractMethod();

    protected virtual int ProtectedMethod() => 1;

    public virtual int SomeMethod(int i) => 1;

    public virtual int GenericMethod<T>(int i) => 1;
}

```

### Generated Tests
``` csharp
public class TestClassTests
{
    private class TestTestClass : TestClass
    {
        public TestTestClass() : base()
        {
        }

        public int PublicProtectedMethod()
        {
            return base.ProtectedMethod();
        }

        protected override int AbstractMethod()
        {
            return default(int);
        }
    }

    private TestTestClass _testClass;

    public TestClassTests()
    {
        _testClass = new TestTestClass();
    }

    [Fact]
    public void CanCallProtectedMethod()
    {
        // Act
        var result = _testClass.PublicProtectedMethod();

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallSomeMethod()
    {
        // Arrange
        var i = 534011718;

        // Act
        var result = _testClass.SomeMethod(i);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallGenericMethod()
    {
        // Arrange
        var i = 237820880;

        // Act
        var result = _testClass.GenericMethod<T>(i);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }
}

```
