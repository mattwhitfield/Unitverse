namespace SentryOne.UnitTestGenerator.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Management.Instrumentation;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using NuGet.VisualStudio;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Properties;
    using VSLangProj;
    using VSLangProj80;

    internal static class ReferencesHelper
    {
        public static void AddNugetPackagesToProject(Project currentProject, IList<INugetPackageReference> packagesToInstall, Action<string> logMessage, IUnitTestGeneratorPackage generatorPackage)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var installedPackages = generatorPackage.PackageInstallerServices.GetInstalledPackages(currentProject).ToList();
            var installablePackages = packagesToInstall.Where(x => PackageNeedsInstalling(installedPackages, x)).ToList();

            if (generatorPackage.GetService(typeof(SVsThreadedWaitDialogFactory)) is IVsThreadedWaitDialogFactory dialogFactory)
            {
                dialogFactory.CreateInstance(out var dialog);
                if (dialog != null)
                {
                    foreach (var package in installablePackages)
                    {
                        var message = string.Format(CultureInfo.CurrentCulture, "Installing package '{0}'...", package.Name);
                        dialog.StartWaitDialog("Installing NuGet packages", message, string.Empty, null, message, 0, false, true);

                        InstallPackage(currentProject, generatorPackage, package);
                    }

                    dialog.EndWaitDialog(out _);
                    return;
                }
            }

            foreach (var package in installablePackages)
            {
                logMessage(string.Format(CultureInfo.CurrentCulture, "Installing package '{0}'...", package.Name));
                InstallPackage(currentProject, generatorPackage, package);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VSLangProj", Justification = "Is correct")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VSProject", Justification = "Is correct")]
        public static void AddReferencesToProject(Project currentProject, IList<IReferencedAssembly> referencedAssemblies, Action<string> logMessage)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var vsLangProj = currentProject.Object as VSProject;

            if (vsLangProj == null)
            {
                throw new InstanceNotFoundException(Strings.ReferencesHelper_AddReferencesToProject_The_VSLangProj_VSProject_instance_could_not_be_found_);
            }

            // Case-insensitive comparisons for all actions using this HashSet<T>.
            var existingReferences = new Dictionary<string, Reference3>(StringComparer.OrdinalIgnoreCase);

            foreach (var existingReference in vsLangProj.References.OfType<Reference3>())
            {
                existingReferences.Add(existingReference.Name, existingReference);
                if (string.Equals(existingReference.Name, "nunit.framework", StringComparison.OrdinalIgnoreCase))
                {
                    existingReferences[string.Format(CultureInfo.InvariantCulture, "nunit.framework({0})", existingReference.MajorVersion)] = existingReference;
                }
            }

            AddReferencesToProject(referencedAssemblies, existingReferences, vsLangProj, logMessage);
        }

        private static void AddReferencesToProject(IEnumerable<IReferencedAssembly> referencedAssemblies, IReadOnlyDictionary<string, Reference3> existingReferences, VSProject vsLangProj, Action<string> logMessage)
        {
            var addableReferences = new List<IReferencedAssembly>();

            foreach (var referenceAssembly in referencedAssemblies)
            {
                if (existingReferences.TryGetValue(referenceAssembly.Name, out var reference))
                {
                    if (reference.AutoReferenced)
                    {
                        continue;
                    }

                    try
                    {
                        var version = new Version(reference.Version);
                        var fileName = Path.GetFileName(referenceAssembly.Location);
                        var isSystemAssembly =
                            fileName?.StartsWith("System.", StringComparison.OrdinalIgnoreCase) ?? false;
                        isSystemAssembly |= fileName?.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase) ??
                                            false;
                        var isMicrosoftAssembly =
                            fileName?.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) ?? false;
                        var pathsAreEqual = string.Equals(reference.Path, referenceAssembly.Location, StringComparison.OrdinalIgnoreCase);

                        if (version == referenceAssembly.Version)
                        {
                            if (reference.SpecificVersion || isSystemAssembly || isMicrosoftAssembly)
                            {
                                continue;
                            }

                            if (pathsAreEqual)
                            {
                                SetSpecificVersionOnReference(reference);
                                continue;
                            }

                            if (reference.SpecificVersion)
                            {
                                continue;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(reference.Path))
                        {
                            if (reference.Path.IndexOf("bin\\", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                reference.Path.IndexOf("obj\\", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                try
                                {
                                    File.Delete(reference.Path);
                                }
                                catch (IOException)
                                {
                                }
                            }
                        }

                        reference.Remove();
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (FormatException)
                    {
                    }
                    catch (OverflowException)
                    {
                    }
                }

                addableReferences.Add(referenceAssembly);
            }

            addableReferences.ForEach(x =>
            {
                logMessage(string.Format(CultureInfo.CurrentCulture, "Adding reference to '{0}'...", x.Name));
                Reference reference;
                try
                {
                    reference = vsLangProj.References.Add(x.Location);
                }
                catch (FileLoadException)
                {
                    reference = null;
                }
#pragma warning disable CA1031 // we don't know the exception types up front because the reference we're adding could come from user code
                catch (Exception e)
                {
                    logMessage(string.Format(CultureInfo.CurrentCulture, "Could not add reference to '{0}': {1}", x.Name, e.Message));
                    reference = null;
                }
#pragma warning restore CA1031

                if (reference != null)
                {
                    SetSpecificVersionOnReference(reference);
                }
            });
        }

        private static void InstallPackage(Project currentProject, IUnitTestGeneratorPackage generatorPackage, INugetPackageReference package)
        {
            try
            {
                generatorPackage.PackageInstaller.InstallPackage("All", currentProject, package.Name, package.Version, false);
            }
            catch (InvalidOperationException)
            {
                generatorPackage.PackageInstaller.InstallPackage("https://www.nuget.org/api/v2/", currentProject, package.Name, package.Version, false);
            }
        }

        private static bool PackageNeedsInstalling(IEnumerable<IVsPackageMetadata> installedPackages, INugetPackageReference package)
        {
            var shouldInstall = false;

            // check whether we should install based on whether we have the current assembly and all it's DLLs
            var installedPackage = installedPackages.FirstOrDefault(x => string.Equals(x.Id, package.Name, StringComparison.OrdinalIgnoreCase));
            if (installedPackage != null)
            {
                try
                {
                    var requestedVersion = SemanticVersion.Parse(package.Version);
                    var installedVersion = SemanticVersion.Parse(installedPackage.VersionString);

                    if (requestedVersion.IsNewerThan(installedVersion))
                    {
                        shouldInstall = true;
                    }
                }
                catch (FormatException)
                {
                    shouldInstall = true;
                }
            }
            else
            {
                shouldInstall = true;
            }

            return shouldInstall;
        }

        private static void SetSpecificVersionOnReference(Reference reference)
        {
            if (reference.Name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) || reference.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var existingReference = reference as Reference3;

            if (existingReference == null)
            {
                return;
            }

            if (existingReference.AutoReferenced)
            {
                return;
            }

            try
            {
                existingReference.SpecificVersion = true;
                existingReference.CopyLocal = true;
            }
            catch (COMException)
            {
            }
        }
    }
}