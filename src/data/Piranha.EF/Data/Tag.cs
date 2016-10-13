/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.EF.Data
{
    public class Tag : IModel, ISlug, ICreated, IModified
    {
        #region Properties
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        #endregion
    }
}
