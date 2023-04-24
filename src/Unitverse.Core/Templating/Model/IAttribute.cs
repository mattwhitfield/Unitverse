namespace Unitverse.Core.Templating.Model
{
    public interface IAttribute : INameProvider
    {
        public IType Type { get; }
    }
}
