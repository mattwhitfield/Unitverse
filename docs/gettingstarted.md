# Getting Started

After installation, you can access the functionality of the extension through:

* The solution explorer context menu
* The code editor context menu

## Extension Functions

The following functions are available:

* **Generate tests** - generates tests for the selected entity.
* **Go to tests** - opens the file containing the tests for the selected object. This option also works if you are selecting a member in the code window.
* **Regenerate tests** - replaces existing tests with new ones. This is useful for cases like changes in a constructor signature. 

_**Important:** Regenerating a test will replace code that you have added to the test class or method that is being regenerated. Please use this with care._

Using the code editor context menu:

![Code editor context menu](https://raw.githubusercontent.com/mattwhitfield/unittestgenerator/master/assets/CodeEditorContextMenu.png)

Using the solution explorer context menu:

![Solution Explorer context menu](https://raw.githubusercontent.com/mattwhitfield/unittestgenerator/master/assets/SolutionContextMenu.png)

**Regenerate tests** and **Go to tests** are not available at higher levels in the solution explorer (for example when you have a folder or project selected). **Regenerate tests** is not shown by default, to prevent accidental overwriting of test code. Hold SHIFT while you open the context menu to use this option.

## Use Case

Consider this simple class:

 ![Example source class](https://raw.githubusercontent.com/mattwhitfield/unittestgenerator/master/assets/SourceClass.png)

Although the constructor and methods are not implemented, it serves as a good example because the extension largely generates tests based on signatures only. The following illustrates the results of generating tests for this class.

 ![Example generated test class with annotations](https://raw.githubusercontent.com/mattwhitfield/unittestgenerator/master/assets/SourceClassTestsAnnotated.png)

Notice that the dependency for the class has been automatically mocked & injected, and there are generated tests for the constructor. There are also tests to verify that parameters can’t be null for both constructors and methods. Note that the generator is producing values required for testing – both initializing a POCO using an object initializer and an immutable class by providing values for its constructor.

For more examples of the output that Unitverse generates, please see the [Examples section](examples.md}.