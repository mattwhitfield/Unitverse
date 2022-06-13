# Features ✨

Unitverse contains a lot of features that are all aimed at making the authoring of unit tests more engaging. The basic scaffolding becomes automatic and you can get straight to concentrating on how to really exercise your code and make sure that all the paths are tested.

### 🔴 Incremental generation & re-generation

Unitverse allows you to add to existing tests and re-generate tests when incompatible changes are made. By default, if you generate tests for a class and existing tests are present, Unitverse will only add newly generated tests to your existing test class. You can also selectively generate tests for individual class members by right clicking on the member name in the code editor window and choosing 'Generate tests' from there.

### 🟠 Automatic test project organization

Unitverse automatically mimics the structure of your source project, making tests quick & easy to find. If you are generating a test class for a class that is in a hierarchy of folders, then Unitverse will create that same hierarchy of folders in your test project.

### 🟡 Framework auto-detection

Unitverse can automatically inspect the references in the test project, and emit tests compatible with the frameworks you use. It will look for references for test framework, mocking framework and whether or not Fluent Assertions and AutoFixture are present. If it doesn't find those references, it will use the defaults configured in the Visual Studio options page.

### 🟢 Generation of tests for abstract classes & protected methods

Unitverse emits tests for abstract classes or classes with protected methods by automatically creating a sub-class to test. Any members without public visibility will receive a public wrapper method which is then exercised by the emitted unit test.

### 🔵 Per-generation control

Unitverse can be configured to show a user interface before each generation, or show it selectively by holding the Control key while clicking on a menu item. This allows you to change the configuration and select the target project for an individual generation, if required.

### 🟣 Support for nullable reference types

Unitverse will correctly emit tests for code that uses nullable reference types. If a parameter is declared with the nullable syntax `TypeName?` then it will be considered to be explicitly nullable, and a `null` checking test will not be emitted. In the case of a string test, where `null`, empty string and whitespace are checked for, a declaration of type `string?` will result in only the empty string and whitespace tests being emitted.

### 🔴 Support for async methods

Unitverse will correctly create tests for async methods, using async parameter checking methods and awaiting calls. This varies from test framework to test framework, but typically yields a test method with an `async Task` signature, and awaiting the result of framework methods to check for invalid value exceptions.

### 🟠 Mapping test generation

Unitverse will automatically detect when a method returns a type which shares member names with an input parameter's type, and emit a test to ensure mapping has occurred correctly. It does this by initializing the input and then, for each member shared by both input and return types, checks that the returned member matches the input member.

### 🟡 Dependency injection

Unitverse will automatically create dependencies using the mocking framework of your choice and inject them into the constructor. These dependencies will be created as mocks for interface types, or a suitable value for non-interface types.

### 🟢 Mock configuration

If Unitverse detects a method call to an injected dependency it will emit default mock configuration calls so that you can more easily configure your mocks. It does this by tracking the fields in which constructor parameters are stored, and then finding method calls and property reads that involve those fields. Default mock configuration calls are then emitted before the test method calls the method being tested, and verification calls are emitted afterwards.

### 🔵 Automatic property checks

When a class has a property that has the same name as a constructor parameter (ignoring case), Unitverse will emit a test to ensure that the property was initialized to the same value that was injected into the constructor.

### 🟣 Default value generation

Unitverse will create values for a wide variety of types when needed for method parameters, property values, constructor parameters etc. Unitverse tries to smartly choose values for the type in question in a recursive way - so if construction of the type in question requires a value of another type, then it emits code that works.

### 🔴 Per-member generation

Unitverse can generate tests for single members, meaning that you can generate tests for only the items that you want to test, or add tests for new members when your classes already have tests defined.

### 🟠 Test project creation

Unitverse allows you to easily create unit tests projects with the right packages installed by clicking on the 'Create unit test project' option on the project context menu.
