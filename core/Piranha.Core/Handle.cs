/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

using System;

namespace Piranha
{
	/// <summary>
	/// The startup options for the Piranha middleware.
	/// </summary>
	[Flags]
    public enum Handle
    {
		All = 1,
		Archives = 2,
		Media = 4,
		Pages = 8,
		Posts = 16,
		StartPage = 32
    }
}
