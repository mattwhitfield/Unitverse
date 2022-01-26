namespace Unitverse.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    public class DependencyAccessMap
    {
        private Dictionary<string, HashSet<IMethodSymbol>> _methodCalls = new Dictionary<string, HashSet<IMethodSymbol>>();
        private Dictionary<string, HashSet<IPropertySymbol>> _propertyCalls = new Dictionary<string, HashSet<IPropertySymbol>>();

        public DependencyAccessMap(IEnumerable<Tuple<IMethodSymbol, string>> methodCalls, IEnumerable<Tuple<IPropertySymbol, string>> propertyCalls, int invocationCount, int memberAccessCount)
        {
            if (methodCalls is null)
            {
                throw new ArgumentNullException(nameof(methodCalls));
            }

            if (propertyCalls is null)
            {
                throw new ArgumentNullException(nameof(propertyCalls));
            }

            foreach (var method in methodCalls)
            {
                if (!_methodCalls.TryGetValue(method.Item2, out var set))
                {
                    _methodCalls[method.Item2] = set = new HashSet<IMethodSymbol>();
                }

                set.Add(method.Item1);
            }

            foreach (var property in propertyCalls)
            {
                if (!_propertyCalls.TryGetValue(property.Item2, out var set))
                {
                    _propertyCalls[property.Item2] = set = new HashSet<IPropertySymbol>();
                }

                set.Add(property.Item1);
            }

            InvocationCount = invocationCount;
            MemberAccessCount = memberAccessCount;
        }

        public int InvocationCount { get; }

        public int MemberAccessCount { get; }

        public IEnumerable<IMethodSymbol> GetAccessedMethodSymbolsFor(string dependencyFieldName)
        {
            if (_methodCalls.TryGetValue(dependencyFieldName, out var set))
            {
                return set;
            }

            return Enumerable.Empty<IMethodSymbol>();
        }

        public IEnumerable<IPropertySymbol> GetAccessedPropertySymbolsFor(string dependencyFieldName)
        {
            if (_propertyCalls.TryGetValue(dependencyFieldName, out var set))
            {
                return set;
            }

            return Enumerable.Empty<IPropertySymbol>();
        }
    }
}
