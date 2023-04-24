namespace Unitverse.Core.Templating.Model
{
    public interface IType : INameProvider
    {
        string FullName { get; }

        string Namespace { get; }
    }
}
