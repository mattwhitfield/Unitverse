
# Introduction 👀
The Unitverse extension generates tests for classes written in C#. The extension covers basic tests automatically (for example, checking for correct property initialization), and creates placeholder tests for methods. Unitverse aims to produce tests that compile, so that you can generate tests as you code, but still focus on what you are doing rather than divert your attention to fixing broken generated code. Unitverse also allows incremental test generation - as you add new members to your types you can add tests for those new members quickly through the editor. Also, if you refactor methods or constructor signatures, you can regenerate those tests quickly and easily.

For more in-depth documentation, visit the [documentation on GitHub Pages](https://mattwhitfield.github.io/Unitverse/).

# Using the Extension 🔧

Using the code editor context menu:

![Code editor context menu](https://raw.githubusercontent.com/mattwhitfield/Unitverse/master/docs/assets/CodeEditorContextMenu.png)

Using the solution explorer context menu:

![Solution Explorer context menu](https://raw.githubusercontent.com/mattwhitfield/Unitverse/master/docs/assets/SolutionContextMenu.png)

# Visual Studio Versions 🆚
Due to the transition to 64-bit, Visual Studio 2022 introduces some architectural differences that necessitate a separate VSIX package. If you're working with Visual Studio 2019, you will need [Unitverse for Visual Studio 2019](https://marketplace.visualstudio.com/items?itemName=MattWhitfield.Unitverse) and if you're working with Visual Studio 2022, you will need [Unitverse for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=MattWhitfield.UnitverseVS2022).

# Supported Frameworks 🖼
### Test Frameworks

* MSTest 
* NUnit 
* xUnit 

FluentAssertions can also be used for assertions, replacing the assertions built in to whichever test framework is being used.

Unitverse can also use AutoFixture for value generation if configured. There are examples of [standard value generation](https://mattwhitfield.github.io/Unitverse/examples/ValueGeneration.md) and [value generation with AutoFixture](https://mattwhitfield.github.io/Unitverse/examples/ValueGenerationWithAutoFixture.md).

### Mocking Frameworks

* NSubstitute 
* Moq 
* Moq with Moq.AutoMock
* FakeItEasy 
* JustMock

### Framework Auto-Detection

If Unitverse finds a test project related to the source project, it will look at the project references to determine what test and mocking frameworks to use. It will automatically use FluentAssertions if present. You can turn off framework auto-detection by going to Tools->Options->Unitverse.
