/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNet.Mvc.Rendering;

namespace Piranha.Extend.Regions
{
	/// <summary>
	/// Region for html fields.
	/// </summary>
	[Extension(Types = ExtensionType.Region)]
    public class HtmlRegion : SimpleRegion<string>
    {
		/// <summary>
		/// Gets the value for the client model.
		/// </summary>
		/// <returns>The value</returns>
		public override object GetValue() {
			return new HtmlString(Value);
		}
	}
}
