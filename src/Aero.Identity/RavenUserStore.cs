using Aero.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Aero.Identity;

/// <summary>
/// RavenDB store for users.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
public class RavenUserStore<TUser> : RavenUserStore<TUser, RavenRole>
    where TUser : RavenUser, new()
{
    /// <summary>
    /// Initializes a new instance of the RavenUserStore.
    /// </summary>
    /// <param name="session">The RavenDB session.</param>
    public RavenUserStore(IAsyncDocumentSession session) : base(session)
    {
    }
}

/// <summary>
/// RavenDB store for users with a specific role type.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TRole">The role type.</typeparam>
public class RavenUserStore<TUser, TRole> :
    IUserPasswordStore<TUser>,
    IUserSecurityStampStore<TUser>,
    IUserEmailStore<TUser>,
    IUserPhoneNumberStore<TUser>,
    IUserRoleStore<TUser>,
    IUserLoginStore<TUser>,
    IUserClaimStore<TUser>,
    IUserTwoFactorStore<TUser>,
    IUserAuthenticatorKeyStore<TUser>,
    IUserTwoFactorRecoveryCodeStore<TUser>,
    IUserPasskeyStore<TUser>,
    IQueryableUserStore<TUser>
    where TUser : RavenUser, new()
    where TRole : RavenRole, new()
{
    protected readonly IAsyncDocumentSession _session;

    /// <summary>
    /// Initializes a new instance of the RavenUserStore.
    /// </summary>
    /// <param name="session">The RavenDB session.</param>
    public RavenUserStore(IAsyncDocumentSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    /// <inheritdoc />
    public IQueryable<TUser> Users => _session.Query<TUser>();

    /// <inheritdoc />
    public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        await _session.StoreAsync(user, cancellationToken);
        await _session.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        _session.Delete(user.Id);
        await _session.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    /// <inheritdoc />
    public async Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _session.LoadAsync<TUser>(userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _session.Query<TUser>()
            .Customize(x => x.WaitForNonStaleResults())
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    /// <inheritdoc />
    public Task<string?> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.NormalizedUserName);
    }

    /// <inheritdoc />
    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.Id);
    }

    /// <inheritdoc />
    public Task<string?> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.UserName);
    }

    /// <inheritdoc />
    public Task SetNormalizedUserNameAsync(TUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SetUserNameAsync(TUser user, string? userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.UserName = userName;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        try
        {
            // If the user is not loaded in the current session, we store it to track it.
            if (!_session.Advanced.IsLoaded(user.Id))
            {
                await _session.StoreAsync(user, cancellationToken);
            }

            await _session.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    /// <inheritdoc />
    public Task SetPasswordHashAsync(TUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.PasswordHash);
    }

    /// <inheritdoc />
    public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
    }

    /// <inheritdoc />
    public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.SecurityStamp);
    }

    /// <inheritdoc />
    public Task SetEmailAsync(TUser user, string? email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.Email = email;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetEmailAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.Email);
    }

    /// <inheritdoc />
    public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.EmailConfirmed);
    }

    /// <inheritdoc />
    public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<TUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _session.Query<TUser>()
            .Customize(x => x.WaitForNonStaleResults())
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    /// <inheritdoc />
    public Task<string?> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.NormalizedEmail);
    }

    /// <inheritdoc />
    public Task SetNormalizedEmailAsync(TUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SetPhoneNumberAsync(TUser user, string? phoneNumber, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.PhoneNumber = phoneNumber;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.PhoneNumber);
    }

    /// <inheritdoc />
    public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    /// <inheritdoc />
    public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.PhoneNumberConfirmed = confirmed;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

        if (!user.Roles.Any(r => string.Equals(r, roleName, StringComparison.OrdinalIgnoreCase)))
        {
            user.Roles.Add(roleName);

            // Denormalized: copy role claims to user claims
            var normalizedRoleName = roleName.ToUpperInvariant();
            var role = await _session.Query<TRole>()
                .Customize(x => x.WaitForNonStaleResults())
                .FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName, cancellationToken);

            if (role != null)
            {
                foreach (var roleClaim in role.Claims)
                {
                    if (!string.IsNullOrEmpty(roleClaim.ClaimType) && !string.IsNullOrEmpty(roleClaim.ClaimValue))
                    {
                        if (!user.Claims.Any(uc => uc.ClaimType == roleClaim.ClaimType && uc.ClaimValue == roleClaim.ClaimValue))
                        {
                            user.Claims.Add(new RavenUserClaim
                            {
                                ClaimType = roleClaim.ClaimType,
                                ClaimValue = roleClaim.ClaimValue
                            });
                        }
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

        var existing = user.Roles.FirstOrDefault(r => string.Equals(r, roleName, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            user.Roles.Remove(existing);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult<IList<string>>(user.Roles);
    }

    /// <inheritdoc />
    public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

        return Task.FromResult(user.Roles.Any(r => string.Equals(r, roleName, StringComparison.OrdinalIgnoreCase)));
    }

    /// <inheritdoc />
    public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

        return await _session.Query<TUser>()
            .Where(u => u.Roles.Any(r => r == roleName))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (login == null) throw new ArgumentNullException(nameof(login));

        if (!user.Logins.Any(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey))
        {
            user.Logins.Add(new RavenUserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName
            });
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        var login = user.Logins.FirstOrDefault(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
        if (login != null)
        {
            user.Logins.Remove(login);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult<IList<UserLoginInfo>>(user.Logins
            .Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName)).ToList());
    }

    /// <inheritdoc />
    public async Task<TUser?> FindByLoginAsync(string loginProvider, string providerKey,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _session.Advanced.AsyncDocumentQuery<TUser>()
            .WaitForNonStaleResults()
            .WhereEquals("Logins[].LoginProvider", loginProvider)
            .AndAlso()
            .WhereEquals("Logins[].ProviderKey", providerKey)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddClaimsAsync(TUser user, IEnumerable<System.Security.Claims.Claim> claims,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (claims == null) throw new ArgumentNullException(nameof(claims));

        foreach (var claim in claims)
        {
            if (!user.Claims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value))
            {
                user.Claims.Add(new RavenUserClaim { ClaimType = claim.Type, ClaimValue = claim.Value });
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ReplaceClaimAsync(TUser user, System.Security.Claims.Claim claim, System.Security.Claims.Claim newClaim,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (claim == null) throw new ArgumentNullException(nameof(claim));
        if (newClaim == null) throw new ArgumentNullException(nameof(newClaim));

        var existingClaims = user.Claims.Where(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value).ToList();
        foreach (var existingClaim in existingClaims)
        {
            existingClaim.ClaimType = newClaim.Type;
            existingClaim.ClaimValue = newClaim.Value;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveClaimsAsync(TUser user, IEnumerable<System.Security.Claims.Claim> claims,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (claims == null) throw new ArgumentNullException(nameof(claims));

        foreach (var claim in claims)
        {
            var existingClaim =
                user.Claims.FirstOrDefault(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
            if (existingClaim != null)
            {
                user.Claims.Remove(existingClaim);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));
        return Task.FromResult<IList<System.Security.Claims.Claim>>(user.Claims
            .Select(c => new System.Security.Claims.Claim(c.ClaimType, c.ClaimValue)).ToList());
    }

    /// <inheritdoc />
    public async Task<IList<TUser>> GetUsersForClaimAsync(System.Security.Claims.Claim claim,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (claim == null) throw new ArgumentNullException(nameof(claim));

        return await _session.Advanced.AsyncDocumentQuery<TUser>()
            .WaitForNonStaleResults()
            .WhereEquals("Claims[].ClaimType", claim.Type)
            .AndAlso()
            .WhereEquals("Claims[].ClaimValue", claim.Value)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        user.TwoFactorEnabled = enabled;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.TwoFactorEnabled);
    }

    /// <inheritdoc />
    public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        user.AuthenticatorKey = key;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.AuthenticatorKey);
    }

    /// <inheritdoc />
    public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        user.RecoveryCodes = string.Join(";", recoveryCodes);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        var codes = (user.RecoveryCodes ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
        if (codes.Remove(code))
        {
            user.RecoveryCodes = string.Join(";", codes);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        var codes = (user.RecoveryCodes ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries);
        return Task.FromResult(codes.Length);
    }

    /// <inheritdoc />
    public Task AddOrUpdatePasskeyAsync(TUser user, UserPasskeyInfo passkey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (passkey == null) throw new ArgumentNullException(nameof(passkey));

        var existing = user.Passkeys.FirstOrDefault(p => p.CredentialId.SequenceEqual(passkey.CredentialId));
        if (existing != null)
        {
            existing.PublicKey = passkey.PublicKey;
            existing.SignCount = passkey.SignCount;
            existing.CreatedAt = passkey.CreatedAt;
            existing.Transports = passkey.Transports?.ToArray();
            existing.IsBackupEligible = passkey.IsBackupEligible;
            existing.IsBackedUp = passkey.IsBackedUp;
            existing.IsUserVerified = passkey.IsUserVerified;
            existing.ClientDataJson = passkey.ClientDataJson;
            existing.AttestationObject = passkey.AttestationObject;
            existing.Name = passkey.Name;
        }
        else
        {
            user.Passkeys.Add(new PasskeyCredential
            {
                CredentialId = passkey.CredentialId,
                PublicKey = passkey.PublicKey,
                CreatedAt = passkey.CreatedAt,
                SignCount = passkey.SignCount,
                Transports = passkey.Transports?.ToArray(),
                IsBackupEligible = passkey.IsBackupEligible,
                IsBackedUp = passkey.IsBackedUp,
                IsUserVerified = passkey.IsUserVerified,
                ClientDataJson = passkey.ClientDataJson,
                AttestationObject = passkey.AttestationObject,
                Name = passkey.Name
            });
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IList<UserPasskeyInfo>> GetPasskeysAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        return Task.FromResult<IList<UserPasskeyInfo>>(user.Passkeys.Select(p => new UserPasskeyInfo(
            p.CredentialId,
            p.PublicKey,
            p.CreatedAt,
            p.SignCount,
            p.Transports,
            p.IsBackupEligible,
            p.IsBackedUp,
            p.IsUserVerified,
            p.ClientDataJson,
            p.AttestationObject) { Name = p.Name }).ToList());
    }

    /// <inheritdoc />
    public Task RemovePasskeyAsync(TUser user, byte[] credentialId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        var passkey = user.Passkeys.FirstOrDefault(p => p.CredentialId.SequenceEqual(credentialId));
        if (passkey != null)
        {
            user.Passkeys.Remove(passkey);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<TUser?> FindByPasskeyIdAsync(byte[] credentialId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _session.Advanced.AsyncDocumentQuery<TUser>()
            .WaitForNonStaleResults()
            .WhereEquals("Passkeys[].CredentialId", credentialId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<UserPasskeyInfo?> FindPasskeyAsync(TUser user, byte[] credentialId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        var p = user.Passkeys.FirstOrDefault(p => p.CredentialId.SequenceEqual(credentialId));
        if (p == null) return Task.FromResult<UserPasskeyInfo?>(null);

        return Task.FromResult<UserPasskeyInfo?>(new UserPasskeyInfo(
            p.CredentialId,
            p.PublicKey,
            p.CreatedAt,
            p.SignCount,
            p.Transports,
            p.IsBackupEligible,
            p.IsBackedUp,
            p.IsUserVerified,
            p.ClientDataJson,
            p.AttestationObject) { Name = p.Name });
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Session is injected and should be disposed by the caller or DI container.
    }
}
