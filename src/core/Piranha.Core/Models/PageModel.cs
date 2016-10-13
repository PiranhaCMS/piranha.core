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
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Piranha.Models
{
    public class PageModel : PageBase
    {
		#region Properties
		/// <summary>
		/// Gets/sets the regions.
		/// </summary>
		public dynamic Regions { get; set; }

		/// <summary>
		/// Gets if this is the startpage of the site.
		/// </summary>
		public bool IsStartPage {
			get { return !ParentId.HasValue && SortOrder == 0; }
		}
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PageModel() {
			Regions = new ExpandoObject();
		}

		/// <summary>
		/// Creates a new page model using the given page type id.
		/// </summary>
		/// <param name="typeId">The unique page type id</param>
		/// <returns>The new model</returns>
		public static PageModel Create(string typeId) {
			var pageType = App.PageTypes
				.SingleOrDefault(t => t.Id == typeId);
			
			if (pageType != null) {
				var model = new PageModel() {
					PageTypeId = typeId
				};

				foreach (var region in pageType.Regions) {
					object value = null;

					if (region.Collection) {
						var reg = CreateRegion(region);

						if (reg != null) {
							value = Activator.CreateInstance(typeof(List<>).MakeGenericType(reg.GetType()));
							((IList)value).Add(reg);
						}
					} else {
						value = CreateRegion(region);
					}

					if (value != null)
						((IDictionary<string, object>)model.Regions).Add(region.Id, value);
				}
				return model;
			}
			return null;
		}

		/// <summary>
		/// Creates a new region.
		/// </summary>
		/// <param name="typeId">The page type id</param>
		/// <param name="regionId">The region id</param>
		/// <returns>The new region value</returns>
		public static object CreateRegion(string typeId, string regionId) {
			var pageType = App.PageTypes
				.SingleOrDefault(t => t.Id == typeId);

			if (pageType != null) {
				var region = pageType.Regions.SingleOrDefault(r => r.Id == regionId);

				if (region != null)
					return CreateRegion(region);
			}
			return null;
		}

		#region Private methods
		/// <summary>
		/// Creates a region value from the specified json structure.
		/// </summary>
		/// <param name="region">The region type</param>
		/// <returns>The created value</returns>
		private static object CreateRegion(Models.PageTypeRegion region) {
			if (region.Fields.Count == 1) {
				var type = App.Fields.GetByShorthand(region.Fields[0].Type);
				if (type == null)
					type = App.Fields.GetByType(region.Fields[0].Type);

				if (type != null)
					return Activator.CreateInstance(type.Type);
			} else {
				var reg = new ExpandoObject();

				foreach (var field in region.Fields) {
					var type = App.Fields.GetByShorthand(field.Type);
					if (type == null)
						type = App.Fields.GetByType(field.Type);

					if (type != null)
						((IDictionary<string, object>)reg).Add(field.Id, Activator.CreateInstance(type.Type));
				}
				return reg;
			}
			return null;
		}
		#endregion
	}
}
