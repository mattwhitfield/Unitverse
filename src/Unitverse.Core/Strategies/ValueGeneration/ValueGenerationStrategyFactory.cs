namespace Unitverse.Core.Strategies.ValueGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;

    public static class ValueGenerationStrategyFactory
    {
        internal static Random Random { get; private set; } = new Random();

        internal static bool PredictableGeneration { get; private set; }

        public static void UsePredictableGeneration(int seed)
        {
            PredictableGeneration = true;
            Random = new Random(seed);
        }

        private static IEnumerable<IValueGenerationStrategy> Strategies =>
            new IValueGenerationStrategy[]
            {
                new SimpleValueGenerationStrategy(() => Generate.Literal("TestValue" + Random.Next(int.MaxValue)), "string"),
                new SimpleValueGenerationStrategy(() => Generate.Literal(Random.Next(int.MaxValue)), "int", "int?"),
                new SimpleValueGenerationStrategy(() => Generate.Literal((long)Random.Next(int.MaxValue)), "long", "long?"),
                new SimpleValueGenerationStrategy(() => CastedLiteral(Random.Next(int.MaxValue), SyntaxKind.UIntKeyword), "uint", "uint?"),
                new SimpleValueGenerationStrategy(() => CastedLiteral(Random.Next(int.MaxValue), SyntaxKind.ULongKeyword), "ulong", "ulong?"),
                new SimpleValueGenerationStrategy(() => Generate.Literal((decimal)GenerateDouble()), "decimal", "decimal?"),
                new SimpleValueGenerationStrategy(() => CastedLiteral(Random.Next(short.MaxValue), SyntaxKind.ShortKeyword), "short", "short?"),
                new SimpleValueGenerationStrategy(() => CastedLiteral(Random.Next(ushort.MaxValue), SyntaxKind.UShortKeyword), "ushort", "ushort?"),
                new SimpleValueGenerationStrategy(() => CastedLiteral(Random.Next(byte.MaxValue), SyntaxKind.ByteKeyword), "byte", "byte?"),
                new SimpleValueGenerationStrategy(() => CastedLiteral(Random.Next(sbyte.MaxValue), SyntaxKind.SByteKeyword), "sbyte", "sbyte?"),
                new SimpleValueGenerationStrategy(() => Generate.ObjectCreation(SyntaxFactory.IdentifierName("Guid"), Generate.Literal(GetGuid().ToString())), "System.Guid", "System.Guid?"),
                new SimpleValueGenerationStrategy(() => Generate.PropertyAccess(SyntaxFactory.IdentifierName("DateTime"), "UtcNow"), "System.DateTime", "System.DateTime?"),
                new SimpleValueGenerationStrategy(() => Generate.PropertyAccess(SyntaxFactory.IdentifierName("DateTimeOffset"), "UtcNow"), "System.DateTimeOffset", "System.DateTimeOffset?"),
                new SimpleValueGenerationStrategy(() => Generate.Literal(GenerateDouble()), "double", "double?"),
                new SimpleValueGenerationStrategy(() => Generate.Literal((float)(Random.NextDouble() * short.MaxValue)), "float", "float?"),
                new SimpleValueGenerationStrategy(() => (Random.Next(int.MaxValue) % 2) > 0 ? Generate.Literal(true) : Generate.Literal(false), "bool", "bool?"),
                new SimpleValueGenerationStrategy(() => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("CultureInfo"), SyntaxFactory.IdentifierName((Random.Next(int.MaxValue) % 2) > 0 ? "CurrentCulture" : "InvariantCulture")), "System.Globalization.CultureInfo"),
                new SimpleValueGenerationStrategy(() => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("CancellationToken"), SyntaxFactory.IdentifierName("None")), "System.Threading.CancellationToken"),
                new SimpleValueGenerationStrategy(ArrayFactory.Byte, "byte[]"),
                new TypedValueGenerationStrategy(EnumFactory.Random, "System.Enum"),
                new SimpleValueGenerationStrategy(() => Generate.ObjectCreation(SyntaxFactory.IdentifierName("DefaultHttpContext")), "Microsoft.AspNetCore.Http.HttpContext"),
                new SimpleValueGenerationStrategy(() => Generate.ObjectCreation(SyntaxFactory.IdentifierName("MemoryStream")), "System.IO.Stream"),
                new TypedValueGenerationStrategy(ArrayFactory.ImplicitlyTyped, "System.Collections.Generic.IEnumerable"),
                new TypedValueGenerationStrategy(ArrayFactory.ImplicitlyTyped, "System.Collections.Generic.IList"),
                new TypedValueGenerationStrategy(ArrayFactory.ImplicitlyTypedArray, "System.Array"),
                new SimpleValueGenerationStrategy(BrushFactory.Brushes, "System.Drawing.Brush"),
                new SimpleValueGenerationStrategy(BrushFactory.Brushes, "System.Windows.Media.Brush"),
                new SimpleValueGenerationStrategy(BrushFactory.Color, "System.Drawing.Color"),
                new SimpleValueGenerationStrategy(BrushFactory.Colors, "System.Windows.Media.Color"),
            };

        private static double GenerateDouble()
        {
            var doubleValue = Random.NextDouble() * int.MaxValue * 0.99d;

            if (doubleValue - Math.Floor(doubleValue) < 0.01)
            {
                doubleValue += 0.5;
            }

            return doubleValue;
        }

        private static Guid GetGuid()
        {
            if (PredictableGeneration)
            {
                var array = new byte[16];
                Random.NextBytes(array);
                return new Guid(array);
            }

            return Guid.NewGuid();
        }

        public static ExpressionSyntax GenerateFor(ITypeSymbol symbol, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet)
        {
            return GenerateFor(symbol.ToFullName(), symbol, model, visitedTypes, frameworkSet);
        }

        public static ExpressionSyntax GenerateFor(string typeName, ITypeSymbol symbol, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            var strategy = Strategies.FirstOrDefault(x => x.SupportedTypeNames.Any(t => string.Equals(t, typeName, StringComparison.OrdinalIgnoreCase)));
            if (strategy != null)
            {
                return strategy.CreateValueExpression(symbol, model, visitedTypes, frameworkSet);
            }

            var baseType = symbol.BaseType;
            while (baseType != null)
            {
                var name = baseType.ToFullName();
                strategy = Strategies.FirstOrDefault(x => x.SupportedTypeNames.Any(t => string.Equals(t, name, StringComparison.OrdinalIgnoreCase)));
                if (strategy != null)
                {
                    return strategy.CreateValueExpression(symbol, model, visitedTypes, frameworkSet);
                }

                baseType = baseType.BaseType;
            }

            return null;
        }

        public static bool IsSupported(ITypeSymbol symbol)
        {
            var current = symbol;
            while (current != null)
            {
                var fullName = current.ToFullName();
                var isSupported = Strategies.Any(x => x.SupportedTypeNames.Any(t => string.Equals(t, fullName, StringComparison.OrdinalIgnoreCase)));
                if (isSupported)
                {
                    return true;
                }

                current = current.BaseType;
            }

            return false;
        }

        private static ExpressionSyntax CastedLiteral(object o, SyntaxKind typeKeyword)
        {
            return SyntaxFactory.CastExpression(SyntaxFactory.PredefinedType(SyntaxFactory.Token(typeKeyword)), Generate.Literal(o));
        }
    }
}