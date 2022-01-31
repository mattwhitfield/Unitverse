## AbstractClass
Demonstrates how Unitverse generates tests when the source class is abstract or contains protected methods, as well as how inheritance chains are accounted for

### Source Type(s)
``` csharp
public abstract class TestClass1
{
    protected TestClass1()
    { }

    public abstract int SomeMethodShouldNot();

    public abstract int SomeMethodMaybe(int i);

    public abstract int SomeMethodMaybe(int i, int j);

    public abstract int SomeMethodMaybe<T>(int i);

    public abstract int SomeMethodMaybe<T>(int i, int j);
}

public abstract class TestClass2 : TestClass1
{
    protected TestClass2()
    { }

    public override int SomeMethodShouldNot() { return 1; }

    public override int SomeMethodMaybe(int i, int j) { return 1; }

    public abstract int SomeMethodShould();
}

public abstract class TestClass3 : TestClass2
{
    protected TestClass3()
    { }

    public abstract int SomeMethodShould2();
}

```

### Generated Tests
``` csharp
private class TestTestClass3 : TestClass3
{
    public TestTestClass3() : base()
    {
    }

    public override int SomeMethodShould2()
    {
        return default(int);
    }

    public override int SomeMethodMaybe(int i)
    {
        return default(int);
    }

    public override int SomeMethodShould()
    {
        return default(int);
    }

    public override int SomeMethodMaybe<T>(int i)
    {
        return default(int);
    }

    public override int SomeMethodMaybe<T>(int i, int j)
    {
        return default(int);
    }
}

```
