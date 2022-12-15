namespace Unitverse.Core.Generation
{
    using System.Threading.Tasks;

    internal interface ICompilationUnitStrategyBootstrapper
    {
        Task<ICompilationUnitStrategy> Initialize();
    }
}
