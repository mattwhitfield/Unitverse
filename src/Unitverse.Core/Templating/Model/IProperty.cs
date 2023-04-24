namespace Unitverse.Core.Templating.Model
{
    public interface IProperty : ITemplateTarget
    {
        bool HasGet { get; }

        bool HasInit { get; }

        bool HasSet { get; }

        bool IsStatic { get; }

        IType? Type { get; }
    }
}
