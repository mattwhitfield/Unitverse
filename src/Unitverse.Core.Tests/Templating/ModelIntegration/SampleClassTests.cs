using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using Unitverse.Core.Templating.Model;
using Unitverse.Core.Templating.Model.Implementation;

namespace Unitverse.Core.Tests.Templating.ModelIntegration
{
    [TestFixture]
    public class SampleClassTests
    {
        static SampleClassTests()
        {
            var model = ClassModelProvider.CreateModel(TemplateModelSources.TS_SampleClass);

            TemplatingModel = new ClassFilterModel(model);
        }

        static IClass TemplatingModel { get; }

        [Test]
        public void ClassHasType()
        {
            TemplatingModel.Type.Name.Should().Be("TestClass");
        }

        [Test]
        public void AttributesAreReturnedOnClass()
        {
            TemplatingModel.Attributes.Should().Contain(x => x.Type.Name == "SerializableAttribute" && x.Type.Namespace == "System" && x.Type.FullName == "System.SerializableAttribute");
        }

        [Test]
        public void AttributesAreReturnedOnConstructors()
        {
            TemplatingModel.Constructors.Should().ContainSingle(x => x.Attributes.Any(a => a.Type.Name == "ObsoleteAttribute") && x.Parameters.Count() == 1);
        }

        [Test]
        public void AttributesAreReturnedOnMethods()
        {
            TemplatingModel.Methods.Should().ContainSingle(x => x.Attributes.Any(a => a.Type.Name == "STAThreadAttribute"));
        }

        [Test]
        public void MultipleAttributesAreReturnedOnMethods()
        {
            TemplatingModel.Methods.Single(x => x.Name == "GottaLoveAttributes").Attributes.Select(x => x.Type.Name).Should().BeEquivalentTo("ObsoleteAttribute", "CallerLineNumberAttribute", "CategoryAttribute");
        }

        [Test]
        public void AttributesAreReturnedOnMethodParameters()
        {
            TemplatingModel.Methods.Should().ContainSingle(x => x.Parameters.Any(p => p.Attributes.Any(a => a.Type.Name == "CallerMemberNameAttribute")));
        }

        [Test]
        public void AttributesAreReturnedOnProperties()
        {
            TemplatingModel.Properties.Should().ContainSingle(x => x.Attributes.Any(a => a.Type.Name == "CategoryAttribute"));
        }

        [Test]
        public void ParametersAreReturnedOnConstructors()
        {
            TemplatingModel.Constructors.Single(x => x.Parameters.Count() == 1).Parameters.Select(x => x.Name).Should().BeEquivalentTo("stringProp");
            TemplatingModel.Constructors.Single(x => x.Parameters.Count() == 2).Parameters.Select(x => x.Name).Should().BeEquivalentTo("stringProp", "intProp");
        }

        [Test]
        public void ParametersAreReturnedOnMethods()
        {
            TemplatingModel.Methods.Single(x => x.Name == "ThisIsAMethod").Parameters.Select(x => x.Name).Should().BeEquivalentTo("methodName", "methodValue");
            TemplatingModel.Methods.Single(x => x.Name == "ThisIsAMethod").Parameters.Single(x => x.Name == "methodName").Type.Name.Should().Be("String");
            TemplatingModel.Methods.Single(x => x.Name == "ThisIsAMethod").Parameters.Single(x => x.Name == "methodValue").Type.Name.Should().Be("Int32");
        }

        [Test]
        public void PropertiesPassThroughParameters()
        {
            TemplatingModel.Properties.Count(x => x.HasGet && !x.HasSet).Should().Be(1);
            TemplatingModel.Properties.Count(x => x.HasSet && !x.HasGet).Should().Be(1);
            TemplatingModel.Properties.Count(x => x.HasGet && x.HasSet).Should().Be(1);
            TemplatingModel.Properties.Count(x => x.HasInit).Should().Be(0);
            TemplatingModel.Properties.Count(x => x.IsStatic).Should().Be(1);
            TemplatingModel.Properties.Single(x => x.Name == "ThisIsAReadOnlyString").Type.Name.Should().Be("String");
        }

        [Test]
        public void CanFindStaticMethods()
        {
            TemplatingModel.Methods.Single(x => x.IsStatic).Name.Should().Be("WillReturnAString");
        }

        [Test]
        public void CanFindAsyncMethods()
        {
            TemplatingModel.Methods.Single(x => x.IsAsync).Name.Should().Be("AsyncMethod");
        }

        [Test]
        public void CanFindVoidMethods()
        {
            TemplatingModel.Methods.Where(x => x.IsVoid).Select(x => x.Name).Should().BeEquivalentTo("MethodWithAttributeOnParameter", "ThisIsAMethod");
        }

        [Test]
        public void BaseTypesAreReturnedFromClasses()
        {
            TemplatingModel.BaseTypes.Count().Should().Be(1);
            TemplatingModel.BaseTypes.Single().Name.Should().Be("Object");
        }

        [Test]
        public void InterfacesAreReturnedFromClasses()
        {
            TemplatingModel.Interfaces.Count().Should().Be(1);
            TemplatingModel.Interfaces.Single().Name.Should().Be("IEnumerable<>");
        }

        [Test]
        public void AllInterfacesAreReturnedFromClasses()
        {
            TemplatingModel.AllInterfaces.Count().Should().Be(2);
            TemplatingModel.AllInterfaces.Select(x => x.Name).Should().BeEquivalentTo("IEnumerable<>", "IEnumerable");
        }

        [Test]
        public void GenericTypeParameterLengthsAreRepresented()
        {
            TemplatingModel.Methods.Single(x => x.Name == "WillReturnAKeyValuePair").ReturnType.Name.Should().Be("KeyValuePair<,>");
        }
    }
}
