/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

namespace Piranha.Data
{
    public sealed class DbMigration
    {
        /// <summary>
        /// Gets/sets the migration name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the migration script.
        /// </summary>
        public string Script { get; set; }
    }
}
