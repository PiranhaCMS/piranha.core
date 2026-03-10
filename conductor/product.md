# Initial Concept
Porting Piranha CMS to RavenDB, replacing EF Core with a document-oriented architecture, and building a custom Identity provider in `Aero.Identity`.

# Product Vision
To provide a modern, high-performance, document-oriented backend for Piranha CMS using RavenDB. This transformation will modernize the data access layer and identity management, leveraging the flexibility and performance of RavenDB while maintaining the core Piranha extensibility model.

# Key Goals
1.  **RavenDB Identity Provider**: Develop a robust, highly reliable ASP.NET Core Identity implementation in `Aero.Identity`, verified by comprehensive unit testing, supporting standard authentication, external providers, and modern FIDO2/WebAuthn passkeys.
2.  **RavenDB-Driven CMS Port**: Migrate all core Piranha repositories and services to use RavenDB, maintaining parity with existing interfaces.
3.  **EF Core Elimination**: Complete removal of Entity Framework Core dependencies from the entire framework and example projects.
4.  **Modern Infrastructure**: Upgrade the entire solution to .NET 10.0 and modern C# conventions.

# Core Requirements
- **Standardized Identity**: Support for User, Role, External Provider, and Passkey authentication via RavenDB.
- **Interface Stability**: Maintain existing Piranha API and Repository interfaces to minimize breaking changes for downstream consumers.
- **Modern Runtime**: Fully optimized for .NET 10.0 performance and features.

# Target Stack
- **Runtime**: .NET 10.0
- **Database**: RavenDB
- **Framework**: ASP.NET Core
- **Identity**: RavenDB-based custom implementation (`Aero.Identity`)
