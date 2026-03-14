# Track Specification: RavenDB Identity Integration

## Overview
This track focuses on the deep integration of the `Aero.Identity` RavenDB provider into the Piranha CMS framework. It involves creating the necessary Piranha Module, DI registration extensions, and addressing solution-wide RavenDB integration `// TODO` items.

## Functional Requirements
- **RavenIdentityModule**: Implement a Piranha `IModule` in `Aero.Identity` that registers Identity-related permissions and manager menu items.
- **DI Registration**: Create `AddPiranhaRavenDbIdentity` extension methods to:
    - Register RavenDB `IDocumentStore` and `IAsyncDocumentSession`.
    - Register `RavenUserStore` and `RavenRoleStore`.
    - Configure standard Piranha Security policies (Roles, Users CRUD).
    - Register the `RavenIdentityModule` in the Piranha application context.
- **Startup Integration**: Update the example project(s) or core startup logic to use the new RavenDB Identity provider.
- **TODO Resolution**: Scan and resolve all `// TODO` comments related to RavenDB integration across the entire solution.

## Non-Functional Requirements
- **Parity**: Maintain functional parity with the existing EF Core Identity implementation.
- **Clean Architecture**: Ensure the integration follows Piranha's modular architecture and doesn't introduce circular dependencies.
- **Stability**: Resolve all identified technical debt items (TODOs) to ensure a stable document-oriented foundation.

## Acceptance Criteria
- Piranha Manager allows listing, adding, and editing users/roles using RavenDB.
- Login and logout functionality works correctly via the new `RavenIdentitySecurity` bridge.
- All RavenDB-related `// TODO` items are addressed or converted to tracked issues.
- The solution compiles and passes all integration tests.

## Out of Scope
- Migrating existing user data from EF Core to RavenDB.
- Implementing non-Identity RavenDB repositories (to be handled in a separate track).
