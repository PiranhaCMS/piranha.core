/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend;

/// <summary>
/// Attribute for marking a class as a content group.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ContentGroupAttribute : Attribute
{
    private string _title;

    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets/sets the title.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            _title = value;

            if (string.IsNullOrWhiteSpace(Id))
            {
                Id = Utils.GenerateInternalId(value);
            }
        }
    }

    /// <summary>
    /// Gets/set the icon css.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Gets/sets if the content group should be hidden from the
    /// menu or not. The default value is false.
    /// </summary>
    public bool IsHidden { get; set; }
}
