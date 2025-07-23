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

[Serializable]
public class MediaFolder
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets/sets the optional parent id.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Gets/sets the folder name.
    /// </summary>
    [Required]
    [StringLength(128)]
    public string Name { get; set; }

    /// <summary>
    /// Gets/sets the optional description.
    /// </summary>
    [StringLength(512)]
    public string Description { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }
}

[Serializable]
public class MediaFolderSimple
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets/sets the folder name.
    /// </summary>
    [Required]
    [StringLength(128)]
    public string Name { get; set; }
}
