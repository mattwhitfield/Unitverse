namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Frameworks.Mocking;
    using Unitverse.Core.Models;

    public static class MockHelper
    {
        public static bool PrepareMockCalls(ClassModel model, CSharpSyntaxNode targetBody, ExpressionSyntax? propertyAccess, IList<ISymbol> interfaceMethodsImplemented, IEnumerable<string> parameterNames, IFrameworkSet frameworkSet, out List<StatementSyntax> mockSetupStatements, out List<StatementSyntax> mockAssertionStatements)
        {
            mockSetupStatements = new List<StatementSyntax>();
            mockAssertionStatements = new List<StatementSyntax>();

            if (!frameworkSet.Options.GenerationOptions.AutomaticallyConfigureMocks)
            {
                return false;
            }

            var mappedInterfaceFields = model.DependencyMap.MappedInterfaceFields.ToList();
            var dependencyMap = InvocationExtractor.ExtractFrom(targetBody, model.SemanticModel, mappedInterfaceFields);

            foreach (var field in mappedInterfaceFields)
            {
                var fieldType = model.DependencyMap.GetTypeSymbolFor(field);
                if (fieldType != null)
                {
                    var constructorParameters = model.DependencyMap.GetConstructorParametersFor(field).ToList();
                    foreach (var constructorParameter in constructorParameters)
                    {
                        var mockFieldName = model.GetConstructorParameterFieldName(constructorParameter, frameworkSet);
                        var accessedMethodSymbols = dependencyMap.GetAccessedMethodSymbolsFor(field).ToList();

                        // plain interface redirection effectively means that we are in a call where there is only one statement inside, and that
                        // statement is a method call to the same interface definition as the method we're testing. So we aim to emit tests that
                        // by default test delegation to the injected interface method
                        var isPlainInterfaceMethodRedirection =
                            constructorParameters.Count == 1 &&
                            accessedMethodSymbols.Count == 1 &&
                            accessedMethodSymbols.SequenceEqual(interfaceMethodsImplemented) &&
                            dependencyMap.MemberAccessCount == 0 &&
                            dependencyMap.InvocationCount == 1;

                        foreach (var dependencyMethod in accessedMethodSymbols)
                        {
                            if (dependencyMethod.ContainingType == fieldType)
                            {
                                var returnType = dependencyMethod.ReturnType;
                                var isAsync = dependencyMethod.IsAsyncCallable();
                                var isPlainTaskReturnType = isAsync && ((INamedTypeSymbol)returnType).TypeArguments.Length == 0;

                                var hasReturnType = !dependencyMethod.ReturnsVoid && !isPlainTaskReturnType;
                                var parameters = Enumerable.Empty<string>();

                                if (hasReturnType)
                                {
                                    returnType = MockingHelper.ReduceAsyncReturnType(returnType);
                                    var expectedReturnValue = AssignmentValueHelper.GetDefaultAssignmentValue(returnType, model.SemanticModel, frameworkSet);
                                    if (isPlainInterfaceMethodRedirection)
                                    {
                                        // if we are in plainInterface mode - we add a mockSetupStatement to declare expectedReturnValue and assign it
                                        mockSetupStatements.Add(Generate.VariableDeclaration(returnType, frameworkSet, "expectedReturnValue", expectedReturnValue));
                                        expectedReturnValue = SyntaxFactory.IdentifierName("expectedReturnValue");
                                        parameters = parameterNames;
                                    }

                                    mockSetupStatements.Add(Generate.Statement(frameworkSet.MockingFramework.GetSetupFor(dependencyMethod, mockFieldName, model.SemanticModel, frameworkSet, expectedReturnValue, parameters)));
                                }

                                var assertion = frameworkSet.MockingFramework.GetAssertionFor(dependencyMethod, mockFieldName, model.SemanticModel, frameworkSet, parameters);
                                if (isAsync && frameworkSet.MockingFramework.AwaitAsyncAssertions)
                                {
                                    assertion = SyntaxFactory.AwaitExpression(assertion);
                                }

                                mockAssertionStatements.Add(Generate.Statement(assertion));

                                if (isPlainInterfaceMethodRedirection && hasReturnType)
                                {
                                    // add assert to check that 'result' is the same as 'expectedReturnValue'
                                    mockAssertionStatements.Add(frameworkSet.AssertionFramework.AssertEqual(SyntaxFactory.IdentifierName("result"), SyntaxFactory.IdentifierName("expectedReturnValue"), returnType.IsReferenceTypeAndNotString()));
                                }
                            }
                        }

                        var accessedPropertySymbols = dependencyMap.GetAccessedPropertySymbolsFor(field).ToList();

                        var isPlainInterfacePropertyRedirection =
                            constructorParameters.Count == 1 &&
                            accessedPropertySymbols.Count == 1 &&
                            accessedPropertySymbols.SequenceEqual(interfaceMethodsImplemented) &&
                            dependencyMap.MemberAccessCount == 1 &&
                            dependencyMap.InvocationCount == 0;

                        foreach (var dependencyProperty in dependencyMap.GetAccessedPropertySymbolsFor(field))
                        {
                            if (dependencyProperty.ContainingType == fieldType)
                            {
                                var expectedReturnValue = AssignmentValueHelper.GetDefaultAssignmentValue(dependencyProperty.Type, model.SemanticModel, frameworkSet);

                                if (isPlainInterfacePropertyRedirection)
                                {
                                    // if we are in plainInterface mode - we add a mockSetupStatement to declare expectedValue and assign it
                                    mockSetupStatements.Add(Generate.VariableDeclaration(dependencyProperty.Type, frameworkSet, "expectedValue", expectedReturnValue));
                                    expectedReturnValue = SyntaxFactory.IdentifierName("expectedValue");
                                }

                                mockSetupStatements.Add(Generate.Statement(frameworkSet.MockingFramework.GetSetupFor(dependencyProperty, mockFieldName, model.SemanticModel, frameworkSet, expectedReturnValue)));

                                if (isPlainInterfacePropertyRedirection && propertyAccess != null)
                                {
                                    // add assert to check that the property is the same as 'expectedValue'
                                    mockAssertionStatements.Add(frameworkSet.AssertionFramework.AssertEqual(propertyAccess, SyntaxFactory.IdentifierName("expectedValue"), dependencyProperty.Type.IsReferenceTypeAndNotString()));
                                }
                            }
                        }

                        if (isPlainInterfaceMethodRedirection || isPlainInterfacePropertyRedirection)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
