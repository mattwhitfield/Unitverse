# Features

Unitverse contains a lot of features that are all aimed at making the authoring of unit tests more engaging. The basic scaffolding becomes automatic and you can get straight to concentrating on how to really exercise your code and make sure that all the paths are tested.

| Feature | Description |
| --- | --- |
| Incremental generation & re-generation | Unitverse allows you to add to existing tests and re-generate tests when incompatible changes are made |
| Automatic test project organization | Unitverse automatically mimics the structure of your source project, making tests quick & easy to find |
| Framework auto-detection | Unitverse can automatically inspect the references in the test project, and emit tests compatible with the frameworks you use |
| Generation of tests for abstract & static classes | Unitverse emits tests for static classes correctly, and can even emit tests for abstract classes by automatically creating a sub-class to test |
| Generation of tests for protected methods | Unitverse can create tests for protected methods by creating a sub-class with publically visible wrapper methods |
| Support for nullable reference types | Unitverse will correctly emit tests for code that uses nullable reference types |
| Support for async methods | Unitverse will correctly create tests for async methods, using async parameter checking methods and awaiting calls |
| Mapping test generation | Unitverse will automatically detect when a method returns a type which shares member names with an input parameter's type, and emit a test to ensure mapping has occurred correctly |
| Dependency injection | Unitverse will automatically create dependencies using the mocking framework of your choice and inject them into the constructor |
| Mock configuration | If Unitverse detects a method call to an injected dependency it will emit default mock configuration calls so that you can more easily configure your mocks |
| Default Value Generation | Unitverse will create values for a wide variety of types when needed for method parameters, property values, constructor parameters etc. |
