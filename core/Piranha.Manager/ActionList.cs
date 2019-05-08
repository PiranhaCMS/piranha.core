/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using System.Linq;

namespace Piranha.Manager
{
    /// <summary>
    /// Static class for defining the manager menu.
    /// </summary>
    public class ActionList : List<ActionItem>
    {
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