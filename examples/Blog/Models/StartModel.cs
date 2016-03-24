/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Models;

namespace Blog.Models
{
	/// <summary>
	/// The start page model.
	/// </summary>
    public class StartModel : PageModel
    {
		/// <summary>
		/// Gets/sets the startpage blog archive.
		/// </summary>
		public ArchiveModel Archive { get; set; }
    }
}
