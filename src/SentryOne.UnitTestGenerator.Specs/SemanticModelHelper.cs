namespace SentryOne.UnitTestGenerator.Specs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;

    internal static class SemanticModelHelper
    {
        public static readonly Lazy<List<MetadataReference>> References = new Lazy<List<MetadataReference>>(CreateReferences);

        private static List<MetadataReference> CreateReferences()
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            return new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(INotifyPropertyChanged).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Stream).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
            };
        }

        public static string RemoveSpaces(string source)
        {
            bool inString = false;
            var builder = new StringBuilder();
            foreach (char ch in source)
            {
                if (ch == ' ')
                {
                    if (inString)
                    {
                        builder.Append(' ');
                    }
                }
                else
                {
                    builder.Append(ch);
                    if (ch == '"')
                    {
                        inString = !inString;
                    }
                }
            }
            return builder.ToString();
        }

        public static bool FindMatches<T>(this IEnumerable<T> list, Func<T, bool> match, out IList<T> found)
        {
            bool isThere = false;
            found = new List<T>();

            foreach (var item in list)
            {
                if (match(item))
                {
                    isThere = true;
                    break;
                }
                found.Add(item);
            }

            return isThere;
        }

        public static bool FindMatches<T>(this IEnumerable<T> list, Func<T, string> convert, string lookingFor, out IList<string> found, out T foundItem)
        {
            bool isThere = false;
            found = new List<string>();
            foundItem = default(T);

            foreach (var item in list)
            {
                var stringRepresentation = convert(item);
                if (stringRepresentation == lookingFor)
                {
                    isThere = true;
                    foundItem = item;
                    break;
                }
                found.Add(stringRepresentation);
            }

            if (!found.Any())
            {
                found.Add("none");
            }

            return isThere;
        }

        public static void WriteMethods(IEnumerable<MethodDeclarationSyntax> methodList)
        {
            using (var workspace = new AdhocWorkspace())
            {
                foreach (var method in methodList)
                {
                    Console.WriteLine(Formatter.Format(method, workspace).ToString());
                }
            }
        }

        public static IEnumerable<MethodDeclarationSyntax> GetMethodList<T>(this IEnumerable<T> list, Func<T, bool> canHandle, Func<T, IEnumerable<MethodDeclarationSyntax>> generate)

        {
            var methodList = new List<MethodDeclarationSyntax>();

            foreach (var item in list)
            {
                if (canHandle(item))
                {
                    methodList.AddRange(generate(item));
                }
            }

            SemanticModelHelper.WriteMethods(methodList);
            return methodList;
        }
    }
}
