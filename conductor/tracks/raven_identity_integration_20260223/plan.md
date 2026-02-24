# Implementation Plan: RavenDB Identity Integration

## Phase 1: Module and Extensions [checkpoint: 77786d8]
- [x] Task: Implement `RavenIdentityModule` in `Aero.Identity` 7cec895
    - [x] Define module metadata (Name, Description, etc.)
    - [x] Register Manager permissions (Users, Roles CRUD)
    - [x] Add Manager menu items for Users and Roles
- [x] Task: Create `AddPiranhaRavenDbIdentity` DI extensions 809d24b
    - [x] Implement service registration for stores and managers
    - [x] Implement authorization policy configuration
    - [x] Implement module registration logic
- [x] Task: Conductor - User Manual Verification 'Phase 1: Module and Extensions' (Protocol in workflow.md) 77786d8

## Phase 2: TODO Resolution (Solution-Wide) [checkpoint: 7cec895]
- [x] Task: Scan solution for RavenDB-related `// TODO` items 7cec895
- [x] Task: Address Identity-related TODOs 7cec895
- [x] Task: Address Repository/Core-related TODOs (where applicable for integration foundation) 7cec895
- [x] Task: Conductor - User Manual Verification 'Phase 2: TODO Resolution' (Protocol in workflow.md) 7cec895

## Phase 3: Startup Integration and Verification [checkpoint: 73d4c2e]
- [x] Task: Integrate `AddPiranhaRavenDbIdentity` into a sample application (e.g., `MvcWeb`) 73d4c2e
- [x] Task: Verify User/Role management in Piranha Manager 73d4c2e
- [x] Task: Verify Login/Logout flows 73d4c2e
- [x] Task: Conductor - User Manual Verification 'Phase 3: Startup Integration and Verification' (Protocol in workflow.md) 73d4c2e

## Quality Gate: Final Verification
- [x] Task: Run full integration test suite and confirm 100% coverage for new integration logic 73d4c2e
- [x] Task: Verify no remaining RavenDB // TODOs exist in the codebase 73d4c2e
