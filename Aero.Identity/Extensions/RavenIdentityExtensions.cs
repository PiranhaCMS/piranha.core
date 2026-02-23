using Aero.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aero.Identity;

/// <summary>
/// Contains extension methods for registering RavenDB-based Identity stores.
/// </summary>
public static class RavenIdentityExtensions
{
    /// <summary>
    /// Adds RavenDB implementations of Identity stores.
    /// </summary>
    /// <param name="builder">The Identity builder.</param>
    /// <returns>The Identity builder.</returns>
    public static IdentityBuilder AddRavenDbStores(this IdentityBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        AddStores(builder.Services, builder.UserType, builder.RoleType);

        return builder;
    }

    private static void AddStores(IServiceCollection services, Type userType, Type? roleType)
    {
        if (userType == null) throw new ArgumentNullException(nameof(userType));

        var userStoreType = typeof(RavenUserStore<>).MakeGenericType(userType);

        if (roleType != null)
        {
            var roleStoreType = typeof(RavenRoleStore<>).MakeGenericType(roleType);

            services.TryAddScoped(
                typeof(IUserStore<>).MakeGenericType(userType),
                userStoreType);

            services.TryAddScoped(
                typeof(IRoleStore<>).MakeGenericType(roleType),
                roleStoreType);
        }
        else
        {
            services.TryAddScoped(
                typeof(IUserStore<>).MakeGenericType(userType),
                userStoreType);
        }
    }
}
