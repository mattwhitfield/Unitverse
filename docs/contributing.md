# Contributing ✋

Unitverse isn't affiliated with any company, so the development is completely open source. As such, the development isn't guaranteed to be on any particular schedule. Any input from the community is hugely valuable, even if it's just raising an issue or suggesting a feature. You can get started by visiting the [Unitverse GitHub repo](https://github.com/mattwhitfield/Unitverse).

If you want to go further, and contribute some code to the project, then this guide aims to give an indication of how the Unitverse extension is engineered and explain some of the decisions.

## Guidelines 🚧

Please follow these guidelines:

* Smaller PRs that cover a single functionality enhancement are generally preferred
* Please ensure that there are no Code Analysis warnings or test failures created by the code
* Please add scenario tests to cover any new functionality - these can be in the `Unitverse.Specs` project or by adding your use case to `TestClasses.resx` in the `Unitverse.Core.Tests` project.
* If adding any new options that control how tests are generated, please make sure that the defaults for those options reflect existing behavior.
* Generators for third party scenarios (i.e. that require inspection of non-framework types) should not be part of the core test generator

I am really excited to see what you come up with to take this project forward - so thank you again!

## Solution Layout 🗃

The code base on Unitverse is split into a few projects:

| Project | Description |
| - | - |
| Unitverse | This project contains the Visual Studio specific functionality, adding commands to menus, providing options etc. It is intended to be as thin a wrapper as possible around the core code |
| Unitverse.Core | This project is where the main magic happens. All the generation functionality, model extraction etc. exists within this project |
| Unitverse.Core.Tests | This project contains unit tests for Unitverse.Core - the main aim is to ensure that Unitverse always produces code that compiles, hence a lot of tests aren't super specific about the output produced |
| Unitverse.Specs | This project contains SpecFlow tests that ensure that the core generation functions of Unitverse operate correctly and produce the expected basic output |
| Unitverse.ExampleGenerator | This project contains a small application that, for a set of example classes, produces the markdown output that forms the examples section in the documentation. This also forms the basis of the build protection - see the 'Build Protection' section below |

## Unitverse.Core 🍏

This project bears some more explanation as to it's layout - the main folders are:

| Folder | Description |
| - | - |
| Assets | Things that can get added to test projects, like the NotifyPropertyChanged helper |
| Frameworks\Assertion | Code that supports emitting tests using assertion frameworks |
| Frameworks\Mocking | Code that supports using different mocking libraries |
| Frameworks\Test | Code that supports using different test frameworks |
| Helpers | General helper functions |
| Models | Models to represent the input types for the generation process |
| Options | Code to handle options classes, including loading from `.unitTestGeneratorConfig` files |
| Resources | Test classes, used to validate that the code produced compiles using every different combination of available frameworks |
| Strategies\ClassDecoration | Code that supports adding attributes to emitted test classes when necessary |
| Strategies\ClassGeneration | Code that generates the basic test classes for different scenarios (standard/static/abstract) |
| Strategies\ClassLevelGeneration | Code that generates tests that are grouped at the class level (i.e. constructors and initializers) |
| Strategies\IndexerGeneration | Code that generates tests for indexers |
| Strategies\IntefaceGeneration | Code that generates tests for specific interface implementations |
| Strategies\MethodGeneration | Code that generates tests for methods |
| Strategies\OperatorGeneration | Code that generates tests for operators |
| Strategies\ProptertyGeneration | Code that generates tests for properties |
| Strategies\ValueGeneration | Code that generates default values when they are needed (e.g. for method parameters) |

The main bulk of the interesting code is in the `Strategies` folder. The `CoreGenerator` class is the main class from which generation happens.

## Testing Ethos 🚨

The general aim with the extension is not to be really specific about the output that is generated. So, for example, if a new method of deriving a default value for a type is added, it doesn't matter so much that it is done in an exact way. The most important thing is that the code compiles - Unitverse aims to 'get out of the way' of the developer so they can immediately think about how they want to go about testing the code they just wrote, instead of having to fix a bunch of broken code. Obviously, it won't generate tests that pass except for the simplest of cases, which is fine.

There are some tests that do check output - in the `Unitverse.Specs` project - and they check things like 'has a mock been generated for this dependency' and 'is this basic assert emitted'. But they are not designed to break every time something minor changes.

## Build Protection 💥

The example generator generates test classes for a bunch of known samples, and then emits markdown that form the 'examples' section in the documentation. It runs every build (when the example generator has built, it is called by the msbuild script in the post build actions). So then, if you have changed the output, you see the changes in the example documentation.

When the build runs in GitHub Actions, it checks that the examples generated are the same as what has been checked in. It does this by checking that there are no uncommitted changes in the docs folder, and breaks the build if there are. So if you do make changes the generated tests, you have to 'accept' that by committing the newly generated markdown docs. Equally, when reviewing a PR, we can pay careful attention to the examples folder.

## Areas for tidy up 🧹

There are several things that could be better:

* Although the existing tests do a really good job of making sure that we produce compilable code, we could always do with more tests
* We could transition the code base to make use of nullable reference types
* The `Generate` class isn't universally used - there are some strategies that make heavy use of SyntaxFactory directly
* We could do with some better test output for specific scenarios - MVC controllers come to mind
