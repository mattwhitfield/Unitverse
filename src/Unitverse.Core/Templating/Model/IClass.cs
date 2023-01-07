namespace Unitverse.Core.Templating.Model
{
    using System.Collections.Generic;

    public interface IClass
    {
        IEnumerable<IType> AllInterfaces { get; }

        IEnumerable<IAttribute> Attributes { get; }

        IEnumerable<IType> BaseTypes { get; }

        IEnumerable<IType> Interfaces { get; }

        IEnumerable<IMethod> Methods { get; }

        IEnumerable<IConstructor> Constructors { get; }

        IEnumerable<IProperty> Properties { get; }

        IType Type { get; }
    }
}
