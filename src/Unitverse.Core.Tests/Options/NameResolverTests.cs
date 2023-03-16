namespace Unitverse.Core.Tests.Options
{
    using Unitverse.Core.Options;
    using System;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class NameResolverTests
    {
        private string _pattern;

        [SetUp]
        public void SetUp()
        {
            _pattern = "TestValue222916131";
            
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new NameResolver(_pattern);
            instance.Should().NotBeNull();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CanConstructWithInvalidPattern(string value)
        {
            FluentActions.Invoking(() => new NameResolver(value)).Should().NotThrow();
        }

        [Test]
        [TestCase("someStuff{typeName}", "someStuffType1")]
        [TestCase("someStuff{typeName}{interfaceName}", "someStuffType1Interface1")]
        [TestCase("someStuff{typeName:camel}{interfaceName}", "someStufftype1Interface1")]
        [TestCase("{interfaceName}", "Interface1")]
        [TestCase("{memberName}", "Member1WithThing")]
        [TestCase("{memberName:upper}", "MEMBER1WITHTHING")]
        [TestCase("{memberName:lower}", "member1withthing")]
        [TestCase("{memberName:nonexitstent}", "Member1WithThing")]
        [TestCase("{nonexitstent}", "")]
        [TestCase("{nonexitstent:nonexitstent}", "")]
        [TestCase("{badlyFormatted", "")]
        [TestCase("badlyFormatted}", "badlyFormatted}")]
        [TestCase("{memberBareName}", "Member1")]
        [TestCase("{parameterName}", "parameter1")]
        [TestCase("_{parameterName:camel}", "_parameter1")]
        [TestCase("{parameterName:pascal}", "Parameter1")]
        [TestCase("{typeParameters}", "TypeParam1")]
        public void CanCallResolve(string pattern, string expectedResult)
        {
            var context = new NamingContext("Type1").WithInterfaceName("Interface1").WithMemberName("Member1WithThing", "Member1").WithParameterName("parameter1").WithTypeParameters("TypeParam1");
            var testClass = new NameResolver(pattern);
            var result = testClass.Resolve(context);
            result.Should().Be(expectedResult);
        }
    }
}