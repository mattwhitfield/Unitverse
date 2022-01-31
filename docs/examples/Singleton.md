## Singleton
Demonstrates how Unitverse attempts to use a static property to get a type instance when the constructor is private

### Source Type(s)
``` csharp
public class TestClass
{
    static TestClass()
    {
        Instance = new TestClass();
    }

    private TestClass()
    {
    }

    public static TestClass Instance { get; }

    public bool IsShared => true;

    public string GetTableName(string baseName)
    {
        return baseName;
    }
}

```

### Generated Tests
``` csharp
public class TestClassTests
{
    private TestClass _testClass;

    public TestClassTests()
    {
        _testClass = TestClass.Instance;
    }

    [Fact]
    public void CanCallGetTableName()
    {
        var baseName = "TestValue1645757808";
        var result = _testClass.GetTableName(baseName);
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallGetTableNameWithInvalidBaseName(string value)
    {
        FluentActions.Invoking(() => _testClass.GetTableName(value)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanGetInstance()
    {
        TestClass.Instance.Should().BeAssignableTo<TestClass>();
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanGetIsShared()
    {
        _testClass.IsShared.As<object>().Should().BeAssignableTo<bool>();
        throw new NotImplementedException("Create or modify test");
    }
}

```
