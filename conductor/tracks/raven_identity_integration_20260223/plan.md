# Implementation Plan: RavenDB Identity Integration

## Phase 1: Module and Extensions
- [ ] Task: Implement `RavenIdentityModule` in `Aero.Identity`
    - [ ] Define module metadata (Name, Description, etc.)
    - [ ] Register Manager permissions (Users, Roles CRUD)
    - [ ] Add Manager menu items for Users and Roles
- [ ] Task: Create `AddPiranhaRavenDbIdentity` DI extensions
    - [ ] Implement service registration for stores and managers
    - [ ] Implement authorization policy configuration
    - [ ] Implement module registration logic
- [ ] Task: Conductor - User Manual Verification 'Phase 1: Module and Extensions' (Protocol in workflow.md)

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
