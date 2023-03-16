namespace Unitverse.Core.Tests.Templating
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating;

    [TestFixture]
    public static class TemplateStoreTests
    {
        private static readonly string OneSub = "sub";
        private static readonly string TwoSubs = Path.Combine("sub", "sub");
        private static readonly string ThreeSubs = Path.Combine("sub", "sub", "sub");

        [Test]
        public static void CanCallLoadTemplatesFor()
        {
            var logger = Substitute.For<IMessageLogger>();
            var context = new NamingContext("dummy");
            RunOnDirectory(dir =>
            {
                WriteTemplateTo(dir, OneSub, "testMethod1");
                WriteTemplateTo(dir, TwoSubs, "testMethod2");
                WriteTemplateTo(dir, ThreeSubs, "testMethod3");

                TemplateStore.LoadTemplatesFor(Path.Combine(dir, ThreeSubs), logger).Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1", "testMethod2", "testMethod3");
                TemplateStore.LoadTemplatesFor(Path.Combine(dir, TwoSubs), logger).Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1", "testMethod2");
                TemplateStore.LoadTemplatesFor(Path.Combine(dir, OneSub), logger).Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1");
            });
        }

        [Test]
        public static void LoadTemplatesForIgnoresInvalidTemplatesAndLogsMessage()
        {
            var logger = Substitute.For<IMessageLogger>();
            var context = new NamingContext("dummy");
            RunOnDirectory(dir =>
            {
                WriteTemplateTo(dir, OneSub, "testMethod1");
                WriteTemplateTo(dir, TwoSubs, "testMethod2");
                WriteTemplateTo(dir, ThreeSubs, "testMethod3");
                WriteTemplateTo(dir, ThreeSubs, "testMethod4", true);

                TemplateStore.LoadTemplatesFor(Path.Combine(dir, ThreeSubs), logger).Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1", "testMethod2", "testMethod3");

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
                WriteTemplateTo(dir, OneSub, "testMethod1");
                WriteTemplateTo(dir, TwoSubs, "testMethod2");
                WriteTemplateTo(dir, ThreeSubs, "testMethod3");

                var originalTemplates = TemplateStore.LoadTemplatesFor(Path.Combine(dir, ThreeSubs), logger);
                originalTemplates.Select(x => x.TestMethodName.Resolve(context)).Should().BeEquivalentTo("testMethod1", "testMethod2", "testMethod3");

                FileInfo original = new FileInfo(Path.Combine(dir, ThreeSubs, TemplateStore.TemplateFolderName, "testMethod3" + TemplateStore.TemplateFileExtension));
                FileInfo updated = original;

                while (updated.LastWriteTimeUtc.ToString("O") == original.LastWriteTimeUtc.ToString("O"))
                {
                    WriteTemplateTo(dir, ThreeSubs, "testMethod3");
                    updated = new FileInfo(Path.Combine(dir, ThreeSubs, TemplateStore.TemplateFolderName, "testMethod3" + TemplateStore.TemplateFileExtension));
                    Thread.Sleep(100);
                }

                var updatedTemplates = TemplateStore.LoadTemplatesFor(Path.Combine(dir, ThreeSubs), logger);
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