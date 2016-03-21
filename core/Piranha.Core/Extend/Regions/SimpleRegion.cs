/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend.Regions
{
	/// <summary>
	/// Base class for simple single value regions.
	/// </summary>
	/// <typeparam name="T">The value type</typeparam>
    public abstract class SimpleRegion<T> : Extension
    {
		/// <summary>
		/// Gets/sets the value.
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		/// Gets the value for the client model.
		/// </summary>
		/// <returns>The value</returns>
		public override object GetValue() {
			return Value;
		}
    }
}
