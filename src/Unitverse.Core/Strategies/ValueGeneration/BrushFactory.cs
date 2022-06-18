namespace Unitverse.Core.Strategies.ValueGeneration
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;

    public static class BrushFactory
    {
        private static readonly string[] ColorsArray = { "Red", "Orange", "Yellow", "Green", "Blue", "Purple", "Black", "Brown", "Cyan", "Magenta", "Gold", "White", "Teal", "Pink", "Lime" };

        public static Func<ExpressionSyntax> Brushes => () => RandomBrush("Brushes");

        public static Func<ExpressionSyntax> Color => () => RandomBrush("Color");

        public static Func<ExpressionSyntax> Colors => () => RandomBrush("Colors");

        private static ExpressionSyntax RandomBrush(string typeName)
        {
            var identifier = ColorsArray[ValueGenerationStrategyFactory.Random.Next(ColorsArray.Length)];

            return Generate.MemberAccess(typeName, identifier);
        }
    }
}