namespace Unitverse.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Strategies;
    using Unitverse.Core.Strategies.ClassLevelGeneration;
    using Unitverse.Core.Strategies.IndexerGeneration;
    using Unitverse.Core.Strategies.InterfaceGeneration;
    using Unitverse.Core.Strategies.MethodGeneration;
    using Unitverse.Core.Strategies.OperatorGeneration;
    using Unitverse.Core.Strategies.PropertyGeneration;
    using TechTalk.SpecFlow;

    [Binding]
    public class MethodBasedStrategySteps
    {
        private readonly MethodBasedStrategyContext _context;

        public MethodBasedStrategySteps(MethodBasedStrategyContext context)
        {
            _context = context;
        }

        [When(@"I generate unit tests for the class using strategy '(.*)'")]
        public void WhenIGenerateUnitTestsForTheClass(string strategy)
        {
            var options = new UnitTestGeneratorOptions(new GenerationOptions(_context.TargetFramework, _context.MockFramework));
            var frameworkSet = FrameworkSetFactory.Create(options);

            // TODO: Replace with an Argument Transformation: https://specflow.org/documentation/Step-Argument-Transformations/
            IGenerationStrategy<ClassModel> generationStrategy = null;
            if (strategy == "ComparableGenerationStrategy")
            {
                generationStrategy = new ComparableGenerationStrategy(frameworkSet);
            }

            if (strategy == "NullParameterCheckConstructorGenerationStrategy")
            {
                generationStrategy = new NullParameterCheckConstructorGenerationStrategy(frameworkSet);
            }

            if (strategy == "StringParameterCheckConstructorGenerationStrategy")
            {
                generationStrategy = new StringParameterCheckConstructorGenerationStrategy(frameworkSet);
            }

            if (strategy == "CanConstructMultiConstructorGenerationStrategy")
            {
                generationStrategy = new CanConstructMultiConstructorGenerationStrategy(frameworkSet);
            }

            if (strategy == "CanConstructSingleConstructorGenerationStrategy")
            {
                generationStrategy = new CanConstructSingleConstructorGenerationStrategy(frameworkSet);
            }

            if (strategy == "CanConstructNoConstructorGenerationStrategy")
            {
                generationStrategy = new CanConstructNoConstructorGenerationStrategy(frameworkSet);
            }

            if (strategy == "EnumerableGenerationStrategy")
            {
                generationStrategy = new EnumerableGenerationStrategy(frameworkSet);
            }

            if (generationStrategy == null)
            {
                throw new InvalidOperationException();
            }

            if (generationStrategy.CanHandle(_context.ClassModel, _context.ClassModel))
            {
                _context.Result = generationStrategy.Create(_context.ClassModel, _context.ClassModel);
            }

            SemanticModelHelper.WriteMethods(_context.Result);
        }

        [When(@"I generate tests for the method using the strategy '(.*)'")]
        public void WhenIGenerateTestsForTheMethod(string strategy)
        {
            var options = new UnitTestGeneratorOptions(new GenerationOptions(_context.TargetFramework, _context.MockFramework));
            var frameworkSet = FrameworkSetFactory.Create(options);

            IGenerationStrategy<IMethodModel> generationStrategy = null;
            if (strategy == "NullParameterCheckMethodGenerationStrategy")
            {
                generationStrategy = new NullParameterCheckMethodGenerationStrategy(frameworkSet);
            }

            if (strategy == "CanCallMethodGenerationStrategy")
            {
                generationStrategy = new CanCallMethodGenerationStrategy(frameworkSet);
            }

            if (strategy == "StringParameterCheckMethodGenerationStrategy")
            {
                generationStrategy = new StringParameterCheckMethodGenerationStrategy(frameworkSet);
            }

            if (strategy == "MappingMethodGenerationStrategy")
            {
                generationStrategy = new MappingMethodGenerationStrategy(frameworkSet);
            }

            if (generationStrategy == null)
            {
                throw new InvalidOperationException();
            }

            _context.Result = _context.ClassModel.Methods.GetMethodList(
                method => generationStrategy.CanHandle(method, _context.ClassModel),
                method => generationStrategy.Create(method, _context.ClassModel));
        }

        [When(@"I generate tests for the indexer using the strategy '(.*)'")]
        public void WhenIGenerateTestsForTheIndexer(string strategy)
        {
            var options = new UnitTestGeneratorOptions(new GenerationOptions(_context.TargetFramework, _context.MockFramework));
            var frameworkSet = FrameworkSetFactory.Create(options);

            IGenerationStrategy<IIndexerModel> generationStrategy = null;
            if (strategy == "ReadOnlyIndexerGenerationStrategy")
            {
                generationStrategy = new ReadOnlyIndexerGenerationStrategy(frameworkSet);
            }

            if (strategy == "WriteOnlyIndexerGenerationStrategy")
            {
                generationStrategy = new WriteOnlyIndexerGenerationStrategy(frameworkSet);
            }

            if (strategy == "ReadWriteIndexerGenerationStrategy")
            {
                generationStrategy = new ReadWriteIndexerGenerationStrategy(frameworkSet);
            }

            if (generationStrategy == null)
            {
                throw new InvalidOperationException();
            }

            _context.Result = _context.ClassModel.Indexers.GetMethodList(
                indexer => generationStrategy.CanHandle(indexer, _context.ClassModel),
                indexer => generationStrategy.Create(indexer, _context.ClassModel));
        }

        [When(@"I generate tests for the operator using the strategy '(.*)'")]
        public void WhenIGenerateTestsForTheOperator(string strategy)
        {
            var options = new UnitTestGeneratorOptions(new GenerationOptions(_context.TargetFramework, _context.MockFramework));
            var frameworkSet = FrameworkSetFactory.Create(options);

            IGenerationStrategy<IOperatorModel> generationStrategy = null;
            if (strategy == "NullParameterCheckOperatorGenerationStrategy")
            {
                generationStrategy = new NullParameterCheckOperatorGenerationStrategy(frameworkSet);
            }

            if (strategy == "CanCallOperatorGenerationStrategy")
            {
                generationStrategy = new CanCallOperatorGenerationStrategy(frameworkSet);
            }

            if (generationStrategy == null)
            {
                throw new InvalidOperationException();
            }
            
            _context.Result = _context.ClassModel.Operators.GetMethodList(
                methodOperator => generationStrategy.CanHandle(methodOperator, _context.ClassModel),
                methodOperator => generationStrategy.Create(methodOperator, _context.ClassModel));
        }

        [When(@"I generate tests for the property using the strategy '(.*)'")]
        public void WhenIGenerateTestsForTheProperty(string strategy)
        {
            var options = new UnitTestGeneratorOptions(new GenerationOptions(_context.TargetFramework, _context.MockFramework));
            var frameworkSet = FrameworkSetFactory.Create(options);

            IGenerationStrategy<IPropertyModel> generationStrategy = null;
            if (strategy == "ReadOnlyPropertyGenerationStrategy")
            {
                generationStrategy = new ReadOnlyPropertyGenerationStrategy(frameworkSet);
            }

            if (strategy == "WriteOnlyPropertyGenerationStrategy")
            {
                generationStrategy = new WriteOnlyPropertyGenerationStrategy(frameworkSet);
            }

            if (strategy == "ReadWritePropertyGenerationStrategy")
            {
                generationStrategy = new ReadWritePropertyGenerationStrategy(frameworkSet);
            }

            if (strategy == "NotifyPropertyChangedGenerationStrategy")
            {
                generationStrategy = new NotifyPropertyChangedGenerationStrategy(frameworkSet);
            }

            if (strategy == "SingleConstructorInitializedPropertyGenerationStrategy")
            {
                generationStrategy = new SingleConstructorInitializedPropertyGenerationStrategy(frameworkSet);
            }

            if (strategy == "MultiConstructorInitializedPropertyGenerationStrategy")
            {
                generationStrategy = new MultiConstructorInitializedPropertyGenerationStrategy(frameworkSet);
            }

            if (generationStrategy == null)
            {
                throw new InvalidOperationException();
            }

            _context.Result = _context.ClassModel.Properties.GetMethodList(
                property => generationStrategy.CanHandle(property, _context.ClassModel),
                property => generationStrategy.Create(property, _context.ClassModel));
        }

        [Then (@"I expect a method called '(.*)'")]
        public void ThenIExpectMethod(string methodName)
        {
            var isThere = _context.Result.FindMatches(
                method => method.Identifier.ValueText,
                methodName,
                out var found,
                out var foundItem);
            
            _context.CurrentMethod = foundItem;

            Assert.IsTrue(isThere, "Expected to find method '{0}', found methods '{1}'", methodName, found.Aggregate((x, y) => x + ", " + y));
        }

        [Then(@"I expect it to contain the statement '(.*)'")]
        public void ThenIExpectStatement(string methodStatement)
        {
            methodStatement = SemanticModelHelper.RemoveSpaces(methodStatement);

            var isThere = _context.CurrentMethod.Body.Statements.FindMatches(
                statement => statement.ToString(),
                methodStatement,
                out var found,
                out _);

            Assert.IsTrue(isThere, "Expected to find statement '{0}', found statements '{1}'", methodStatement, found.Aggregate((x, y) => x + ", " + y));
        }

        [Then(@"I expect it to contain a statement like '(.*)'")]
        public void ThenIExpectStatementLike(string methodStatement)
        {
            methodStatement = SemanticModelHelper.RemoveSpaces(methodStatement);
            string updateMethodStatement = Regex.Escape(methodStatement).Replace("\\{\\{\\{AnyString}}}", "(.+)").Replace("\\{\\{\\{AnyInteger}}}", "(\\d+)");
            var matcher = new Regex(updateMethodStatement);

            var isThere = _context.CurrentMethod.Body.Statements.FindMatches(
                statement => matcher.IsMatch(statement.ToString()),
                out var foundStatements);

            var found = new List<string>();

            foreach (var foundStatement in foundStatements)
            {
                    found.Add(foundStatement.ToString());
            }

            found.RemoveAll(item => item == "none");
            
            if (!found.Any())
            {
                found.Add("none");
            }

            Assert.True(isThere, "Expected to find statement like '{0}', found statements '{1}'", methodStatement, found.Aggregate((x, y) => x + ", " + y));
        }

        [Then(@"I expect it to contain the variable '(.*)'")]
        public void ThenIExpectVariable(string methodVariable)
        {
            var isThere = false;
            var found = new List<string>();
            
            foreach (var variable in _context.CurrentMethod.Body.Statements.OfType<LocalDeclarationStatementSyntax>())
            {
                isThere = variable.Declaration.Variables.Select(x => x.Identifier.ValueText).ToList().FindMatches(variableName => variableName,
                    methodVariable,
                    out var foundList,
                    out _);

                if (isThere)
                {
                    break;
                }

                found.AddRange(foundList);
            }

            found.RemoveAll(item => item == "none");

            if (!found.Any())
            {
                found.Add("None");
            }

            Assert.True(isThere, "Expected to find variable '{0}', found variables '{1}'", methodVariable, found.Aggregate((x, y) => x + ", " + y));
        }

        [Then(@"I expect it to contain (\d*) statements called '(.*)'")]
        public void ThenIExpectSameStatements(int numberStatements, string methodStatement)
        {
            int numberFound = 0;
            bool isThere = false;
            var found = new List<string>();
            methodStatement = SemanticModelHelper.RemoveSpaces(methodStatement);

            foreach (var statement in _context.CurrentMethod.Body.Statements)
            {
                if (statement.ToString() == methodStatement)
                {
                    isThere = true;
                    numberFound += 1;
                }
                
                found.Add(statement.ToString());
            }

            if (!found.Any())
            {
                found.Add("none");
            }
            
            if (!isThere)
            {
                Assert.Fail("Expected to find statement '{0}', found statements '{1}'", methodStatement, found.Aggregate((x, y) => x + ", " + y));
            }
            
            Assert.That(numberFound, Is.EqualTo(numberStatements), "Expected '{0}' statements '{1}', found '{2}'", numberStatements, methodStatement, numberFound);
        }

        [Then(@"I expect no method with a name like '(.*)'")]
        public void ThenIExpectNotMethod(string methodName)
        { 
            var matcher = new Regex(methodName);
            var isThere = _context.Result.FindMatches(
                method => matcher.IsMatch(method.Identifier.ValueText), 
                out _);
            
            Assert.IsTrue(!isThere, "Expected no method called '{0}', found none", methodName);
        }

        [Then(@"I expect it to contain a using statement with a '(.*)' token")]
        public void ThenIExpectUsingWithToken(string usingToken)
        {
            bool isThere = false;
            var found = new List<String>();
            
            if (!_context.CurrentMethod.Body.Statements.OfType<UsingStatementSyntax>().Any())
            {
                Assert.Fail("Expected a using statement, found none");
            }

            foreach (var statement in _context.CurrentMethod.Body.Statements.OfType<UsingStatementSyntax>())
            {
                isThere = statement.Statement.DescendantTokens().FindMatches(token => token.ToString(),
                   usingToken,
                   out var foundTokens,
                   out _);

                if (isThere)
                {
                    break;
                }

                found.AddRange(foundTokens);
            }

            found.RemoveAll(item => item == "none");

            if (!found.Any())
            {
                found.Add("none");
            }

            Assert.IsTrue(isThere, "Found a using statement, expected it to contain a '{0}' token, in contained tokens '{1}'", usingToken, found.Aggregate((x, y) => x + ", " + y));
        }
        
        [Then(@"I expect methods with statements like:")]
        public void ThenIExpectMethodsWithStatementsLike(Table table)
        {
            foreach (var row in table.Rows)
            {
                string methodName = row.First().Value;
                var methodThere = _context.Result.FindMatches(
                    method => method.Identifier.ValueText,
                    methodName,
                    out var found,
                    out var methodFound);
                
                if (!methodThere)
                {
                    Assert.Fail("Expected to find method '{0}', found methods '{1}'", methodName, found.Aggregate((x, y) => x + ", " + y));
                }

                string methodStatement = row.Last().Value;
                methodStatement = SemanticModelHelper.RemoveSpaces(methodStatement);
                string updateMethodStatement = Regex.Escape(methodStatement).Replace("\\{\\{\\{AnyString}}}", "(.+)").Replace("\\{\\{\\{AnyInteger}}}", "(\\d+)");
                var matcher = new Regex(updateMethodStatement);

                var isThere = methodFound.Body.Statements.FindMatches(
                statement => matcher.IsMatch(statement.ToString()),
                out var statementsFound
                );

                var foundStatements = new List<string>();
                foreach (var foundStatement in statementsFound)
                {
                        foundStatements.Add(foundStatement.ToString());
                }

                foundStatements.RemoveAll(item => item == "none");

                if (!foundStatements.Any())
                {
                    foundStatements.Add("none");
                }

                Assert.True(isThere, "Expected to find statement like '{0}', found statements '{1}'", methodStatement, foundStatements.Aggregate((x, y) => x + ", " + y));
            }
        }
    }
}