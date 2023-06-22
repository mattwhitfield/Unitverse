# Automatic Mock Generation
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

    delegate void SampleDelegate();

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

    public async Task SampleDependencyCalledInsidePrivateMethod()
    {
        await _dummyService.AsyncMethod();
        await PrivateMethod();
    }

    public async Task SampleDeeperNestedDependencyCall()
    {
        await _dummyService.AsyncMethod();
        await PrivateMethodWrapper();
    }

    public async Task SampleDependencyCalledInsidePublicMethod()
    {
        await _dummyService.AsyncMethod();
        await PublicMethod();
    }

    public async Task SampleDependencyCalledAsADelegateMethod()
    {
        await _dummyService.AsyncMethod();
        SampleDelegate myDelegate = async () => await PrivateMethod();
        myDelegate();
    }

    public async Task SampleDependencyCalledAsALambdaMethod()
    {
        await _dummyService.AsyncMethod();

        Func<Task> myLambda = async () => await PrivateMethod();
        await myLambda();
    }

    public async Task SampleDependencyCalledAsAActionMethod()
    {
        await _dummyService.AsyncMethod();

        Func<Task> myLambda = async () => await PrivateMethod();
        await myLambda();
    }

    private async Task PublicMethod()
    {
        await _dummyService2.AsyncMethod("foo");
    }

    private async Task PrivateMethodWrapper()
    {
        await PrivateMethod();
    }

    private async Task PrivateMethod()
    {
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
        // Act
        var instance = new AutomaticMockGenerationExample(_dummyService, _dummyService2);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullDummyService()
    {
        FluentActions.Invoking(() => new AutomaticMockGenerationExample(default(IDummyService), _dummyService2)).Should().Throw<ArgumentNullException>().WithParameterName("dummyService");
    }

    [Fact]
    public void CannotConstructWithNullDummyService2()
    {
        FluentActions.Invoking(() => new AutomaticMockGenerationExample(_dummyService, default(IDummyService2))).Should().Throw<ArgumentNullException>().WithParameterName("dummyService2");
    }

    [Fact]
    public async Task CanCallSampleAsyncMethod()
    {
        // Arrange
        _dummyService.AsyncMethod().Returns("TestValue534011718");
        _dummyService2.AsyncMethod(Arg.Any<string>()).Returns("TestValue237820880");

        // Act
        await _testClass.SampleAsyncMethod();

        // Assert
        await _dummyService.Received().AsyncMethod();
        await _dummyService2.Received().AsyncMethod(Arg.Any<string>());

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public async Task CanCallSampleDependencyCalledInsidePrivateMethod()
    {
        // Arrange
        _dummyService.AsyncMethod().Returns("TestValue1002897798");
        _dummyService2.AsyncMethod(Arg.Any<string>()).Returns("TestValue1657007234");

        // Act
        await _testClass.SampleDependencyCalledInsidePrivateMethod();

        // Assert
        await _dummyService.Received().AsyncMethod();
        await _dummyService2.Received().AsyncMethod(Arg.Any<string>());

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public async Task CanCallSampleDeeperNestedDependencyCall()
    {
        // Arrange
        _dummyService.AsyncMethod().Returns("TestValue1412011072");
        _dummyService2.AsyncMethod(Arg.Any<string>()).Returns("TestValue929393559");

        // Act
        await _testClass.SampleDeeperNestedDependencyCall();

        // Assert
        await _dummyService.Received().AsyncMethod();
        await _dummyService2.Received().AsyncMethod(Arg.Any<string>());

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public async Task CanCallSampleDependencyCalledInsidePublicMethod()
    {
        // Arrange
        _dummyService.AsyncMethod().Returns("TestValue760389092");
        _dummyService2.AsyncMethod(Arg.Any<string>()).Returns("TestValue2026928803");

        // Act
        await _testClass.SampleDependencyCalledInsidePublicMethod();

        // Assert
        await _dummyService.Received().AsyncMethod();
        await _dummyService2.Received().AsyncMethod(Arg.Any<string>());

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public async Task CanCallSampleDependencyCalledAsADelegateMethod()
    {
        // Arrange
        _dummyService.AsyncMethod().Returns("TestValue217468053");
        _dummyService2.AsyncMethod(Arg.Any<string>()).Returns("TestValue1379662799");

        // Act
        await _testClass.SampleDependencyCalledAsADelegateMethod();

        // Assert
        await _dummyService.Received().AsyncMethod();
        await _dummyService2.Received().AsyncMethod(Arg.Any<string>());

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public async Task CanCallSampleDependencyCalledAsALambdaMethod()
    {
        // Arrange
        _dummyService.AsyncMethod().Returns("TestValue61497087");
        _dummyService2.AsyncMethod(Arg.Any<string>()).Returns("TestValue532638534");

        // Act
        await _testClass.SampleDependencyCalledAsALambdaMethod();

        // Assert
        await _dummyService.Received().AsyncMethod();
        await _dummyService2.Received().AsyncMethod(Arg.Any<string>());

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public async Task CanCallSampleDependencyCalledAsAActionMethod()
    {
        // Arrange
        _dummyService.AsyncMethod().Returns("TestValue687431273");
        _dummyService2.AsyncMethod(Arg.Any<string>()).Returns("TestValue2125508764");

        // Act
        await _testClass.SampleDependencyCalledAsAActionMethod();

        // Assert
        await _dummyService.Received().AsyncMethod();
        await _dummyService2.Received().AsyncMethod(Arg.Any<string>());

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCallSampleNoReturn()
    {
        // Arrange
        var srr = "TestValue1321446349";

        _dummyService.GenericMethod<string>(Arg.Any<string>()).Returns("TestValue1464848243");
        _dummyService2.ReturnMethod(Arg.Any<string>(), Arg.Any<string>()).Returns(1406361028);
        _dummyService2.SomeProp.Returns("TestValue607156385");

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
