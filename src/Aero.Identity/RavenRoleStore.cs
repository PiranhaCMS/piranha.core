using Aero.Identity.Models;
using Marten;
using Microsoft.AspNetCore.Identity;



namespace Aero.Identity;

/// <summary>
/// RavenDB store for roles.
/// </summary>
/// <typeparam name="TRole">The role type.</typeparam>
public class RavenRoleStore<TRole> :
    IQueryableRoleStore<TRole>,
    IRoleClaimStore<TRole>
    where TRole : RavenRole, new()
{
    private readonly IDocumentSession _session;

    /// <summary>
    /// Initializes a new instance of the RavenRoleStore.
    /// </summary>
    /// <param name="session">The RavenDB session.</param>
    public RavenRoleStore(IDocumentSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    /// <inheritdoc />
    public IQueryable<TRole> Roles => _session.Query<TRole>();

    /// <inheritdoc />
    public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null) throw new ArgumentNullException(nameof(role));

        _session.Store(role);
        await _session.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null) throw new ArgumentNullException(nameof(role));

        _session.Delete<TRole>(role.Id);
        await _session.SaveChangesAsync(cancellationToken);
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
            //if (!_session.Advanced.IsLoaded(role.Id))
            {
                _session.Store(role);
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
    public Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(TRole role,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null) throw new ArgumentNullException(nameof(role));

        // Denormalized: claims are stored directly in the role document
        var claims = role.Claims
            .Where(c => !string.IsNullOrEmpty(c.ClaimType) && !string.IsNullOrEmpty(c.ClaimValue))
            .Select(c => new System.Security.Claims.Claim(c.ClaimType, c.ClaimValue))
            .ToList();

        return Task.FromResult<IList<System.Security.Claims.Claim>>(claims);
    }

    /// <inheritdoc />
    public Task AddClaimAsync(TRole role, System.Security.Claims.Claim claim,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null) throw new ArgumentNullException(nameof(role));
        if (claim == null) throw new ArgumentNullException(nameof(claim));

        // Denormalized: add claim to the role document
        if (!role.Claims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value))
        {
            role.Claims.Add(new RavenRoleClaim
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveClaimAsync(TRole role, System.Security.Claims.Claim claim,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null) throw new ArgumentNullException(nameof(role));
        if (claim == null) throw new ArgumentNullException(nameof(claim));

        // Denormalized: remove claim from the role document
        var existingClaims = role.Claims
            .Where(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value)
            .ToList();

        foreach (var c in existingClaims)
        {
            role.Claims.Remove(c);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Session is injected and should be disposed by the caller or DI container.
    }
}
