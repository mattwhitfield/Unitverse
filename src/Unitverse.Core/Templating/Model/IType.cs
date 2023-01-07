namespace Unitverse.Core.Templating.Model
{
    public interface IType
    {
        string FullName { get; }

        string Name { get; }

        string Namespace { get; }
    }
}
