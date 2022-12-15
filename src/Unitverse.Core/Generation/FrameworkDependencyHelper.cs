namespace Unitverse.Core.Generation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    internal static class FrameworkDependencyHelper
    {
        public static void AddUsingStatements(ICompilationUnitStrategy strategy, IFrameworkSet frameworkSet, List<ClassModel> classModels)
        {
            strategy.AddUsing(Generate.UsingDirective("System"));
            strategy.AddUsings(frameworkSet.TestFramework.GetUsings());
            strategy.AddUsings(frameworkSet.AssertionFramework.GetUsings());
            if (frameworkSet.Context.MocksUsed)
            {
                strategy.AddUsings(frameworkSet.MockingFramework.GetUsings());
            }

            if (frameworkSet.Options.GenerationOptions.UseAutoFixture)
            {
                strategy.AddUsing(Generate.UsingDirective("AutoFixture"));

                if (frameworkSet.Options.GenerationOptions.CanUseAutoFixtureForMocking())
                {
                    switch (frameworkSet.Options.GenerationOptions.MockingFrameworkType)
                    {
                        case MockingFrameworkType.NSubstitute:
                            strategy.AddUsing(Generate.UsingDirective("AutoFixture.AutoNSubstitute"));
                            break;
                        case MockingFrameworkType.MoqAutoMock:
                        case MockingFrameworkType.Moq:
                            strategy.AddUsing(Generate.UsingDirective("AutoFixture.AutoMoq"));
                            break;
                        case MockingFrameworkType.FakeItEasy:
                            strategy.AddUsing(Generate.UsingDirective("AutoFixture.AutoFakeItEasy"));
                            break;
                    }
                }
            }

            foreach (var emittedType in frameworkSet.Context.EmittedTypes)
            {
                if (emittedType?.ContainingNamespace != null)
                {
                    strategy.AddUsing(Generate.UsingDirective(emittedType.ContainingNamespace.ToDisplayString()));
                }
            }

            if (classModels.SelectMany(x => x.Methods).Any(x => x.IsAsync))
            {
                strategy.AddUsing(Generate.UsingDirective(typeof(Task).Namespace));
            }

            if (!string.IsNullOrWhiteSpace(frameworkSet.Options.GenerationOptions.TestTypeBaseClass) &&
                !string.IsNullOrWhiteSpace(frameworkSet.Options.GenerationOptions.TestTypeBaseClassNamespace))
            {
                strategy.AddUsing(Generate.UsingDirective(frameworkSet.Options.GenerationOptions.TestTypeBaseClassNamespace));
            }
        }
    }
}
