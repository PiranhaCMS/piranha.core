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
using Piranha;
using Piranha.SkiaSharp;

/// <summary>
/// 
/// </summary>
public static class SkiaSharpExtensions
{
    /// <summary>
    /// Enables UseSkiaSharp
    /// </summary>
    /// <param name="serviceBuilder"></param>
    /// <param name="scope"></param>
    /// <returns></returns>
    public static PiranhaServiceBuilder UseSkiaSharp(this PiranhaServiceBuilder serviceBuilder,
        ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        serviceBuilder.Services.AddPiranhaImageSharp(scope);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the services for the ImageSharp service.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="scope">The optional service scope</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddPiranhaImageSharp(this IServiceCollection services,
        ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        services.Add(new ServiceDescriptor(typeof(IImageProcessor), typeof(SkiaSharpProcessor), scope));

        return services;
    }
}
