/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Piranha.Runtime;

public sealed class AppMethod
{
    /// <summary>
    /// Gets/sets the method to invoke.
    /// </summary>
    public MethodInfo Method { get; set; }

    /// <summary>
    /// Gets/sets if the method is async.
    /// </summary>
    public bool IsAsync { get; set; }

    /// <summary>
    /// Gets/sets the parameter types.
    /// </summary>
    public IList<Type> ParameterTypes { get; set; } = new List<Type>();

    /// <summary>
    /// Invokes the current method on the given instance.
    /// </summary>
    /// <param name="instance">The object instance</param>
    /// <param name="scope">The current service scope</param>
    /// <returns>An async taks</returns>
    public async Task InvokeAsync(object instance, IServiceScope scope)
    {
        var param = new List<object>();
        foreach (var type in ParameterTypes)
        {
            param.Add(scope.ServiceProvider.GetService(type));
        }

        if (IsAsync)
        {
            await ((Task)Method.Invoke(instance, param.ToArray()))
                .ConfigureAwait(false);
        }
        else
        {
            Method.Invoke(instance, param.ToArray());
        }
    }
}
