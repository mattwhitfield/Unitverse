namespace Unitverse.Core.Models
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public interface IConstructorModel : ITestableModel<ConstructorDeclarationSyntax>
    {
        IList<ParameterModel> Parameters { get; }
    }
}