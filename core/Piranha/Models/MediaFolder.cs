/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    [Serializable]
    public sealed class MediaFolder : MediaFolder<Guid>
    {
        /// <summary>
        /// Gets/sets the optional parent id.
        /// </summary>
        public Guid? ParentId { get; set; }
    }

    [Serializable]
    public abstract class MediaFolder<TKey>
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public TKey Id { get; set; }

        /// <summary>
        /// Gets/sets the folder name.
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }
    }
}