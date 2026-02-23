# Implementation Plan: Implement RavenDB Identity Provider (Aero.Identity)

## Phase 1: Scaffolding and Models
- [ ] Task: Set up `Aero.Identity` project structure and target .NET 10.0
- [ ] Task: Define `IdentityUser` and `IdentityRole` document models for RavenDB
- [ ] Task: Conductor - User Manual Verification 'Phase 1: Scaffolding and Models' (Protocol in workflow.md)

## Phase 2: RavenDB Stores
- [ ] Task: Implement `UserStore` for RavenDB
    - [ ] Write unit tests for `UserStore` CRUD operations
    - [ ] Implement `UserStore` logic
- [ ] Task: Implement `RoleStore` for RavenDB
    - [ ] Write unit tests for `RoleStore` CRUD operations
    - [ ] Implement `RoleStore` logic
- [ ] Task: Conductor - User Manual Verification 'Phase 2: RavenDB Stores' (Protocol in workflow.md)

## Phase 3: Identity Service Integration
- [ ] Task: Implement Identity Service Registration extensions
    - [ ] Write unit tests for service registration
    - [ ] Implement `.AddRavenDbIdentity<TUser, TRole>()` extension methods
- [ ] Task: Integrate with `SignInManager` and `UserManager`
- [ ] Task: Conductor - User Manual Verification 'Phase 3: Identity Service Integration' (Protocol in workflow.md)

## Phase 4: Piranha CMS Security Bridge
- [ ] Task: Implement the security bridge for Piranha's built-in manager
    - [ ] Write tests for Piranha security compatibility
    - [ ] Implement bridge logic between `Aero.Identity` and Piranha security models
- [ ] Task: Conductor - User Manual Verification 'Phase 4: Piranha CMS Security Bridge' (Protocol in workflow.md)
