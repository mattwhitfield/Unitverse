---
title: Home
---
![Latest release](https://img.shields.io/github/v/release/mattwhitfield/Unitverse?color=00A000) ![Last commit](https://img.shields.io/github/last-commit/mattwhitfield/Unitverse?color=00A000) ![Build status](https://img.shields.io/github/workflow/status/mattwhitfield/Unitverse/Extension%20build) ![Open issue count](https://img.shields.io/github/issues/mattwhitfield/Unitverse)

# Introduction 👀
The Unitverse Visual Studio extension generates tests for classes written in C#. The extension covers basic tests automatically (for example, checking for correct property initialization), and creates placeholder tests for methods. Unitverse aims to produce tests that compile, so that you can generate tests as you code, but still focus on what you are doing rather than divert your attention to fixing broken generated code. Unitverse also allows incremental test generation - as you add new members to your types you can add tests for those new members quickly through the editor. Also, if you refactor methods or constructor signatures, you can regenerate those tests quickly and easily.

### Documentation Sections 📖

* [Getting started](gettingstarted.md) - covers basic usage and how to start generating tests.
* [Features](features.md) - covers the features offered by Unitverse along with in-depth descriptions.
* [Configuration](configuration.md) - covers the configuration options available, as well as details on how to configure the extension through source code.
* [Contributing](contributing.md) - covers useful information for anyone who wants to contribute to the project.
* [Examples](examples/index.md) - contains a selection of sample scenarios showing the source code and the generated tests.

## Supported Environments 🌳

### Visual Studio Versions 🆚
Due to the transition to 64-bit, Visual Studio 2022 introduces some architectural differences that necessitate a separate VSIX package. If you're working with Visual Studio 2019, you will need [Unitverse for Visual Studio 2019](https://marketplace.visualstudio.com/items?itemName=MattWhitfield.Unitverse) and if you're working with Visual Studio 2022, you will need [Unitverse for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=MattWhitfield.UnitverseVS2022).

### Supported Frameworks 🖼
#### Test Frameworks

* MSTest 
* NUnit 
* xUnit 

FluentAssertions can also be used for assertions, replacing the assertions built in to whichever test framework is being used.

Unitverse can also use AutoFixture for value generation if configured. There are examples of [standard value generation](examples/ValueGeneration.md) and [value generation with AutoFixture](examples/ValueGenerationWithAutoFixture.md).

#### Mocking Frameworks 

* NSubstitute 
* Moq 
* Moq with Moq.AutoMock
* FakeItEasy 

### Framework Auto-Detection 🔍

If Unitverse finds a test project related to the source project, it will look at the project references to determine what test and mocking frameworks to use. It will automatically use FluentAssertions and AutoFixture if present. You can turn off framework auto-detection by going to Tools->Options->Unitverse.

## Contributing ✋

test

Any contributions are welcome. Code. Feedback on what you like or what could be better. Please feel free to fork the repo and make changes, the license is MIT so you're pretty much free to do whatever you like. For more information, please see the [contributing section](contributing.md). You can get started by visiting the [Unitverse GitHub repo](https://github.com/mattwhitfield/Unitverse).

## Support 🤝

If you'd like to support the project but you don't want to contribute code - a really good way to help is spread the word. There isn't a donation or financial option becuase honestly I don't need donations. I am, however, rubbish at 'marketing'. So any help there would be greatly appreciated! Whether it's a blog post, a marketplace review or just telling some folks at the office - every little helps.