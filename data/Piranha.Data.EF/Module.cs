/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend;

namespace Piranha.Data.EF;

/// <summary>
/// The EF data module.
/// </summary>
public class Module : IModule
{
    /// <summary>
    /// Gets the shared mapper instance used throughout the EF data layer.
    /// </summary>
    public static IPiranhaMapper Mapper { get; } = new PiranhaMapper();

    /// <summary>
    /// Gets the Author
    /// </summary>
    public string Author => "Piranha";

    /// <summary>
    /// Gets the Name
    /// </summary>
    public string Name => "Piranha.Data.EF";

    /// <summary>
    /// Gets the Version
    /// </summary>
    public string Version => Piranha.Utils.GetAssemblyVersion(GetType().Assembly);

    /// <summary>
    /// Gets the description
    /// </summary>
    public string Description => "Data implementation for Entity Framework Core.";

    /// <summary>
    /// Gets the package url.
    /// </summary>
    public string PackageUrl => "https://www.nuget.org/packages/Piranha.Data.EF";

    /// <summary>
    /// Gets the icon url.
    /// </summary>
    public string IconUrl => "https://piranhacms.org/assets/twitter-shield.png";

    /// <summary>
    /// Initializes the module.
    /// </summary>
    public void Init() { }
}
