/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

namespace Piranha.Extend.Regions
{
	/// <summary>
	/// Region for multiline text fields.
	/// </summary>
	[Extension(Types = ExtensionType.Region)]
    public class TextRegion : SimpleRegion<string> { }
}
