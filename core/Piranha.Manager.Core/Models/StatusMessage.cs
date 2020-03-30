/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Status message to pass from an api to the manager interface.
    /// </summary>
    public class StatusMessage
    {
        public static readonly string Error = "danger";
        public static readonly string Information = "info";
        public static readonly string Success = "success";
        public static readonly string Warning = "warning";

        /// <summary>
        /// Gets/sets the message type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets/sets the message body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets/sets if the status message should be hidden
        /// automatically from the notification hub after a
        /// period of time.
        /// </summary>
        public bool Hide { get; set; } = true;
    }
}