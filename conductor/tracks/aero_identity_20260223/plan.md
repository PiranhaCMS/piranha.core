# Implementation Plan: Implement RavenDB Identity Provider (Aero.Identity)

## Phase 1: Scaffolding and Models [checkpoint: e9bb809]
- [x] Task: Set up `Aero.Identity` project structure and target .NET 10.0 118c35d
- [x] Task: Define `RavenUser` and `RavenRole` document models for RavenDB c4a8a87
    - [x] Include `Passkeys` list in `RavenUser`
- [x] Task: Conductor - User Manual Verification 'Phase 1: Scaffolding and Models' (Protocol in workflow.md) e9bb809

## Phase 2: RavenDB Stores (Core & Security) [checkpoint: 123ca89]
- [x] Task: Implement `RavenUserStore` (Part 1: Basic & Security) 09283df
    - [x] `IUserStore`, `IUserPasswordStore`, `IUserSecurityStampStore`
    - [x] **Exhaustive Testing**: Achieve 100% coverage for user CRUD and security stamps (including failure modes and concurrency).
- [x] Task: Implement `RavenUserStore` (Part 2: Communication & Roles) 7530ce0
    - [x] `IUserEmailStore`, `IUserPhoneNumberStore`, `IUserRoleStore`
    - [x] **Exhaustive Testing**: Achieve 100% coverage for email, phone, and role mapping (including unique constraints and normalization).
- [x] Task: Implement `RavenRoleStore` 7530ce0
    - [x] `IRoleStore` implementation
    - [x] **Exhaustive Testing**: Achieve 100% coverage for role CRUD and validation logic.
- [x] Task: Conductor - User Manual Verification 'Phase 2: RavenDB Stores (Core & Security)' (Protocol in workflow.md) 123ca89

## Phase 3: Advanced Identity Features [checkpoint: 6ac7d4d]
- [x] Task: Implement `RavenUserStore` (Part 3: Logins & Claims) 09e2b53
    - [x] `IUserLoginStore`, `IUserClaimStore`
    - [x] **Exhaustive Testing**: Achieve 100% coverage for external logins and claims (including duplicate logins and claim filtering).
- [x] Task: Implement `RavenUserStore` (Part 4: 2FA & Passkeys) 09e2b53
    - [x] `IUserTwoFactorStore`, `IUserAuthenticatorKeyStore`, `IUserTwoFactorRecoveryCodeStore`
    - [x] `IUserPasskeyStore` (ASP.NET Core 10 FIDO2/WebAuthn)
    - [x] **Exhaustive Testing**: Achieve 100% coverage for 2FA, TOTP keys, recovery codes, and passkey management.
- [x] Task: Conductor - User Manual Verification 'Phase 3: Advanced Identity Features' (Protocol in workflow.md) 6ac7d4d

## Phase 4: Identity Service Integration
- [ ] Task: Implement Identity Service Registration extensions
    - [ ] Implement `.AddRavenDbIdentity<TUser, TRole>()` extension methods
    - [ ] **Exhaustive Testing**: Achieve 100% coverage for DI registration and service configuration.
- [ ] Task: Integrate with `SignInManager` and `UserManager`
- [ ] Task: Conductor - User Manual Verification 'Phase 4: Identity Service Integration' (Protocol in workflow.md)

## Phase 5: Piranha CMS Security Bridge
- [ ] Task: Implement the security bridge for Piranha's built-in manager
    - [ ] Implement bridge logic between `Aero.Identity` and Piranha security models
    - [ ] **Exhaustive Testing**: Achieve 100% coverage for compatibility layer and manager UI integration.
- [ ] Task: Conductor - User Manual Verification 'Phase 5: Piranha CMS Security Bridge' (Protocol in workflow.md)

## Quality Gate: Final Coverage Verification
- [ ] Task: Run full solution coverage report and confirm 100% coverage for `Aero.Identity`
