# Examples
This section contains examples of the output that Unitverse outputs, refreshed every build. Each example aims to demonstrate a particular scenario which is described in the following table.

| Example | Description |
| --- | --- |
| [AbstractClass](examples/AbstractClass.md) | Demonstrates how Unitverse generates tests when the source class is abstract or contains protected methods, as well as how inheritance chains are accounted for |
| [AsyncMethod](examples/AsyncMethod.md) | Demonstrates how tests are generated for async methods, as well as showing how the assertion framework is driven differently for async methods |
| [AutomaticMockGeneration](examples/AutomaticMockGeneration.md) | Demonstrates how dependencies injected into constructors are tracked, and mock configuration calls emitted for any detected dependencies |
| [ConstrainedGenericType](examples/ConstrainedGenericType.md) | Demonstrates how appropriate types are selected for the generation of tests for generic types with type constraints |
| [DelegateGeneration](examples/DelegateGeneration.md) | Demonstrates how Unitverse generates default values for method parameters when the parameter is a delegate type |
| [ExtensionMethod](examples/ExtensionMethod.md) | Demonstrates how Unitverse generates tests for extension methods |
| [GenericMethod](examples/GenericMethod.md) | Demonstrates how Unitverse generates tests for generic methods |
| [IComparableTests](examples/IComparableTests.md) | Demonstrates the tests generated for a type that implements IComparable |
| [IndexerTests](examples/IndexerTests.md) | Demonstrates the tests generated for a type that contains an indexer |
| [MappingMethod](examples/MappingMethod.md) | Shows how unitverse generates a test to verify mappings between input parameter type and return type where the types share property names |
| [MultipleOverloads](examples/MultipleOverloads.md) | Shows how unitverse generates unambiguous names for methods that test multiple overloads of the same source method |
| [NullableReferenceTypes](examples/NullableReferenceTypes.md) | Shows how Unitverse will omit `null` tests for parameters declared to explicitly accept null |
| [OperatorOverloading](examples/OperatorOverloading.md) | Shows how Unitverse emits tests for declared unary and binary operators |
| [PocoInitialization](examples/PocoInitialization.md) | Demonstrates how test values are produced to initialize POCO members when the type is consumed |
| [RefAndOutParameters](examples/RefAndOutParameters.md) | Demonstrates the tests that Unitverse emits when methods contain `ref` or `out` parameters |
| [SimplePoco](examples/SimplePoco.md) | Demonstrates how tests are generated for a simple POCO type |
| [Singleton](examples/Singleton.md) | Demonstrates how Unitverse attempts to use a static property to get a type instance when the constructor is private |
| [StaticClass](examples/StaticClass.md) | Demonstrates how Unitverse generates tests when the source class is static |
