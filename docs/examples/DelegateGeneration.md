# Delegate Generation
Demonstrates how Unitverse generates default values for method parameters when the parameter is a delegate type

### Source Type(s)
``` csharp
public class SomeClass
{
    public SomeClass(int val)
    {
        Val = val;
    }

    public int Val { get; }
}

public static class TestClass
{
    public static void ThisIsAMethod(Func<string> func)
    {
    }

    public static void ThisIsAMethod2(Func<string, SomeClass> func)
    {
    }

    public static void ThisIsAMethod3(Func<int, string, SomeClass> func)
    {
    }

    public static void ThisIsAMethod4(Func<int, int, string, SomeClass> func)
    {
    }

    public static void ThisIsAMethod5(Func<int, int, int, string, SomeClass> func)
    {
    }

    public static void ThisIsAMethod6(Action action)
    {
    }

    public static void ThisIsAMethod7(Action<SomeClass> action)
    {
    }

    public static void ThisIsAMethod8(Action<SomeClass, int> action)
    {
    }

    public static void ThisIsAMethod9(Action<SomeClass, int, int> action)
    {
    }

    public static void ThisIsAMethod10(Action<SomeClass, int, int, int> action)
    {
    }

    public static void ThisIsAMethod11(Action<SomeClass, int, int, int, int> action)
    {
    }
}

```

### Generated Tests
``` csharp
public static class TestClassTests
{
    [Fact]
    public static void CanCallThisIsAMethod()
    {
        Func<string> func = () => "TestValue237820880";
        TestClass.ThisIsAMethod(func);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethodWithNullFunc()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod(default(Func<string>))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallThisIsAMethod2()
    {
        Func<string, SomeClass> func = x => new SomeClass(1002897798);
        TestClass.ThisIsAMethod2(func);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethod2WithNullFunc()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod2(default(Func<string, SomeClass>))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallThisIsAMethod3()
    {
        Func<int, string, SomeClass> func = (x, y) => new SomeClass(1657007234);
        TestClass.ThisIsAMethod3(func);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethod3WithNullFunc()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod3(default(Func<int, string, SomeClass>))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallThisIsAMethod4()
    {
        Func<int, int, string, SomeClass> func = (x, y, z) => new SomeClass(1412011072);
        TestClass.ThisIsAMethod4(func);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethod4WithNullFunc()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod4(default(Func<int, int, string, SomeClass>))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallThisIsAMethod5()
    {
        Func<int, int, int, string, SomeClass> func = (a, b, c, d) => new SomeClass(929393559);
        TestClass.ThisIsAMethod5(func);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethod5WithNullFunc()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod5(default(Func<int, int, int, string, SomeClass>))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallThisIsAMethod6()
    {
        Action action = () => { };
        TestClass.ThisIsAMethod6(action);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethod6WithNullAction()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod6(default(Action))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallThisIsAMethod7()
    {
        Action<SomeClass> action = x => { };
        TestClass.ThisIsAMethod7(action);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethod7WithNullAction()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod7(default(Action<SomeClass>))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallThisIsAMethod8()
    {
        Action<SomeClass, int> action = (x, y) => { };
        TestClass.ThisIsAMethod8(action);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethod8WithNullAction()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod8(default(Action<SomeClass, int>))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallThisIsAMethod9()
    {
        Action<SomeClass, int, int> action = (x, y, z) => { };
        TestClass.ThisIsAMethod9(action);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethod9WithNullAction()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod9(default(Action<SomeClass, int, int>))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallThisIsAMethod10()
    {
        Action<SomeClass, int, int, int> action = (a, b, c, d) => { };
        TestClass.ThisIsAMethod10(action);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethod10WithNullAction()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod10(default(Action<SomeClass, int, int, int>))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallThisIsAMethod11()
    {
        Action<SomeClass, int, int, int, int> action = (a, b, c, d, e) => { };
        TestClass.ThisIsAMethod11(action);
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallThisIsAMethod11WithNullAction()
    {
        FluentActions.Invoking(() => TestClass.ThisIsAMethod11(default(Action<SomeClass, int, int, int, int>))).Should().Throw<ArgumentNullException>();
    }
}

```
