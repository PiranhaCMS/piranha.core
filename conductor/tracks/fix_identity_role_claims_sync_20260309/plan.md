# Implementation Plan: `plan.md`

## Phase 1: Diagnosis and Reproduction
- [ ] **Task: Write Failing Tests for Role Retrieval**
    - [ ] Create a new test in `Aero.Identity.Tests` that creates a role and then attempts to retrieve it by name via `RoleManager.FindByNameAsync`.
    - [ ] Confirm the test fails (Red phase).
- [ ] **Task: Investigate `RoleStore` Implementation**
    - [ ] Examine `Aero.Identity/Stores/RoleStore.cs` (or similar) to see how `FindByNameAsync` is implemented.
    - [ ] Check if normalization is being applied correctly to the role name in the query.
- [ ] **Task: Conductor - User Manual Verification 'Phase 1: Diagnosis and Reproduction' (Protocol in workflow.md)**

## Phase 2: Comprehensive Identity Unit Testing
- [ ] **Task: Test User Creation**
    - [ ] Write unit tests to verify that a new user can be created and saved to RavenDB correctly.
- [ ] **Task: Test Roles and Claims Management**
    - [ ] Write unit tests to verify role creation, claim assignment to roles, and role retrieval.
- [ ] **Task: Test User Updates**
    - [ ] Write unit tests to verify that user profile information and claims can be updated.
- [ ] **Task: Test User Deletion**
    - [ ] Write unit tests to verify that a user can be removed from RavenDB.
- [ ] **Task: Conductor - User Manual Verification 'Phase 2: Comprehensive Identity Unit Testing' (Protocol in workflow.md)**

## Phase 3: Implement Fix and Verify Claims Sync
- [ ] **Task: Fix Role Retrieval in `RoleStore`**
    - [ ] Implement the necessary fix to ensure `FindByNameAsync` correctly retrieves the role from RavenDB (Green phase).
    - [ ] Rerun the reproduction test to ensure it passes.
- [ ] **Task: Verify `IdentitySecurity` Claims Sync**
    - [ ] Write a test in `Aero.Cms.AspNetCore.Identity.Tests` (if it exists) that mocks `UserManager` and `RoleManager` (or uses an in-memory RavenDB) to verify `SyncRoleClaimsToUserAsync` updates user claims.
    - [ ] Confirm `IdentitySecurity` now correctly synchronizes claims.
- [ ] **Task: Conductor - User Manual Verification 'Phase 3: Implement Fix and Verify Claims Sync' (Protocol in workflow.md)**

## Phase 4: Final Verification and Accessibility
- [ ] **Task: Verify Manager Access**
    - [ ] Perform a manual verification by logging into the Piranha Manager with an admin user.
    - [ ] Confirm the 403 Forbidden error is resolved.
- [ ] **Task: Conductor - User Manual Verification 'Phase 4: Final Verification and Accessibility' (Protocol in workflow.md)**
