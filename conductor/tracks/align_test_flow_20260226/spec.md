# Track Specification: Align Test Infrastructure with Flow Map

## Overview
This track focuses on verifying and aligning the unit test initialization logic with the architectural flow defined in `flow.md`. The primary goal is to ensure the test suite correctly utilizes the RavenDB TestDriver and DI containers to achieve a reliable and idiomatic testing environment, ultimately resolving existing test failures.

## Scope
- **Test Base Classes**: Audit `BaseTests.cs` and `BaseTestsAsync.cs` in `test/Piranha.Tests`.
- **DI Registration**: Verify `CreateServiceCollection()` and `CreateApi()` in the test base match the "Detailed Test Initialization Flow" in `flow.md`.
- **Lifecycle Management**: Ensure `InitializeAsync()` and `DisposeAsync()` correctly manage the static `App` singleton and database state.
- **Service Alignment**: Confirm all 14 repositories are correctly composed into the `IApi` instance used in tests.

## Requirements
- **Flow Consistency**: The code implementation MUST match the "Unit Test Initialization Map" in `flow.md`.
- **RavenDB Test Driver**: Tests must use the in-memory `DocumentStore` provided by the Raven Test Driver.
- **Clean State**: Each test (or test class) must start with a fresh, initialized state.

## Acceptance Criteria
- Unit tests in `test/Piranha.Tests` pass (targeting at least the core infrastructure tests first).
- The `IApi` construction in tests follows the composition root pattern defined in the diagram.
- `Piranha.App.Init()` is called with a valid `IApi` instance during test setup.

## Out of Scope
- Runtime request flow (ASP.NET Core middleware/hosting).
- Integration testing in example projects (`MvcWeb`, `RazorWeb`).
- Optimization of passing tests.
