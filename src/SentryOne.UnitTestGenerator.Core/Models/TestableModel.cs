namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System;
    using Microsoft.CodeAnalysis;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public abstract class TestableModel<T> : ITestableModel<T>
        where T : SyntaxNode
    {
        protected TestableModel(string name, T node)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            OriginalName = Name = name;
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public string Name { get; private set; }

        public T Node { get; }

        public string OriginalName { get; private set; }

        public void MutateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentNullException(nameof(newName));
            }

            if (!string.Equals(OriginalName, Name, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(Strings.TestableModel_MutateName_Cannot_mutate_name_more_than_once);
            }

            OriginalName = Name;
            Name = newName;
        }
    }
}