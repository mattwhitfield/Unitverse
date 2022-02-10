## Extension Methods
Demonstrates how Unitverse generates tests for extension methods

### Source Type(s)
``` csharp
public static class ExtensionMethodClass
{
    public static string ToOther(this string source)
    {
        return source;
    }

    public static T ToOther<T>(this List<T> source)
    {
        return source.FirstOrDefault();
    }
}

```

### Generated Tests
``` csharp
public static class ExtensionMethodClassTests
{
    [Fact]
    public static void CanCallToOtherWithString()
    {
        var source = "TestValue534011718";
        var result = source.ToOther();
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public static void CannotCallToOtherWithStringWithInvalidSource(string value)
    {
        FluentActions.Invoking(() => value.ToOther()).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public static void CanCallToOtherWithListOfT()
    {
        var source = new List<T>();
        var result = source.ToOther<T>();
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallToOtherWithListOfTWithNullSource()
    {
        FluentActions.Invoking(() => default(List<T>).ToOther<T>()).Should().Throw<ArgumentNullException>();
    }
}

```
