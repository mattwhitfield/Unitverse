namespace SentryOne.UnitTestGenerator.Core.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    public class NUnit2TestFramework : NUnitTestFramework
    {
        public override AttributeSyntax SingleThreadedApartmentAttribute => Generate.Attribute("RequiresSTA");
    }
}