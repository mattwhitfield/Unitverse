namespace Unitverse.Core.Models
{
    using System;
    using System.IO;
    using Unitverse.Core.Options;

    public class ProjectCreationManifest
    {
        public ProjectCreationManifest(string name, string folderName, IGenerationOptions generationOptions)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            Name = name;
            FolderName = folderName;
            GenerationOptions = generationOptions ?? throw new ArgumentNullException(nameof(generationOptions));
        }

        public string Name { get; }

        public string FolderName { get; }

        public IGenerationOptions GenerationOptions { get; }

        public string ProjectFileName => Path.Combine(FolderName, Name, Name + ".csproj");
    }
}
