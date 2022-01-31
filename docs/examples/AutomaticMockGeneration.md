## AutomaticMockGeneration
Demonstrates how dependencies injected into constructors are tracked, and mock configuration calls emitted for any detected dependencies

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
        _dummyService = Substitute.For<IDummyService>();
        _dummyService2 = Substitute.For<IDummyService2>();
        _testClass = new AutomaticMockGenerationExample(_dummyService, _dummyService2);
    }

    [Fact]
    public void CanConstruct()
    {
        var instance = new AutomaticMockGenerationExample(_dummyService, _dummyService2);
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullDummyService()
    {
        FluentActions.Invoking(() => new AutomaticMockGenerationExample(default(IDummyService), Substitute.For<IDummyService2>())).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CannotConstructWithNullDummyService2()
    {
        FluentActions.Invoking(() => new AutomaticMockGenerationExample(Substitute.For<IDummyService>(), default(IDummyService2))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task CanCallSampleAsyncMethod()
    {
        _dummyService.AsyncMethod().Returns("TestValue687431273");
        _dummyService2.AsyncMethod(Arg.Any<string>()).Returns("TestValue2125508764");

        await _testClass.SampleAsyncMethod();

        await _dummyService.Received().AsyncMethod();
        await _dummyService2.Received().AsyncMethod(Arg.Any<string>());

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallSampleNoReturn()
    {
        var srr = "TestValue1321446349";

        _dummyService.GenericMethod<string>(Arg.Any<string>()).Returns("TestValue1464848243");
        _dummyService2.ReturnMethod(Arg.Any<string>(), Arg.Any<string>()).Returns(1406361028);
        _dummyService2.SomeProp.Returns("TestValue607156385");

        _testClass.SampleNoReturn(srr);

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
        FluentActions.Invoking(() => _testClass.SampleNoReturn(value)).Should().Throw<ArgumentNullException>();
    }
}

```
