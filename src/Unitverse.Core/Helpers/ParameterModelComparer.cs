namespace Unitverse.Core.Helpers
{
    using System.Collections.Generic;
    using Unitverse.Core.Models;

    public class ParameterModelComparer : IEqualityComparer<ParameterModel>
    {
        public bool Equals(ParameterModel x, ParameterModel y)
        {
            if (x is null)
            {
                return y is null;
            }

            if (y is null)
            {
                return false;
            }

            return x.Name.Equals(y.Name) && x.TypeInfo.Equals(y.TypeInfo);
        }

        public int GetHashCode(ParameterModel obj)
        {
            return obj.Name.GetHashCode() ^ obj.TypeInfo.GetHashCode();
        }
    }
}
