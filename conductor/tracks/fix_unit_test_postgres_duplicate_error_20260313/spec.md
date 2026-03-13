# Specification: Fix PostgreSQL Unit Test Duplicate Database Error

## Overview
Resolve unit test failures in `Aero.Identity.Tests` and `Aero.Cms.Tests` that occur during the switch to PostgreSQL with Marten. The error `23505: duplicate key value violates unique constraint "pg_database_datname_index"` indicates that multiple tests or initializations are attempting to create the same database simultaneously or without checking for its existence.

## Functional Requirements
- **Root Cause Analysis:** Identify if Marten's database creation logic (e.g., `CreateDatabaseIfNotExists`) is being called concurrently or without proper idempotent checks.
- **Isolation Strategy:** Implement a robust strategy for database isolation in unit tests, ensuring that each test run or test collection has a dedicated, non-conflicting environment.
- **Fix Implementation:** Update the test base classes or initialization logic in `Aero.Identity.Tests` and `Aero.Cms.Tests` to resolve the error.

## Non-Functional Requirements
- **Execution Efficiency:** Ensure that the fix doesn't add significant overhead to the test execution time.
- **Repeatability:** Tests must be runnable multiple times on the same PostgreSQL instance without manual database cleanup.

## Acceptance Criteria
- All tests in `Aero.Identity.Tests` and `Aero.Cms.Tests` pass consistently.
- No `Npgsql.PostgresException : 23505` related to the `pg_database` table is observed during test runs.

## Out of Scope
- Optimizing database performance for production environments.
- Migrating other test projects not mentioned in the scope.
