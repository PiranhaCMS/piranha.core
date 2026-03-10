# Implementation Plan: `plan.md`

## Phase 1: Diagnosis and Reproduction
- [x] **Task: Write Failing Tests for Role Retrieval**
    - [x] Create a new test in `Aero.Identity.Tests` that creates a role and then attempts to retrieve it by name via `RoleManager.FindByNameAsync`.
    - [x] Confirm the test fails (Red phase).
- [x] **Task: Investigate `RoleStore` Implementation**
    - [x] Examine `Aero.Identity/Stores/RoleStore.cs` (or similar) to see how `FindByNameAsync` is implemented.
    - [x] Check if normalization is being applied correctly to the role name in the query.
- [x] **Task: Conductor - User Manual Verification 'Phase 1: Diagnosis and Reproduction' (Protocol in workflow.md)**

## Phase 2: Comprehensive Identity Unit Testing
- [x] **Task: Test User Creation**
    - [x] Write unit tests to verify that a new user can be created and saved to RavenDB correctly.
- [x] **Task: Test Roles and Claims Management**
    - [x] Write unit tests to verify role creation, claim assignment to roles, and role retrieval.
- [x] **Task: Test User Updates**
    - [x] Write unit tests to verify that user profile information and claims can be updated.
- [x] **Task: Test User Deletion**
    - [x] Write unit tests to verify that a user can be removed from RavenDB.
- [x] **Task: Conductor - User Manual Verification 'Phase 2: Comprehensive Identity Unit Testing' (Protocol in workflow.md)**

## Phase 3: Implement Fix and Verify Claims Sync
- [x] **Task: Fix Role Retrieval in `RoleStore`**
    - [x] Implement the necessary fix to ensure `FindByNameAsync` correctly retrieves the role from RavenDB (Green phase).
    - [x] Rerun the reproduction test to ensure it passes.
- [x] **Task: Verify `IdentitySecurity` Claims Sync**
    - [x] Write a test in `Aero.Cms.AspNetCore.Identity.Tests` (if it exists) that mocks `UserManager` and `RoleManager` (or uses an in-memory RavenDB) to verify `SyncRoleClaimsToUserAsync` updates user claims.
    - [x] Confirm `IdentitySecurity` now correctly synchronizes claims.
- [x] **Task: Conductor - User Manual Verification 'Phase 3: Implement Fix and Verify Claims Sync' (Protocol in workflow.md)**

## Phase 4: Final Verification and Accessibility
- [x] **Task: Verify Manager Access**
    - [x] Perform a manual verification by logging into the Piranha Manager with an admin user.
    - [x] Confirm the 403 Forbidden error is resolved.
- [x] **Task: Conductor - User Manual Verification 'Phase 4: Final Verification and Accessibility' (Protocol in workflow.md)**
