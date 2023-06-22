# Record Types (init Properties)
Demonstrates the tests generated for a record type that has properties that have init accessors

### Source Type(s)
``` csharp
record Person
{
    private readonly string _firstName;
    private readonly string _lastName;

    public Guid Id { get; init; }

    public string FirstName
    {
        get => _firstName;
        init => _firstName = (value ?? throw new ArgumentNullException(nameof(value)));
    }

    public string? MiddleName { get; init; }

    public string LastName
    {
        get => _lastName;
        init => _lastName = (value ?? throw new ArgumentNullException(nameof(value)));
    }

    public IList<string> IceCreamFlavours { get; init; }
}

```

### Generated Tests
``` csharp
public class PersonTests
{
    private Person _testClass;
    private Guid _id;
    private string _firstName;
    private string _middleName;
    private string _lastName;
    private IList<string> _iceCreamFlavours;

    public PersonTests()
    {
        _id = new Guid("8286d046-9740-a3e4-95cf-ff46699c73c4");
        _firstName = "TestValue607156385";
        _middleName = "TestValue1321446349";
        _lastName = "TestValue1512368656";
        _iceCreamFlavours = new[] { "TestValue1507096884", "TestValue2039633683", "TestValue200550235" };
        _testClass = new Person
        {
            Id = _id,
            FirstName = _firstName,
            MiddleName = _middleName,
            LastName = _lastName,
            IceCreamFlavours = _iceCreamFlavours
        };
    }

    [Fact]
    public void CanInitialize()
    {
        // Act
        var instance = new Person
        {
            Id = _id,
            FirstName = _firstName,
            MiddleName = _middleName,
            LastName = _lastName,
            IceCreamFlavours = _iceCreamFlavours
        };

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotInitializeWithNullIceCreamFlavours()
    {
        FluentActions.Invoking(() => new Person
        {
            Id = _id,
            FirstName = _firstName,
            MiddleName = _middleName,
            LastName = _lastName,
            IceCreamFlavours = default(IList<string>)
        }).Should().Throw<ArgumentNullException>().WithParameterName("IceCreamFlavours");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotInitializeWithInvalidFirstName(string value)
    {
        FluentActions.Invoking(() => new Person
        {
            Id = _id,
            FirstName = value,
            MiddleName = _middleName,
            LastName = _lastName,
            IceCreamFlavours = _iceCreamFlavours
        }).Should().Throw<ArgumentNullException>().WithParameterName("FirstName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotInitializeWithInvalidMiddleName(string value)
    {
        FluentActions.Invoking(() => new Person
        {
            Id = _id,
            FirstName = _firstName,
            MiddleName = value,
            LastName = _lastName,
            IceCreamFlavours = _iceCreamFlavours
        }).Should().Throw<ArgumentNullException>().WithParameterName("MiddleName");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotInitializeWithInvalidLastName(string value)
    {
        FluentActions.Invoking(() => new Person
        {
            Id = _id,
            FirstName = _firstName,
            MiddleName = _middleName,
            LastName = value,
            IceCreamFlavours = _iceCreamFlavours
        }).Should().Throw<ArgumentNullException>().WithParameterName("LastName");
    }

    [Fact]
    public void ImplementsIEquatable_Person()
    {
        // Arrange
        var same = new Person
        {
            Id = _id,
            FirstName = _firstName,
            MiddleName = _middleName,
            LastName = _lastName,
            IceCreamFlavours = _iceCreamFlavours
        };
        var different = new Person();

        // Assert
        _testClass.Equals(default(object)).Should().BeFalse();
        _testClass.Equals(new object()).Should().BeFalse();
        _testClass.Equals((object)same).Should().BeTrue();
        _testClass.Equals((object)different).Should().BeFalse();
        _testClass.Equals(same).Should().BeTrue();
        _testClass.Equals(different).Should().BeFalse();
        _testClass.GetHashCode().Should().Be(same.GetHashCode());
        _testClass.GetHashCode().Should().NotBe(different.GetHashCode());
        (_testClass == same).Should().BeTrue();
        (_testClass == different).Should().BeFalse();
        (_testClass != same).Should().BeFalse();
        (_testClass != different).Should().BeTrue();
    }

    [Fact]
    public void IdIsInitializedCorrectly()
    {
        _testClass.Id.Should().Be(_id);
    }

    [Fact]
    public void FirstNameIsInitializedCorrectly()
    {
        _testClass.FirstName.Should().Be(_firstName);
    }

    [Fact]
    public void MiddleNameIsInitializedCorrectly()
    {
        _testClass.MiddleName.Should().Be(_middleName);
    }

    [Fact]
    public void LastNameIsInitializedCorrectly()
    {
        _testClass.LastName.Should().Be(_lastName);
    }

    [Fact]
    public void IceCreamFlavoursIsInitializedCorrectly()
    {
        _testClass.IceCreamFlavours.Should().BeSameAs(_iceCreamFlavours);
    }
}

```
