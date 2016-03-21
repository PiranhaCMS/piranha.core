/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Extend.Regions;

namespace Piranha.Extend.Serializers
{
	/// <summary>
	/// Serializer for HTML regions.
	/// </summary>
    public class HtmlSerializer : ISerializer
    {
		/// <summary>
		/// Serializes the object into a string.
		/// </summary>
		/// <param name="obj">The object</param>
		/// <returns>The serialized object</returns>
		public string Serialize(IExtension obj) {
			return ((HtmlRegion)obj).Value;
		}

		/// <summary>
		/// Deserializes the given string into an object.
		/// </summary>
		/// <param name="str">The serialized data</param>
		/// <returns>The object</returns>
		public IExtension Deserialize(string str) {
			return new HtmlRegion() {
				Value = str
			};
		}
	}
}
