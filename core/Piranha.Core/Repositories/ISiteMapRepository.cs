/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;
using Piranha.Models;

namespace Piranha.Repositories
{
	/// <summary>
	/// The client site map repository interface.
	/// </summary>
	public interface ISiteMapRepository
	{
		/// <summary>
		/// Gets the current sitemap.
		/// </summary>
		/// <returns>The hierarchical sitemap</returns>
		IList<SiteMapModel> Get();
	}
}
