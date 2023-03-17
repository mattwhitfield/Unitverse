using System.Collections.Generic;
using System.Linq;

namespace Unitverse.Core.Tests
{
    public static class CartesianHelper
    {
        public static IList<object[]> CrossJoin(this object[] input, object[] newDimension)
        {
            List<object[]> output = new List<object[]>();
            foreach (var value in newDimension)
            {
                foreach (var inputValue in input)
                {
                    output.Add(new[] { inputValue, value });
                }
            }
            return output;
        }

        public static IList<object[]> CrossJoin(this IList<object[]> input, object[] newDimension)
        {
            List<object[]> output = new List<object[]>();
            foreach (var value in newDimension)
            {
                foreach (var row in input)
                {
                    output.Add(row.Concat(new[] { value }).ToArray());
                }
            }
            return output;
        }


        public static IList<object[]> CrossJoin(this IList<object[]> input, IList<object[]> newDimensions)
        {
            List<object[]> output = new List<object[]>();
            foreach (var newRow in newDimensions)
            {
                foreach (var row in input)
                {
                    output.Add(row.Concat(newRow).ToArray());
                }
            }
            return output;
        }

    }
}
