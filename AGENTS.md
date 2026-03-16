# Agent Instructions: Test Remediation

## Objective
Fix all failed tests identified in `test-results.txt`.

## Focus
- Focus exclusively on resolving test failures.
- Only modify unit tests that require updates due to the recent RavenDB modeling/entity refactor (NoSQL style - normalized -> denormalized).
- Ensure test changes align with the "no relationships" architecture only.
- Do not modify core test logic unless it is strictly necessary to fix a regression caused by the refactor that manifests in these tests.
- modify the non-test code as you see fit to get it working in the new denormazlied refactor + raven idioms

## Diagnostics
- Use `test-results.txt` as the source of truth for failing tests.
- Use `dotnet test --list-tests` to explore available test cases.
- Refer to `test_results.log` for detailed stack traces of past failures if needed.

## Constraints
- Stay within the scope of the unit test failures.
- Modernize test data and assertions to match the flattened NoSQL entity structures.
