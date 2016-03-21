/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Extend
{
	/// <summary>
	/// The different extension types available.
	/// </summary>
	[Flags]
    public enum ExtensionType
    {
		Region = 1,
		Property = 2,
		Author = 4,
		Category = 8,
		Media = 16,
		MediaFolder = 32,
		Page = 64,
		PageType = 128,
		Param = 256,
		Post = 512,
		PostType = 1024,
		Tag = 2048
    }
}
