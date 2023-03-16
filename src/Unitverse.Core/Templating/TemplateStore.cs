namespace Unitverse.Core.Templating
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Unitverse.Core.Helpers;

    public static class TemplateStore
    {
        public const string TemplateFolderName = ".unitverseTemplates";
        public const string TemplateFileExtension = ".template";

        private static readonly Dictionary<string, ITemplate> _cache = new Dictionary<string, ITemplate>(StringComparer.OrdinalIgnoreCase);

        public static IList<ITemplate> LoadTemplatesFor(string folder, IMessageLogger messageLogger)
        {
            var output = new List<ITemplate>();
            var directory = new DirectoryInfo(folder);

            while (directory != null)
            {
                var templateFolders = directory.GetDirectories(TemplateFolderName, SearchOption.TopDirectoryOnly);
                var templateFolder = templateFolders.FirstOrDefault();
                if (templateFolder != null)
                {
                    output.AddRange(ReadTemplates(templateFolder, messageLogger));
                }

                directory = directory.Parent;
            }

            return output;
        }

        private static string GetCacheKey(FileInfo fileInfo)
        {
            return fileInfo.FullName + "." + fileInfo.LastWriteTimeUtc.ToString("O");
        }

        private static IEnumerable<ITemplate> ReadTemplates(DirectoryInfo directoryInfo, IMessageLogger messageLogger)
        {
            foreach (var file in directoryInfo.GetFiles("*" + TemplateFileExtension, SearchOption.TopDirectoryOnly))
            {
                var cacheKey = GetCacheKey(file);
                if (_cache.TryGetValue(cacheKey, out var template))
                {
                    yield return template;
                }
                else
                {
                    try
                    {
                        template = TemplateReader.ReadFrom(file.FullName);
                    }
                    catch (Exception ex)
                    {
                        messageLogger.LogMessage(ex.Message);
                    }

                    if (template != null)
                    {
                        _cache[cacheKey] = template;
                        yield return template;
                    }
                }
            }
        }
    }
}
