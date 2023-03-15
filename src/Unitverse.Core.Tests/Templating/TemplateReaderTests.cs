namespace Unitverse.Core.Tests.Templating
{
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using FluentAssertions;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.IO;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating;

    [TestFixture]
    public static class TemplateReaderTests
    {
        private static IEnumerable<object[]> TestCases()
        {
            object[] testMethodNames = new object[] { "methodName1", "methodName2" };
            object[] targets = new object[] { "Method", "Property" };
            object[] includes = new string[] { "class.Type.Name == 'name'", "class.Type.Name == 'other'" };
            object[] excludes = new string[] { "", "model.Name == 'method'" };
            object[] booleans = new object[] { true, false };
            object[] descriptions = new object[] { "description 1", "description 2" };
            object[] priorities = new object[] { 1, 2 };

            return testMethodNames
                .CrossJoin(targets)
                .CrossJoin(includes)
                .CrossJoin(excludes)
                .CrossJoin(booleans) // isAsync
                .CrossJoin(booleans) // isStatic
                .CrossJoin(descriptions)
                .CrossJoin(booleans) // isExclusive
                .CrossJoin(booleans) // stopMatching
                .CrossJoin(priorities);
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public static void CanCallReadFromWithFileName(
            string testMethodName,
            string target,
            string include,
            string exclude,
            bool isAsync,
            bool isStatic,
            string description,
            bool isExclusive,
            bool stopMatching,
            int priority)
        {
            var testFile = Path.GetTempFileName();
            try
            {
                // Arrange
                using (var writer = new StreamWriter(testFile))
                {
                    writer.WriteLine(TemplateHeaders.TestMethodName + ": " + testMethodName);
                    writer.WriteLine(TemplateHeaders.Target + ": " + target);
                    writer.WriteLine(TemplateHeaders.Include + ": " + include);
                    if (!string.IsNullOrWhiteSpace(exclude))
                    {
                        writer.WriteLine(TemplateHeaders.Exclude + ": " + exclude);
                    }
                    writer.WriteLine(TemplateHeaders.IsAsync + ": " + isAsync);
                    writer.WriteLine(TemplateHeaders.IsStatic + ": " + isStatic);
                    writer.WriteLine(TemplateHeaders.Description + ": " + description);
                    writer.WriteLine(TemplateHeaders.IsExclusive + ": " + isExclusive);
                    writer.WriteLine(TemplateHeaders.StopMatching + ": " + stopMatching);
                    writer.WriteLine(TemplateHeaders.Priority + ": " + priority);
                    writer.WriteLine();
                    writer.WriteLine();
                    writer.WriteLine("// test method content");
                }

                // Act
                var result = TemplateReader.ReadFrom(testFile);

                // Assert
                result.TestMethodName.Resolve(new NamingContext("type")).Should().Be(testMethodName);
                result.Target.Should().Be(target);
                result.IsAsync.Should().Be(isAsync);
                result.IsStatic.Should().Be(isStatic);
                result.Description.Should().Be(description);
                result.IsExclusive.Should().Be(isExclusive);
                result.StopMatching.Should().Be(stopMatching);
                result.Priority.Should().Be(priority);
                result.Content.Should().StartWith("// test method content");
            }
            finally
            {
                File.Delete(testFile);
            }
        }
    }
}