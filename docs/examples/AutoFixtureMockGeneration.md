# AutoFixture Mock Generation
Demonstrates how constructor dependencies can be configured using AutoFixture, and mock configuration calls can still be generated

### Source Type(s)
``` csharp
public interface IDummyService
{
    string SomeProp { get; set; }

    void NoReturnMethod();

    int ReturnMethod();

    T GenericMethod<T>(T val);

    Task<string> AsyncMethod();
}

public interface IDummyService2
{
    string SomeProp { get; }

    void NoReturnMethod(string s);

    int ReturnMethod(string s);
    int ReturnMethod(string s, string s2);
    int ReturnMethod(string s, object o);

    Task<string> AsyncMethod(string s);
}

public static class DummyExt
{
    public static int ReturnMethod(this IDummyService2 d, string s, string s2, string s3)
    {
        return 0;
    }
}

public class AutomaticMockGenerationExample
{
    private IDummyService _dummyService, _otherDummyService;
    private IDummyService2 _dummyService2;
    private int _someIntField;

    public AutomaticMockGenerationExample(IDummyService dummyService, IDummyService2 dummyService2)
    {
        _dummyService = dummyService ?? throw new System.ArgumentNullException(nameof(dummyService));
        _dummyService2 = dummyService2;
        _someIntField = dummyService.ReturnMethod();
        dummyService = null;
    }

    public async Task SampleAsyncMethod()
    {
        await _dummyService.AsyncMethod();
        await _dummyService2.AsyncMethod("foo");
    }

    public void SampleNoReturn(string srr)
    {
        _dummyService.NoReturnMethod();
        _dummyService.NoReturnMethod();
        _dummyService.NoReturnMethod();
        var s = _dummyService.GenericMethod(srr);

        _dummyService.SomeProp += _dummyService2.SomeProp;

        var value = _dummyService2.ReturnMethod("sds", "s");
        var value2 = _dummyService2.ReturnMethod("sds", "s", "s3");
    }
}

```

### Generated Tests
``` csharp
public class AutomaticMockGenerationExampleTests
{
    private AutomaticMockGenerationExample _testClass;
    private IDummyService _dummyService;
    private IDummyService2 _dummyService2;

    public AutomaticMockGenerationExampleTests()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _dummyService = fixture.Freeze<IDummyService>();
        _dummyService2 = fixture.Freeze<IDummyService2>();
        _testClass = fixture.Create<AutomaticMockGenerationExample>();
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new AutomaticMockGenerationExample(_dummyService, _dummyService2);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullDummyService()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        FluentActions.Invoking(() => new AutomaticMockGenerationExample(default(IDummyService), fixture.Create<IDummyService2>())).Should().Throw<ArgumentNullException>().WithParameterName("dummyService");
    }

    [Fact]
    public void CannotConstructWithNullDummyService2()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        FluentActions.Invoking(() => new AutomaticMockGenerationExample(fixture.Create<IDummyService>(), default(IDummyService2))).Should().Throw<ArgumentNullException>().WithParameterName("dummyService2");
    }

    [Fact]
    public async Task CanCallSampleAsyncMethod()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        _dummyService.AsyncMethod().Returns(fixture.Create<string>());
        _dummyService2.AsyncMethod(Arg.Any<string>()).Returns(fixture.Create<string>());

        // Act
        await _testClass.SampleAsyncMethod();

        // Assert
        await _dummyService.Received().AsyncMethod();
        await _dummyService2.Received().AsyncMethod(Arg.Any<string>());

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallSampleNoReturn()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var srr = fixture.Create<string>();

        _dummyService.GenericMethod<string>(Arg.Any<string>()).Returns(fixture.Create<string>());
        _dummyService2.ReturnMethod(Arg.Any<string>(), Arg.Any<string>()).Returns(fixture.Create<int>());
        _dummyService2.SomeProp.Returns(fixture.Create<string>());

        // Act
        _testClass.SampleNoReturn(srr);

        // Assert
        _dummyService.Received().NoReturnMethod();
        _dummyService.Received().GenericMethod<string>(Arg.Any<string>());
        _dummyService2.Received().ReturnMethod(Arg.Any<string>(), Arg.Any<string>());

        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCallSampleNoReturnWithInvalidSrr(string value)
    {
        FluentActions.Invoking(() => _testClass.SampleNoReturn(value)).Should().Throw<ArgumentNullException>().WithParameterName("srr");
    }
}

```
