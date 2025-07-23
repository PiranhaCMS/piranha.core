/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.DependencyInjection;

namespace Piranha.Runtime;

public sealed class AppInitMethod
{
    /// <summary>
    /// Gets/sets the init method.
    /// </summary>
    public AppMethod InitMethod { get; set; }

    /// <summary>
    /// Gets/sets the manager init method.
    /// </summary>
    public AppMethod InitManagerMethod { get; set; }

    /// <summary>
    /// Invokes the current method on the given instance.
    /// </summary>
    /// <param name="instance">The object instance</param>
    /// <param name="scope">The current service scope</param>
    /// <param name="managerInit">If manager init should be invoked</param>
    public async Task InvokeAsync(object instance, IServiceScope scope, bool managerInit)
    {
        if (!managerInit)
        {
            if (InitMethod != null)
            {
                await InitMethod.InvokeAsync(instance, scope).ConfigureAwait(false);
            }
        }
        else
        {
            if (InitManagerMethod != null)
            {
                await InitManagerMethod.InvokeAsync(instance, scope).ConfigureAwait(false);
            }
        }
    }
}
