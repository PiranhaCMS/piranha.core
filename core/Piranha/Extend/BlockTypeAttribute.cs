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

namespace Piranha.Extend;

/// <summary>
/// Attribute for marking a class as a block type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class BlockTypeAttribute : Attribute
{
    private bool _isGenericManuallySet = false;
    private bool _isGeneric = true;

    /// <summary>
    /// The UI component that should handle the block.
    /// </summary>
    protected string _component = "generic-block";

    /// <summary>
    /// Gets/sets the display name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets/sets the block category.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Gets/set the icon css.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Gets/sets the editor width. The default value is "Centered".
    /// </summary>
    public EditorWidth Width { get; set; } = EditorWidth.Centered;

    /// <summary>
    /// Gets/sets the field that will be used to generate the list
    /// item title if the block is used in a block group. Please note
    /// that this value is only used for generic blocks as custom blocks
    /// are responsible for emitting their title changes.
    /// </summary>
    public string ListTitle { get; set; }

    /// <summary>
    /// Gets/sets if the block type should only be listed
    /// where specified explicitly.
    /// </summary>
    public bool IsUnlisted { get; set; }

    /// <summary>
    /// Gets/sets if the block should use a generic model
    /// when rendered in the manager interface.
    /// </summary>
    public bool IsGeneric
    {
        get => _isGeneric;
        set
        {
            _isGeneric = value;
            _isGenericManuallySet = true;
        }
    }

    /// <summary>
    /// Gets/sets the name of the component that should be
    /// used to render the block in the manager interface.
    /// </summary>
    public string Component
    {
        get => _component;
        set
        {
            _component = value;

            if (!_isGenericManuallySet)
            {
                _isGeneric = value == "generic-block";
            }
        }
    }
}
