/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.Models;

/// <summary>
/// Modules model.
/// </summary>
public class ModuleListModel
{
    /// <summary>
    /// A list item in the module model.
    /// </summary>
    public class ModuleItem
    {
        public string Author { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string PackageUrl { get; set; }
        public string IconUrl { get; set; }
    }

    /// <summary>
    /// Gets/set the available items.
    /// </summary>
    public IList<ModuleItem> Items { get; set; } = new List<ModuleItem>();
}
