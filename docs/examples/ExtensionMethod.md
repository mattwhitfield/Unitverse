# Extension Methods
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
        // Arrange
        var source = "TestValue534011718";

        // Act
        var result = source.ToOther();

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public static void CannotCallToOtherWithStringWithInvalidSource(string value)
    {
        FluentActions.Invoking(() => value.ToOther()).Should().Throw<ArgumentNullException>().WithParameterName("source");
    }

    [Fact]
    public static void CanCallToOtherWithListOfT()
    {
        // Arrange
        var source = new List<T>();

        // Act
        var result = source.ToOther<T>();

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public static void CannotCallToOtherWithListOfTWithNullSource()
    {
        FluentActions.Invoking(() => default(List<T>).ToOther<T>()).Should().Throw<ArgumentNullException>().WithParameterName("source");
    }
}

```
