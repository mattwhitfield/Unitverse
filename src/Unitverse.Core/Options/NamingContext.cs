namespace Unitverse.Core.Options
{
    public class NamingContext
    {
        public NamingContext(string typeName)
        {
            TypeName = typeName;
        }

        public string TypeName { get; }

        public string InterfaceName { get; private set; }

        public string MemberName { get; private set; }

        public string MemberBareName { get; private set; }

        public string ParameterName { get; private set; }

        public string TypeParameters { get; private set; }

        private NamingContext Clone()
        {
            var clone = new NamingContext(TypeName);
            clone.InterfaceName = InterfaceName;
            clone.MemberName = MemberName;
            clone.MemberBareName = MemberBareName;
            clone.ParameterName = ParameterName;
            clone.TypeParameters = TypeParameters;
            return clone;
        }

        public NamingContext WithMemberName(string name)
        {
            var clone = Clone();
            clone.MemberName = name;
            return clone;
        }

        public NamingContext WithMemberName(string name, string bareName)
        {
            var clone = Clone();
            clone.MemberName = name;
            clone.MemberBareName = bareName;
            return clone;
        }

        public NamingContext WithInterfaceName(string interfaceName)
        {
            var clone = Clone();
            clone.InterfaceName = interfaceName;
            return clone;
        }

        public NamingContext WithTypeParameters(string typeParameters)
        {
            var clone = Clone();
            clone.TypeParameters = typeParameters;
            return clone;
        }

        public NamingContext WithParameterName(string parameterName)
        {
            var clone = Clone();
            clone.ParameterName = parameterName;
            return clone;
        }
    }
}
