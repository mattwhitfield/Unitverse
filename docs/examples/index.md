# Examples Overview
This section contains examples of the output that Unitverse outputs, refreshed every build. Each example aims to demonstrate a particular scenario which is described in the following table.

| Example | Description |
| --- | --- |
| [Abstract Classes](AbstractClass.md) | Demonstrates how Unitverse generates tests when the source class is abstract or contains protected methods, as well as how inheritance chains are accounted for |
| [Async Methods](AsyncMethod.md) | Demonstrates how tests are generated for async methods, as well as showing how the assertion framework is driven differently for async methods |
| [Automatic Mock Generation](AutomaticMockGeneration.md) | Demonstrates how dependencies injected into constructors are tracked, and mock configuration calls emitted for any detected dependencies |
| [Constrained Generic Types](ConstrainedGenericType.md) | Demonstrates how appropriate types are selected for the generation of tests for generic types with type constraints |
| [Delegate Generation](DelegateGeneration.md) | Demonstrates how Unitverse generates default values for method parameters when the parameter is a delegate type |
| [Extension Methods](ExtensionMethod.md) | Demonstrates how Unitverse generates tests for extension methods |
| [Frameworks - Fluent Assertions](FrameworksFluentAssertions.md) | Demonstrates how tests are generated using XUnit for the test framework and NSubstitute for the mocking framework. Also shows using FluentAssertions for the assertion framework. |
| [Frameworks - MSTest & Moq](FrameworksMsTestMoq.md) | Demonstrates how tests are generated using MsTest for the test framework and Moq for the mocking framework |
| [Frameworks - NUnit 3 & FakeItEasy](FrameworksNUnitFakeItEasy.md) | Demonstrates how tests are generated using NUnit 3 for the test framework and FakeItEasy for the mocking framework |
| [Frameworks - XUnit & NSubstitute](FrameworksXUnitNSubstitute.md) | Demonstrates how tests are generated using XUnit for the test framework and NSubstitute for the mocking framework |
| [Generic Methods](GenericMethod.md) | Demonstrates how Unitverse generates tests for generic methods |
| [IComparable](IComparableTests.md) | Demonstrates the tests generated for a type that implements IComparable |
| [IEquatable](IEquatableTests.md) | Demonstrates the tests generated for a type that implements IEquatable |
| [Indexers](IndexerTests.md) | Demonstrates the tests generated for a type that contains an indexer |
| [Mapping Methods](MappingMethod.md) | Shows how unitverse generates a test to verify mappings between input parameter type and return type where the types share property names |
| [Multiple Overloads](MultipleOverloads.md) | Shows how unitverse generates unambiguous names for methods that test multiple overloads of the same source method |
| [Nullable Reference Types](NullableReferenceTypes.md) | Shows how Unitverse will omit `null` tests for parameters declared to explicitly accept null |
| [Operator Overloading](OperatorOverloading.md) | Shows how Unitverse emits tests for declared unary and binary operators |
| [POCO Initialization](PocoInitialization.md) | Demonstrates how test values are produced to initialize POCO members when the type is consumed |
| [Property Initialization Checks](PropertyInitializationChecks.md) | Demonstrates how properties that have matching constructor parameters are checked that they are initialized automatically |
| [Record Types (init Properties)](RecordTypeInitProperties.md) | Demonstrates the tests generated for a record type that has properties that have init accessors |
| [Record Types (Primary Constructor)](RecordTypesPrimaryConstructor.md) | Demonstrates the tests generated for a simple primary constructor record type |
| [ref & out Parameters](RefAndOutParameters.md) | Demonstrates the tests that Unitverse emits when methods contain `ref` or `out` parameters |
| [Simple POCO](SimplePoco.md) | Demonstrates how tests are generated for a simple POCO type |
| [Singletons](Singleton.md) | Demonstrates how Unitverse attempts to use a static property to get a type instance when the constructor is private |
| [Static Classes](StaticClass.md) | Demonstrates how Unitverse generates tests when the source class is static |
