/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.Extend
{
    public interface IAction
    {
        /// <summary>
        /// Gets/sets the internal id of the action.
        /// </summary>
        string InternalId { get; set; }
    }
}