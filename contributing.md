# Contributing to Unitverse
Welcome! If you are here, I am assuming that you are interested in contributing to the Unitverse project. That's fantastic - thank you in advance for your effort!

Please follow these guidelines:

* Smaller PRs that cover a single functionality enhancement are generally preferred
* Please ensure that there are no Code Analysis warnings or test failures created by the code
* Please add scenario tests to cover any new functionality - these can be in the `Unitverse.Specs` project or by adding your use case to `TestClasses.resx` in the `Unitverse.Core.Tests` project.
* If adding any new options that control how tests are generated, please make sure that the defaults for those options reflect existing behavior.
* Generators for third party scenarios (i.e. that require inspection of non-framework types) should not be part of the core test generator

I am really excited to see what you come up with to take this project forward - so thank you again!

For some more in-depth information about contributing, please see the [contributing page in the documentation](docs/contributing.md).