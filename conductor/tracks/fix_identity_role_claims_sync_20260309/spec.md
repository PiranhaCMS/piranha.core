# Specification: `spec.md`

## Overview
The `IdentitySecurity` service in `Aero.Cms.AspNetCore.Identity` fails to synchronize role-based claims to the user during login. Specifically, in the `SyncRoleClaimsToUserAsync` method, the `RoleManager<Role>.FindByNameAsync(roleName)` call returns `null` for role names retrieved from the `UserManager`. This results in the user missing the necessary claims for authorization, leading to a 403 Forbidden error when attempting to access the manager area.

## Functional Requirements
1. **Role Retrieval Fix:** Investigate and fix the `RoleManager.FindByNameAsync` method to ensure it correctly retrieves roles from RavenDB by their normalized name.
2. **Claim Synchronization:** Ensure `SyncRoleClaimsToUserAsync` correctly adds missing claims from the retrieved role to the user object.
3. **Authentication Success:** Verify that after logging in, the user has the correct claims and can access authorized areas (e.g., the manager dashboard) without a 403 error.

## Non-Functional Requirements
1. **Performance:** Role retrieval should be efficient, utilizing RavenDB indexes where appropriate.
2. **Stability:** The fix should not introduce regressions in user or role management.

## Acceptance Criteria
1. `RoleManager.FindByNameAsync(roleName)` returns a valid `Role` object for existing roles in the database.
2. The `SyncRoleClaimsToUserAsync` method successfully updates the `User` object with role-based claims.
3. An administrative user can log in and access the Piranha Manager area without encountering a 403 Forbidden error.
4. Unit tests demonstrate that roles can be created, retrieved by name, and have their claims synced.

## Out of Scope
- Redesigning the Identity schema.
- Implementing new authentication methods (e.g., OAuth, WebAuthn) unless directly related to the role retrieval issue.
