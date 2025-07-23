/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.ComponentModel.DataAnnotations;

namespace Piranha.Models;

public class Language
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets/sets the display title.
    /// </summary>
    [Required]
    [StringLength(64)]
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the optional culture.
    /// </summary>
    public string Culture { get; set; }

    /// <summary>
    /// Gets/sets if this is the default language.
    /// </summary>
    public bool IsDefault { get; set; }
}
