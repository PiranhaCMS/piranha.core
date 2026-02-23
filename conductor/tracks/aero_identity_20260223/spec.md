# Track Specification: Implement RavenDB Identity Provider (Aero.Identity)

## Overview
This track focuses on creating a custom ASP.NET Core Identity implementation using RavenDB as the data store. This provider will serve as the foundation for authentication and authorization across the modernized Piranha CMS framework.

## Scope
- Develop `UserStore` and `RoleStore` implementations for RavenDB.
- Define RavenDB document models for `IdentityUser` and `IdentityRole` equivalents.
- Integrate with standard ASP.NET Core Identity `UserManager`, `RoleManager`, and `SignInManager`.
- Ensure compatibility with Piranha CMS's existing security interfaces.

## Technical Details
- **Project**: `Aero.Identity`
- **Data Store**: RavenDB
- **Target Runtime**: .NET 10.0
- **Primary Dependencies**: `Raven.Client.Documents`, `Microsoft.Extensions.Identity.Core`

## Implementation Details
- Standardized stores for RavenDB to handle CRUD operations on users and roles.
- Support for common Identity features:
    - User/Role management.
    - Password hashing and verification.
    - External login support (via RavenDB tokens/logins).
    - Claims-based authorization.
