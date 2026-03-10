# Implementation Plan: `plan.md`

## Phase 1: Setup Integration Test Project
- [ ] **Task: Create `test/Aero.Cms.Manager.IntegrationTests` project**
    - [ ] Create `.csproj` with references to `MvcWeb`, `Alba`, `xunit`, `Raven.TestDriver`.
- [ ] **Task: Set up `WebApplicationFactory` equivalent with Alba**
    - [ ] Configure `AlbaHost` to use a test RavenDB store.
- [ ] **Task: Conductor - User Manual Verification 'Phase 1: Setup' (Protocol in workflow.md)**

## Phase 2: Implement Manager Login Tests
- [ ] **Task: Test Admin Login**
    - [ ] Run `Seed()` in the test host.
    - [ ] Use Alba to POST to `/manager/login`.
    - [ ] Verify successful login and 200/302 response (depending on redirect).
- [ ] **Task: Test Permissions (Roles/Claims)**
    - [ ] Verify the admin user has `SysAdmin` and `Admin` roles.
    - [ ] Verify the admin user can access `/manager`.
- [ ] **Task: Conductor - User Manual Verification 'Phase 2: Implement' (Protocol in workflow.md)**
