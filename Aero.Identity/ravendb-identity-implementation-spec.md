# RavenDB ASP.NET Identity Full Implementation Spec

This document describes how to implement a **fully-featured ASP.NET Core Identity** using **RavenDB**, including roles, claims, logins, tokens, 2FA, and **passkeys** (ASP.NET Core 10+).

---

## 1️⃣ Core ASP.NET Identity Interfaces

To implement a full Identity system, you need to implement the following interfaces:

| Interface                                        | Purpose                                                                           |
| ------------------------------------------------ | --------------------------------------------------------------------------------- |
| `IUserStore<TUser>`                              | Basic CRUD for users. Must implement user creation, deletion, update, and lookup. |
| `IRoleStore<TRole>`                              | CRUD for roles.                                                                   |
| `IUserPasswordStore<TUser>`                      | Stores password hashes, supports password verification.                           |
| `IUserEmailStore<TUser>`                         | Manages email addresses and confirmation flags.                                   |
| `IUserPhoneNumberStore<TUser>`                   | Optional; phone numbers and verification.                                         |
| `IUserLoginStore<TUser>`                         | External logins (OAuth, social logins).                                           |
| `IUserClaimStore<TUser>`                         | Claims support (authorization).                                                   |
| `IUserRoleStore<TUser>`                          | User ↔ Role mapping.                                                              |
| `IUserSecurityStampStore<TUser>`                 | Security stamp for invalidating cookies after password changes.                   |
| `IUserTwoFactorStore<TUser>`                     | Two-factor authentication support.                                                |
| `IUserAuthenticatorKeyStore<TUser>`              | Stores authenticator app key for TOTP.                                            |
| `IUserAuthenticationTokenStore<TUser>`           | For tokens like refresh tokens, auth tokens.                                      |
| `IUserDeviceStore<TUser>`                        | Device management (optional, can be for TOTP recovery).                           |
| `IUserRecoveryCodeStore<TUser>`                  | One-time recovery codes for 2FA.                                                  |
| `IUserConfirmableEmailStore<TUser>` *(optional)* | For email confirmation workflows.                                                 |
| `IUserPasskeyStore<TUser>`                       | **New in ASP.NET Core 10** — store FIDO2/WebAuthn passkeys.                       |

---

## 2️⃣ Passkey Support (ASP.NET Core 10+)

ASP.NET Core 10 introduced **passkeys** to support **FIDO2/WebAuthn**.

### `IUserPasskeyStore` methods

```csharp
Task AddPasskeyAsync(TUser user, PasskeyCredential passkey, CancellationToken cancellationToken);
Task<List<PasskeyCredential>> GetPasskeysAsync(TUser user, CancellationToken cancellationToken);
Task RemovePasskeyAsync(TUser user, string credentialId, CancellationToken cancellationToken);
```

### `PasskeyCredential` class

```csharp
public class PasskeyCredential
{
    public string CredentialId { get; set; }
    public byte[] PublicKey { get; set; }
    public long SignCount { get; set; }
    public string UserHandle { get; set; }
}
```

> In RavenDB, store this as a **list property inside the `TUser` document**.

---

## 3️⃣ Recommended Base Classes

```csharp
public class RavenUser : IdentityUser
{
    public string Id { get; set; } // RavenDB document key
    public List<PasskeyCredential> Passkeys { get; set; } = new();
}

public class RavenRole : IdentityRole
{
    public string Id { get; set; }
}
```

* Implement the interfaces in `RavenUserStore` and `RavenRoleStore` classes.

---

## 4️⃣ Minimal Interfaces to Implement for Core Features

```csharp
public class RavenUserStore : 
    IUserStore<RavenUser>,
    IUserPasswordStore<RavenUser>,
    IUserEmailStore<RavenUser>,
    IUserPhoneNumberStore<RavenUser>,
    IUserLoginStore<RavenUser>,
    IUserClaimStore<RavenUser>,
    IUserRoleStore<RavenUser>,
    IUserSecurityStampStore<RavenUser>,
    IUserTwoFactorStore<RavenUser>,
    IUserAuthenticatorKeyStore<RavenUser>,
    IUserAuthenticationTokenStore<RavenUser>,
    IUserRecoveryCodeStore<RavenUser>,
    IUserPasskeyStore<RavenUser>
{
    // Implement each method using RavenDB operations
}

public class RavenRoleStore : IRoleStore<RavenRole>
{
    // Implement role CRUD in RavenDB
}
```

---

## 5️⃣ Implementation Notes for RavenDB

* **IDs:** Use string IDs (`users/1-A`) as Identity keys.
* **Indexes:** Create RavenDB indexes on normalized username/email for fast lookups.
* **Collections:** `Users` and `Roles` collections.
* **Transactions:** Use `IDocumentSession` per request.
* **Concurrency:** Use RavenDB **ETags** for optimistic concurrency.

---

## 6️⃣ Optional Enhancements

* Device tracking for 2FA using nested documents.
* Claims & roles can be embedded in the user document or stored in separate collections.
* Token storage can be per-user or in a separate collection.
* Passkeys stored inside the user document for efficiency.

---

## 7️⃣ AI Agent Instructions to Implement

You can instruct AI agents (like GPT or other code generation agents) as follows:

```text
Task: Implement a full ASP.NET Core Identity store using RavenDB.

Requirements:
1. User and Role stores implementing all necessary interfaces.
2. Full Identity features: roles, claims, logins, tokens, 2FA.
3. ASP.NET Core 10 passkey support via IUserPasskeyStore.
4. Use RavenDB collections: Users and Roles.
5. Use RavenDB document IDs and session-based operations.
6. Implement concurrency using ETags.
7. Include indexes for normalized username/email.
8. Provide code skeletons for RavenUserStore and RavenRoleStore.
9. Include RavenUser and RavenRole base classes with Passkeys property.
10. Output ready-to-use C# files with all methods stubbed or fully implemented.
```

> ✅ Each method in the store classes should be implemented using **RavenDB async API** with `IDocumentSession` for proper async I/O and transactions.

---

**This spec can serve as a guide for developers or AI agents to implement a fully-featured ASP.NET Core Identity system backed by RavenDB, including modern passkey support.**

---

You can copy this text and save it as `RavenDB_ASPNet_Identity_FullSpec.md` to make it downloadable.

If you want, I can also create a **ready-to-use code skeleton with all interfaces implemented as stubs** for RavenDB so you can start coding immediately. Do you want me to do that next?
