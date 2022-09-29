/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;

namespace Piranha.Runtime;

public sealed class AppBlock : AppDataItem
{
    /// <summary>
    /// Gets/sets the display name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets/sets the category.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Gets/sets the block icon.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Gets/sets the field that should be used to render list item title.
    /// </summary>
    public string ListTitleField { get; set; }

    /// <summary>
    /// Gets/sets if the block type should only be listed
    /// where specified explicitly.
    /// </summary>
    public bool IsUnlisted { get; set; }

    /// <summary>
    /// Gets/sets if the block should use a generic model
    /// when rendered in the manager interface.
    /// </summary>
    public bool IsGeneric { get; set; }

    /// <summary>
    /// Gets/sets the name of the component that should be
    /// used to render the block in the manager interface.
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// Gets/sets the editor width.
    /// </summary>
    public EditorWidth Width { get; set; }

    /// <summary>
    /// Gets/sets the specified item types.
    /// </summary>
    public IList<Type> ItemTypes { get; set; } = new List<Type>();

    /// <summary>
    /// Gets/sets the optional init methods.
    /// </summary>
    public AppInitMethod Init { get; set; } = new AppInitMethod();
}
