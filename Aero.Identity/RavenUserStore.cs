using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Aero.Identity;

/// <summary>
/// RavenDB store for users.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
public class RavenUserStore<TUser> : 
    IUserStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserSecurityStampStore<TUser>
    where TUser : IdentityUser, new()
{
    private readonly IAsyncDocumentSession _session;

    /// <summary>
    /// Initializes a new instance of the RavenUserStore.
    /// </summary>
    /// <param name="session">The RavenDB session.</param>
    public RavenUserStore(IAsyncDocumentSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    /// <inheritdoc />
    public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        await _session.StoreAsync(user, cancellationToken);
        return IdentityResult.Success;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null) throw new ArgumentNullException(nameof(user));

        _session.Delete(user.Id);
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
    public void Dispose()
    {
        // Session is injected and should be disposed by the caller or DI container.
    }
}
