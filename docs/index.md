# Introduction
The Unitverse Visual Studio extension generates tests for classes written in C#. The extension covers basic tests automatically (for example, checking for correct property initialization), and creates placeholder tests for methods. Unitverse aims to produce tests that compile, so that you can generate tests as you code, but still focus on what you are doing rather than divert your attention to fixing broken generated code. Unitverse also allows incremental test generation - as you add new members to your types you can add tests for those new members quickly through the editor. Also, if you refactor methods or constructor signatures, you can regenerate those tests quickly and easily.

## Documentation Sections

* [Getting started](gettingstarted.md) - covering basic usage and how to start generating tests.
* [Features](features.md) - covers the features offered by Unitverse along with in-depth descriptions.
* [Configuration](configuration.md) - covering the configuration options available, as well as details on how to configure the extension through source code.
* [Examples](examples.md) - presents a selection of sample scenarios showing the source code and the generated tests.

# Supported Environments

## Visual Studio Versions
Due to the transition to 64-bit, Visual Studio 2022 introduces some architectural differences that necessitate a separate VSIX package. If you're working with Visual Studio 2019, you will need [Unitverse for Visual Studio 2019](https://marketplace.visualstudio.com/items?itemName=MattWhitfield.Unitverse) and if you're working with Visual Studio 2022, you will need [Unitverse for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=MattWhitfield.UnitverseVS2022).

## Supported Frameworks
#### Test Frameworks

* MSTest 
* NUnit 
* xUnit 

FluentAssertions can also be used for assertions, replacing the assertions built in to whichever test framework is being used.

#### Mocking Frameworks

* NSubstitute 
* Moq 
* FakeItEasy 

### Framework Auto-Detection

If Unitverse finds a test project related to the source project, it will look at the project references to determine what test and mocking frameworks to use. It will automatically use FluentAssertions if present. You can turn off framework auto-detection by going to Tools->Options->Unitverse.
