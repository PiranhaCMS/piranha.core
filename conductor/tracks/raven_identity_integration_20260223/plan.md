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

## Phase 2: TODO Resolution (Solution-Wide)
- [ ] Task: Scan solution for RavenDB-related `// TODO` items
- [ ] Task: Address Identity-related TODOs
- [ ] Task: Address Repository/Core-related TODOs (where applicable for integration foundation)
- [ ] Task: Conductor - User Manual Verification 'Phase 2: TODO Resolution' (Protocol in workflow.md)

## Phase 3: Startup Integration and Verification
- [ ] Task: Integrate `AddPiranhaRavenDbIdentity` into a sample application (e.g., `MvcWeb`)
- [ ] Task: Verify User/Role management in Piranha Manager
- [ ] Task: Verify Login/Logout flows
- [ ] Task: Conductor - User Manual Verification 'Phase 3: Startup Integration and Verification' (Protocol in workflow.md)

## Quality Gate: Final Verification
- [ ] Task: Run full integration test suite and confirm 100% coverage for new integration logic
- [ ] Task: Verify no remaining RavenDB // TODOs exist in the codebase
