namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;

    public class ConstructorFieldAssignmentExtractor : CSharpSyntaxWalker
    {
        private ConstructorFieldAssignmentExtractor()
        {
        }

        private readonly Dictionary<string, HashSet<ParameterModel>> _setFields = new Dictionary<string, HashSet<ParameterModel>>();

        private readonly HashSet<ParameterModel> _parameters = new HashSet<ParameterModel>(new ParameterModelComparer());
        private readonly Dictionary<string, ITypeSymbol> _fieldTypes = new Dictionary<string, ITypeSymbol>();

        public static ClassDependencyMap ExtractMapFrom(TypeDeclarationSyntax classDeclaration, SemanticModel model)
        {
            if (classDeclaration is null)
            {
                throw new ArgumentNullException(nameof(classDeclaration));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var extractor = new ConstructorFieldAssignmentExtractor();

            foreach (var field in classDeclaration.Members.OfType<FieldDeclarationSyntax>())
            {
                var info = model.GetTypeInfo(field.Declaration.Type);
                if (info.Type != null)
                {
                    foreach (var declaration in field.Declaration.Variables)
                    {
                        extractor._fieldTypes[declaration.Identifier.Text] = info.Type;
                    }
                }
            }

            foreach (var constructor in classDeclaration.Members.OfType<ConstructorDeclarationSyntax>())
            {
                extractor._parameters.Clear();

                foreach (var parameter in constructor.ParameterList.Parameters)
                {
                    var typeModel = model.GetDeclaredSymbol(parameter);
                    if (typeModel != null && parameter.Type != null)
                    {
                        var typeInfo = model.GetTypeInfo(parameter.Type);

                        var parameterModel = new ParameterModel(parameter.Identifier.Text, parameter, typeModel.ToDisplayString(), typeInfo);

                        extractor._parameters.Add(parameterModel);
                    }
                }

                constructor.Accept(extractor);
            }

            return new ClassDependencyMap(extractor._setFields, extractor._fieldTypes);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            base.VisitAssignmentExpression(node);

            IdentifierNameSyntax? identifier = null;

            if (node.Left is MemberAccessExpressionSyntax memberAccess && memberAccess.Expression is ThisExpressionSyntax && memberAccess.Name is IdentifierNameSyntax identifierSyntax)
            {
                identifier = identifierSyntax;
            }
            else if (node.Left is IdentifierNameSyntax identifierSyntax2)
            {
                identifier = identifierSyntax2;
            }

            if (identifier != null)
            {
                var field = identifier.Identifier.Text;

                // if the assignments left hand side is a field
                if (_fieldTypes.ContainsKey(field))
                {
                    // find all the identifiers in the assignment's right hand side where it matches a parameter name
                    var identifierNames = IdentifierNameExtractor.ExtractFrom(node.Right).ToList();

                    if (identifierNames.Count > 0)
                    {
                        if (!_setFields.TryGetValue(field, out var set))
                        {
                            _setFields[field] = set = new HashSet<ParameterModel>(new ParameterModelComparer());
                        }

                        foreach (var identifierName in identifierNames)
                        {
                            var parameter = _parameters.FirstOrDefault(x => x.Name == identifierName);
                            if (parameter != null)
                            {
                                set.Add(parameter);
                            }
                        }
                    }
                }
            }
        }
    }
}
