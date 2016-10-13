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
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Models
{
    public sealed class PageType
    {
		#region Properties
		/// <summary>
		/// Gets/sets the unique id.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets/sets the optional title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets/sets the optional route.
		/// </summary>
		public string Route { get; set; }

		/// <summary>
		/// Gets/sets the available regions.
		/// </summary>
		public IList<PageTypeRegion> Regions { get; set; }
		#endregion

		/// <summary>
		/// Default internal constructor.
		/// </summary>
		internal PageType() {
			Regions = new List<PageTypeRegion>();
		}

		/// <summary>
		/// Validates that the page type is correctly defined.
		/// </summary>
		internal void Ensure() {
			if (Regions.Select(r => r.Id).Distinct().Count() != Regions.Count)
				throw new Exception($"Region Id not unique for page type {Id}");

			foreach (var region in Regions) {
				region.Title = region.Title ?? region.Id;

				if (region.Fields.Select(f => f.Id).Distinct().Count() != region.Fields.Count)
					throw new Exception($"Field Id not unique for page type {Id}");
				foreach (var field in region.Fields) {
					field.Id = field.Id ?? "Default";
					field.Title = field.Title ?? field.Id;
				}
			}

		}
	}
}
