# Track Specification: Implement RavenDB Identity Provider (Aero.Identity)

## Overview
This track focuses on creating a fully-featured ASP.NET Core 10 Identity implementation using RavenDB as the data store. This provider will serve as the foundation for authentication and authorization across the modernized Piranha CMS framework, including support for modern FIDO2/WebAuthn passkeys.

## Scope
- Develop `RavenUserStore` and `RavenRoleStore` implementations for RavenDB.
- Implement a comprehensive set of Identity interfaces (CRUD, Password, Email, Roles, Claims, Logins, 2FA, Tokens, Recovery Codes, and Passkeys).
- Define RavenDB document models for `RavenUser` and `RavenRole`.
- Integrate with standard ASP.NET Core 10 Identity `UserManager`, `RoleManager`, and `SignInManager`.
- Ensure compatibility with Piranha CMS's existing security interfaces.

## Technical Details
- **Project**: `Aero.Identity`
- **Data Store**: RavenDB
- **Target Runtime**: .NET 10.0
- **Primary Dependencies**: `Raven.Client.Documents`, `Microsoft.Extensions.Identity.Core`
- **Reference**: `Aero.Identity/ravendb-identity-implementation-spec.md`

## Key Interfaces to Implement
| Interface | Purpose |
| --- | --- |
| `IUserStore<TUser>` | Basic CRUD for users |
| `IRoleStore<TRole>` | CRUD for roles |
| `IUserPasswordStore<TUser>` | Password hashes and verification |
| `IUserEmailStore<TUser>` | Email management |
| `IUserLoginStore<TUser>` | External logins (OAuth) |
| `IUserClaimStore<TUser>` | Claims support |
| `IUserRoleStore<TUser>` | User ↔ Role mapping |
| `IUserSecurityStampStore<TUser>` | Security stamp management |
| `IUserTwoFactorStore<TUser>` | 2FA support |
| `IUserPasskeyStore<TUser>` | **ASP.NET Core 10 Passkeys (FIDO2/WebAuthn)** |

## Implementation Notes
- **IDs**: Use string IDs (e.g., `users/1-A`) as Identity keys.
- **Indexes**: Create RavenDB indexes on normalized username/email for fast lookups.
- **Concurrency**: Use RavenDB ETags for optimistic concurrency.
- **Passkeys**: Store as a list property inside the `RavenUser` document.
