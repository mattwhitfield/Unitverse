namespace Unitverse.Core.Tests.Templating
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating;

    [TestFixture]
    public static class TemplateStoreTests
    {
        [Test]
        public static void CanCallLoadTemplatesFor()
        {
            var logger = Substitute.For<IMessageLogger>();
            var context = new NamingContext("dummy");
            RunOnDirectory(dir =>
            {
                WriteTemplateTo(dir, "sub", "testMethod1");
                WriteTemplateTo(dir, "sub\\sub", "testMethod2");
                WriteTemplateTo(dir, "sub\\sub\\sub", "testMethod3");

                TemplateStore.LoadTemplatesFor(Path.Combine(dir, "sub\\sub\\sub"), logger).Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1", "testMethod2", "testMethod3");
                TemplateStore.LoadTemplatesFor(Path.Combine(dir, "sub\\sub"), logger).Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1", "testMethod2");
                TemplateStore.LoadTemplatesFor(Path.Combine(dir, "sub"), logger).Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1");
            });
        }

        [Test]
        public static void LoadTemplatesForIgnoresInvalidTemplatesAndLogsMessage()
        {
            var logger = Substitute.For<IMessageLogger>();
            var context = new NamingContext("dummy");
            RunOnDirectory(dir =>
            {
                WriteTemplateTo(dir, "sub", "testMethod1");
                WriteTemplateTo(dir, "sub\\sub", "testMethod2");
                WriteTemplateTo(dir, "sub\\sub\\sub", "testMethod3");
                WriteTemplateTo(dir, "sub\\sub\\sub", "testMethod4", true);

                TemplateStore.LoadTemplatesFor(Path.Combine(dir, "sub\\sub\\sub"), logger).Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1", "testMethod2", "testMethod3");

                logger.ReceivedWithAnyArgs().LogMessage("message");
            });
        }

        [Test]
        public static void LoadTemplatesForReloadsUpdatedFiles()
        {
            var logger = Substitute.For<IMessageLogger>();
            var context = new NamingContext("dummy");
            RunOnDirectory(dir =>
            {
                WriteTemplateTo(dir, "sub", "testMethod1");
                WriteTemplateTo(dir, "sub\\sub", "testMethod2");
                WriteTemplateTo(dir, "sub\\sub\\sub", "testMethod3");

                var originalTemplates = TemplateStore.LoadTemplatesFor(Path.Combine(dir, "sub\\sub\\sub"), logger);
                originalTemplates.Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1", "testMethod2", "testMethod3");

                WriteTemplateTo(dir, "sub\\sub\\sub", "testMethod3");

                var updatedTemplates = TemplateStore.LoadTemplatesFor(Path.Combine(dir, "sub\\sub\\sub"), logger);
                updatedTemplates.Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1", "testMethod2", "testMethod3");

                ITemplate GetTemplate(IList<ITemplate> templates, string name)
                {
                    return templates.FirstOrDefault(x => x.TestMethodName.Resolve(context) == name);
                }

                GetTemplate(updatedTemplates, "testMethod1").Should().BeSameAs(GetTemplate(originalTemplates, "testMethod1"));
                GetTemplate(updatedTemplates, "testMethod2").Should().BeSameAs(GetTemplate(originalTemplates, "testMethod2"));
                GetTemplate(updatedTemplates, "testMethod3").Should().NotBeSameAs(GetTemplate(originalTemplates, "testMethod3"));
            });
        }

        private static void WriteTemplateTo(string rootFolder, string relativePath, string testMethodName, bool invalid = false)
        {
            var fileName = Path.Combine(rootFolder, relativePath, TemplateStore.TemplateFolderName, testMethodName + TemplateStore.TemplateFileExtension);

            var dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir)) 
            {
                Directory.CreateDirectory(dir);
            }

            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine(TemplateHeaders.TestMethodName + ": " + testMethodName);
                writer.WriteLine(TemplateHeaders.Target + ": Property");
                writer.WriteLine(TemplateHeaders.Include + ": class.Name == 'fred'");
                if (invalid)
                {
                    writer.WriteLine(TemplateHeaders.Include + ": well this is awkward");
                }
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine("// test method content");
            }
        }

        private static void RunOnDirectory(Action<string> action)
        {
            var testDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(testDir);
            try
            {
                action(testDir);
            }
            finally
            {
                Directory.Delete(testDir, true);
            }
        }

    }
}