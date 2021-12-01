# Overview

## Introduction
The Unitverse extension generates tests for classes written in C#. The extension covers basic tests automatically (for example, checking for correct property initialization), and creates placeholder tests for methods.  

Test Project organization is simple and automatic because the tests are created in the test project with the same hierarchy defined in the source project. The extension can be used to modify tests later in the life cycle after refactoring or adding new functionality:

* Add tests for new methods
* Regenerate tests as needed

## The goal
Unitverse aims to make the initial generation of tests much easier, and produce code that compiles. It's not perfect, but it's pretty good at achieving this. As you are writing members you can add tests for those members without having to stop and fix some uncompilable code.

## Supported Frameworks
The following test frameworks are supported:

* MSTest 
* NUnit 
* xUnit 

The following mocking frameworks are supported:

* NSubstitute 
* Moq 
* FakeItEasy 

Support is also present for using FluentAssertions for the assertions framework.

### Framework auto-detection

If Unitverse finds a test project related to the source project, it will look at the project references to determine what test and mocking frameworks to use. It will automatically use FluentAssertions if present. You can turn off framework auto-detection by going to Tools->Options->Unitverse.

## Using the Extension

After installation, open the extension through:

* The solution explorer context menu
* The code editor context menu

## Extension Functions

The following functions are available:

* **Generate tests** - generates tests for the selected entity.
* **Go to tests** - opens the file containing the tests for the selected object. This option also works if you are selecting a member in the code window.
* **Regenerate tests** - replaces existing tests with new ones. This is useful for cases like changes in a constructor signature. 

_**Important:** Regenerating a test will replace any code that you have added to the test class or method that is being regenerated. Please use this with care._

Using the code editor context menu:

![Code editor context menu](https://raw.githubusercontent.com/mattwhitfield/unittestgenerator/master/assets/CodeEditorContextMenu.png)

Using the solution explorer context menu:

![Solution Explorer context menu](https://raw.githubusercontent.com/mattwhitfield/unittestgenerator/master/assets/SolutionContextMenu.png)

**Regenerate tests** and **Go to tests** are not available at higher levels in the solution explorer (for example when you have a folder or project selected). **Regenerate tests** is not shown by default, to prevent accidental overwriting of test code. Hold SHIFT while you open the context menu to use this option.

## Use Case

Consider this simple class:

 ![Example source class](https://raw.githubusercontent.com/mattwhitfield/unittestgenerator/master/assets/SourceClass.png)

Although the constructor and methods are not implemented, it serves as a good example because the extension generates tests based on signatures only. The following illustrates the results of generating tests for this class.

 ![Example generated test class with annotations](https://raw.githubusercontent.com/mattwhitfield/unittestgenerator/master/assets/SourceClassTestsAnnotated.png)

Notice that the dependency for the class has been automatically mocked & injected, and there are generated tests for the constructor. There are also tests to verify that parameters can’t be null for both constructors and methods. Note that the generator is producing values required for testing – both initializing a POCO using an object initializer and an immutable class by providing values for its constructor.

## Controlling the process

The Unitverse extension options page allows for control of various aspects of the process. 

### Generation Options

* Select the test and mocking frameworks to be used
* Control the naming conventions for: test projects, classes, and files

_Note: The default for project naming is **‘{0}.Tests’**. For example, a project named **MyProject** would be associated with a test project called **MyProject.Tests**._

_Note: The default for the class and file naming is **‘{0}Tests’**. For example, a class called **MyClass** would be associated with a test class called **MyClassTests**._

### Test Method Naming Options

Unitverse allows you to customise the method names that are generated, so you can fit the extension in with your existing code base.

There are replacable tokens which can be substituted into the method names as follows:

| Token | Meaning | Available |
| - | - | - |
| typeName | The name of the type that the tested item belongs to | Always |
| interfaceName | The name of the interface for which generation is currently being performed | When generating tests for an interface implementation |
| memberName | The unambiguous name of the member being tested (see below) | Everywhere except constructors |
| memberBareName | The amiguous name of the member being tested (see below) | Everywhere except constructors |
| parameterName | The name of the parameter being tested for guard conditions | Method guard condition generation |
| typeParameters | The list of type parameters for an interface | When generating tests for an interface implementation |

#### On unambiguous vs. ambiguous names

When generating for a method with overloads, the names are decorated with 'WithParameterName' suffixes so that each method can be clearly identified. The unambiguous name includes these suffixes and the ambiguous name does not.

### Setting options per-solution

You can set settings per-solution if you need to (for example if you work with some code that uses MSTest and some that uses NUnit). In order to do this, you can create a `.unitTestGeneratorConfig` file, which is formatted similarly to .editorConfig files, just without the file type heading.

You can set any member of the [IGenerationOptions](https://github.com/mattwhitfield/unittestgenerator/blob/master/src/Unitverse.Core/Options/IGenerationOptions.cs) or the [INamingOptions](https://github.com/mattwhitfield/unittestgenerator/blob/master/src/Unitverse.Core/Options/INamingOptions.cs) interfaces using this method. For example, the following content in a `.unitTestGeneratorConfig` would set the test framework to MSTest, the mocking framework to NSubstitute and the test project naming convention to `<project_name>.UnitTests`:

```
test_project_naming={0}.UnitTests
framework-type=MSTest
MockingFrameworkType=NSubstitute
```

> Note that the formatting for the member names is case insensitive, and underscores and hyphens are ignored. Hence `frameworkType`, `framework-type`, `FRAMEWORK_TYPE` and `FRAME-WORK-TYPE` all resolve to the `FrameworkType` member of `IGenerationOptions`. The rules for file finding & resolution work exactly the same as they do for `.editorConfig` files - in short, all folders from the solution file to the root are searched for a `.unitTestGeneratorConfig` file, and files 'closer' to the solution file take precedence if the same option is set in more than one file.

## History

Unitverse was originally the SentryOne Unit Test Generator, and was written as part of an 'innovation sprint' where we wanted to write something that made our work more efficient. Unitverse is a fork of that project, and is not affiliated with any particular company.