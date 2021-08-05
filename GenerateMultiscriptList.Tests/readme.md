# Automation Testing

## Getting Started



## Naming <a name="Naming"></a>

## Class Method Naming <a name="ClassNaming"></a>
For Tables, the Class Method name should be `DBTypeAbbreviation_ObjectName`, to avoid conflicts with the Model class name.

For all other objects, the Class Method name should be the same as the Object's name.

## Test Method Naming <a name="TestNaming"></a>

All test methods should be named in the following manner:

`Should<Verb>_<Noun/Entity>_<Scenario>`

Examples:

- `ShouldThrow_Exception_WhenUserIDIsNull()`
- `ShouldReturn_1024Bytes_WhenDataTransferredIs1() //Fact`
- `ShouldReturn_XBytes_WhenDataTransferredIsY(input, expectedResult) //Theory`
- `ShouldReturn_ItemsOwnedByUserID_WhenUserIDIsValid()`
- `ShouldReturn_ItemsOwnedByUserID() //Obvious Scenario can be excluded`

## Traits <a name="Traits"></a>

We currently tag our tests with either *UnitTest* or *IntegrationTest* traits. The definition is below:

- **UnitTest:** A small subset of code paths in one individual object
- **IntegrationTest:** Testing object(s) that a) either have dependencies on other objects or b) are behemoth objects (e.g. a stored procedure with over 500 lines of code)

Reference constants in TestAttributes/TestCategory when setting traits

## Test Structure <a name="TestStructure"></a>

Use the Arrange / Act / Assert test pattern for writing tests. As quoted from https://docs.microsoft.com/en-us/visualstudio/test/unit-test-basics :

- The **Arrange** section of a unit test method initializes objects and sets the value of the data that is passed to the method under test.
- The **Act** section invokes the method under test with the arranged parameters.
- The **Assert** section verifies that the action of the method under test behaves as expected.

## Dependent 3rd Party Libraries <a name="3rdParty"></a>

1. xUnit (Testing Framework) - https://xunit.github.io/
2. FluentAssertions
	- https://fluentassertions.com/documentation/
	- https://github.com/fluentassertions/fluentassertions
3. Dapper / Dapper.Contrib (Data Access)
	- https://github.com/StackExchange/Dapper
	- https://github.com/StackExchange/Dapper/tree/master/Dapper.Contrib
4. NBuilder (Fake Data Generator) - https://github.com/nbuilder/nbuilder
5. AutoMapper (Map Multiple Columns without having to explicitly assign them) - https://github.com/AutoMapper/AutoMapper
