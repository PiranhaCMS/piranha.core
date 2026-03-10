# Implementation Plan: `plan.md`

## Phase 1: Setup Integration Test Project
- [x] **Task: Create `test/Aero.Cms.Manager.IntegrationTests` project** [f0fe306]
    - [ ] Create `.csproj` with references to `MvcWeb`, `Alba`, `xunit`, `Raven.TestDriver`.
- [x] **Task: Set up `AlbaHost` for Integration Testing** [7df246a]
    - [ ] Create a custom `AlbaHost` setup in the test project that targets `MvcWeb`.
    - [ ] Configure the host to use a test RavenDB store (Raven.TestDriver) and ensure `IdentityWithSeed` is called.
- [x] **Task: Conductor - User Manual Verification 'Phase 1: Setup' (Protocol in workflow.md)** [e2b73ee]

## Phase 2: Implement Manager Login Tests
- [x] **Task: Test Admin Login with Alba `FormData`** [bf58fc4]
    - [ ] Execute `Seed()` within the test host initialization or at the start of the test.
    - [ ] Implement a test using `host.Scenario(...)`.
    - [ ] Use `_.Post.FormData(loginModel).ToUrl("/manager/login")` to simulate the login POST.
    - [ ] Verify the response status code using `_.StatusCodeShouldBe(302)` (redirection to `/manager` or the auth endpoint).
- [x] **Task: Test Authorized Access and Permissions**
    - [x] Perform a subsequent request (or within the same session if Alba supports it, otherwise use authentication helpers) to `/manager`.
    - [x] Verify the response using `_.StatusCodeShouldBeOk()` (200 OK).
    - [x] Verify the admin user's identity has `SysAdmin` and `Admin` roles and correct claims.
- [ ] **Task: Conductor - User Manual Verification 'Phase 2: Implement' (Protocol in workflow.md)**
