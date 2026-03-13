# Implementation Plan: Fix PostgreSQL Unit Test Duplicate Database Error

## Phase 1: Diagnosis and Reproduction
- [ ] **Task: Identify Failing Test Suites**
    - [ ] Run `dotnet test test/Aero.Identity.Tests` and `dotnet test test/Aero.Cms.Tests` to capture the full stack trace of the `23505` error.
    - [ ] Document which specific tests or base classes are triggering the database creation logic.
- [ ] **Task: Create Minimal Reproduction**
    - [ ] Write a new test case in `Aero.Identity.Tests` that explicitly triggers the Marten database initialization multiple times in parallel to reproduce the race condition (Red Phase).
- [ ] **Task: Conductor - User Manual Verification 'Phase 1: Diagnosis and Reproduction' (Protocol in workflow.md)**

## Phase 2: Infrastructure Fix
- [ ] **Task: Update Test Base Class Initialization**
    - [ ] Modify the `BaseTestsAsync` or Marten configuration logic to ensure that database creation is either serialized or uses unique database names per test collection.
    - [ ] Implement a check to ensure `CreateDatabaseIfNotExists` handles existing databases gracefully without throwing a unique constraint violation on `pg_database`.
- [ ] **Task: Verify Fix in Reproduction Test**
    - [ ] Run the minimal reproduction test created in Phase 1 and confirm it now passes (Green Phase).
- [ ] **Task: Conductor - User Manual Verification 'Phase 2: Fix Implementation' (Protocol in workflow.md)**

## Phase 3: Final Validation and Cleanup
- [ ] **Task: Execute Full Test Suite**
    - [ ] Run all tests in `Aero.Identity.Tests` and `Aero.Cms.Tests` to ensure 100% pass rate.
- [ ] **Task: Verify Database Cleanup**
    - [ ] Ensure that test databases are properly disposed of or don't leak resources after the test run.
- [ ] **Task: Conductor - User Manual Verification 'Phase 3: Final Verification' (Protocol in workflow.md)**
