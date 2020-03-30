/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;

namespace Piranha.Manager.Models
{
    public class AsyncResult<T>
    {
        /// <summary>
        /// Gets/sets the result body.
        /// </summary>
        public T Body { get; set; }
    }

    /// <summary>
    /// Result model.
    /// </summary>
    public class AsyncResult
    {
        /// <summary>
        /// Gets/sets the status message from the last operation.
        /// </summary>
        public StatusMessage Status { get; set; }
    }
}