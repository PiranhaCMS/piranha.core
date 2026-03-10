# Implementation Plan: `plan.md`

## Phase 1: Diagnosis and Reproduction
- [ ] **Task: Write Failing Tests for Role Retrieval**
    - [ ] Create a new test in `Aero.Identity.Tests` that creates a role and then attempts to retrieve it by name via `RoleManager.FindByNameAsync`.
    - [ ] Confirm the test fails (Red phase).
- [ ] **Task: Investigate `RoleStore` Implementation**
    - [ ] Examine `Aero.Identity/Stores/RoleStore.cs` (or similar) to see how `FindByNameAsync` is implemented.
    - [ ] Check if normalization is being applied correctly to the role name in the query.
- [ ] **Task: Conductor - User Manual Verification 'Phase 1: Diagnosis and Reproduction' (Protocol in workflow.md)**

## Phase 2: Implement Fix and Verify Claims Sync
- [ ] **Task: Fix Role Retrieval in `RoleStore`**
    - [ ] Implement the necessary fix to ensure `FindByNameAsync` correctly retrieves the role from RavenDB (Green phase).
    - [ ] Rerun the reproduction test to ensure it passes.
- [ ] **Task: Verify `IdentitySecurity` Claims Sync**
    - [ ] Write a test in `Aero.Cms.AspNetCore.Identity.Tests` (if it exists) that mocks `UserManager` and `RoleManager` (or uses an in-memory RavenDB) to verify `SyncRoleClaimsToUserAsync` updates user claims.
    - [ ] Confirm `IdentitySecurity` now correctly synchronizes claims.
- [ ] **Task: Conductor - User Manual Verification 'Phase 2: Implement Fix and Verify Claims Sync' (Protocol in workflow.md)**

## Phase 3: Final Verification and Accessibility
- [ ] **Task: Verify Manager Access**
    - [ ] Perform a manual verification by logging into the Piranha Manager with an admin user.
    - [ ] Confirm the 403 Forbidden error is resolved.
- [ ] **Task: Conductor - User Manual Verification 'Phase 3: Final Verification and Accessibility' (Protocol in workflow.md)**
