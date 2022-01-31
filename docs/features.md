# Features

Unitverse contains a lot of features that are all aimed at making the authoring of unit tests more engaging. The basic scaffolding becomes automatic and you can get straight to concentrating on how to really exercise your code and make sure that all the paths are tested.

| Feature | Description |
| --- | --- |
| [Incremental generation & re-generation](features/incrementalgeneration.md) | Unitverse allows you to add to existing tests and re-generate tests when incompatible changes are made |
| [Automatic test project organization](features/automaticorganization.md) | Unitverse automatically mimics the structure of your source project, making tests quick & easy to find |
| [Framework auto-detection](features/frameworkautodetection.md) | Unitverse can automatically inspect the references in the test project, and emit tests compatible with the frameworks you use |
| [Generation of tests for abstract classes & protected methods](features/abstractandprotected.md) | Unitverse emits tests for abstract classes or classes with protected methods by automatically creating a sub-class to test |
| [Generation of tests for static classes](features/static.md) | Unitverse correctly emits tests for static classes |
| [Support for nullable reference types](features/nullablereferencetypes.md) | Unitverse will correctly emit tests for code that uses nullable reference types |
| [Support for async methods](features/async.md) | Unitverse will correctly create tests for async methods, using async parameter checking methods and awaiting calls |
| [Mapping test generation](features/mapping.md) | Unitverse will automatically detect when a method returns a type which shares member names with an input parameter's type, and emit a test to ensure mapping has occurred correctly |
| [Dependency injection](features/dependencyinjection.md) | Unitverse will automatically create dependencies using the mocking framework of your choice and inject them into the constructor |
| [Mock configuration](features/mockconfiguration.md) | If Unitverse detects a method call to an injected dependency it will emit default mock configuration calls so that you can more easily configure your mocks |
| [Default value generation](features/defaultvaluegeneration.md) | Unitverse will create values for a wide variety of types when needed for method parameters, property values, constructor parameters etc. |
