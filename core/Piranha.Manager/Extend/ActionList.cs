/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using System.Linq;

namespace Piranha.Manager.Extend
{
    public class ActionList<T> : List<T> where T : IAction
    {
        /// <summary>
        /// Removes the item with the given internal id.
        /// </summary>
        /// <param name="internalId">The internal id</param>
        public void Remove(string internalId)
        {
            var item = this.FirstOrDefault(i => i.InternalId == internalId);

            if (item != null)
            {
                this.Remove(item);
            }
        }
    }
}