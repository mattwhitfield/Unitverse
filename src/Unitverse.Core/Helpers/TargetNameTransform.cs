﻿namespace Unitverse.Core.Helpers
{
    using System;
    using System.Globalization;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

    public static class TargetNameTransform
    {
        public static string GetTargetProjectName(this IGenerationOptions options, string sourceProjectName)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrWhiteSpace(sourceProjectName))
            {
                throw new ArgumentNullException(nameof(sourceProjectName));
            }

            try
            {
                return string.Format(CultureInfo.CurrentCulture, options.TestProjectNaming, sourceProjectName);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException(Strings.TargetNameTransform_GetTargetProjectName_Cannot_not_derive_target_project_name__please_check_the_test_project_naming_setting_);
            }
        }

        public static string GetTargetFileName(this IGenerationOptions options, string sourceFileName)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrWhiteSpace(sourceFileName))
            {
                throw new ArgumentNullException(nameof(sourceFileName));
            }

            try
            {
                return string.Format(CultureInfo.CurrentCulture, options.TestFileNaming, sourceFileName);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException(Strings.TargetNameTransform_GetTargetFileName_Cannot_not_derive_target_file_name__please_check_the_test_file_naming_setting_);
            }
        }

        public static string GetTargetTypeName(this IFrameworkSet frameworkSet, ITypeSymbolProvider classModel)
        {
            if (frameworkSet is null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            return frameworkSet.Options.GenerationOptions.GetTargetTypeName(classModel);
        }

        public static string GetTargetTypeName(this IGenerationOptions options, ITypeSymbolProvider classModel)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (classModel == null)
            {
                throw new ArgumentNullException(nameof(classModel));
            }

            try
            {
                var sourceName = classModel.ClassName;
                if (classModel.TypeSymbol?.TypeParameters.Length > 0)
                {
                    sourceName += "_" + classModel.TypeSymbol.TypeParameters.Length;
                }

                return string.Format(CultureInfo.CurrentCulture, options.TestTypeNaming, sourceName);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException(Strings.TargetNameTransform_GetTargetTypeName_Cannot_not_derive_target_type_name__please_check_the_test_type_naming_setting_);
            }
        }

        public static string GetFullyQualifiedTargetTypeName(this IGenerationOptions options, ITypeSymbolProvider classModel, Func<string, string> transform)
        {
            var symbol = options.GetTargetTypeName(classModel);

            var transformed = transform(classModel.TypeSymbol.ContainingNamespace.ToFullName());

            return transformed + "." + symbol;
        }
    }
}