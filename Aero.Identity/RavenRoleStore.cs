using Aero.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Aero.Identity;

/// <summary>
/// RavenDB store for roles.
/// </summary>
/// <typeparam name="TRole">The role type.</typeparam>
public class RavenRoleStore<TRole> : 
    IRoleStore<TRole>
    where TRole : RavenRole, new()
{
    private readonly IAsyncDocumentSession _session;

    /// <summary>
    /// Initializes a new instance of the RavenRoleStore.
    /// </summary>
    /// <param name="session">The RavenDB session.</param>
    public RavenRoleStore(IAsyncDocumentSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    /// <inheritdoc />
    public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null) throw new ArgumentNullException(nameof(role));

        await _session.StoreAsync(role, cancellationToken);
        return IdentityResult.Success;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null) throw new ArgumentNullException(nameof(role));

        _session.Delete(role.Id);
        return IdentityResult.Success;
    }

    /// <inheritdoc />
    public async Task<TRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _session.LoadAsync<TRole>(roleId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _session.Query<TRole>()
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName, cancellationToken);
    }

    /// <inheritdoc />
    public Task<string?> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(role.NormalizedName);
    }

    /// <inheritdoc />
    public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(role.Id);
    }

    /// <inheritdoc />
    public Task<string?> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(role.Name);
    }

    /// <inheritdoc />
    public Task SetNormalizedRoleNameAsync(TRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        role.NormalizedName = normalizedName;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SetRoleNameAsync(TRole role, string? roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        role.Name = roleName;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null) throw new ArgumentNullException(nameof(role));

        try
        {
            if (!_session.Advanced.IsLoaded(role.Id))
            {
                await _session.StoreAsync(role, cancellationToken);
            }

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Session is injected and should be disposed by the caller or DI container.
    }
}
