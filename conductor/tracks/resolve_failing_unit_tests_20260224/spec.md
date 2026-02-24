# Specification: Resolve Failing Unit Tests

## Objective
Fix all existing unit test failures within the Piranha solution. The goal is to ensure the codebase correctly implements the required functionality as defined by its existing test suite.

## Constraint
- **CRITICAL:** DO NOT MODIFY THE UNIT TESTS. All fixes must be made in the application/production code.
- The only exception is if a test is demonstrably buggy AND the user explicitly approves a test modification (to be avoided).

## Scope
All projects under the `test/` directory, including but not limited to:
- `test/Aero.Identity.Tests`
- `test/Piranha.Tests`
- `test/Piranha.Manager.Tests`

## Success Criteria
- 100% pass rate across the entire test suite.
- No regressions in functionality.
