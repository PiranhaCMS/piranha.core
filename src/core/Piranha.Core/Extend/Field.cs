/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend
{
    public abstract class Field : IField
    {
        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual void Init(IApi api) { }

        /// <summary>
        /// Initializes the field for manager use. This
        /// method can be used for loading additional meta
        /// data needed.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual void InitManager(IApi api) { }

        /// <summary>
        /// Gets the client value.
        /// </summary>
        /// <returns></returns>
        public virtual object GetValue() {
            return this;
        }
    }
}
