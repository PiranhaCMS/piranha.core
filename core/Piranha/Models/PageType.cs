/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Models
{
    public sealed class PageType : ContentType
    {
        /// <summary>
        /// Gets/sets the optional route.
        /// </summary>
        /*
        [JsonIgnore]
        public string Route { 
            get {
                return Routes.FirstOrDefault(r => r.Title == "Default")?.Route;
            }
            set {
                var route = Routes.FirstOrDefault(r => r.Title == "Default");

                if (route != null)
                    route.Route = value;
                else Routes.Add(new PageTypeRoute() {
                    Title = "Default",
                    Route = value
                });
            }
        }
        */

        /// <summary>
        /// Gets/sets the optional routes.
        /// </summary>
        public IList<PageTypeRoute> Routes { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageType() : base() { 
            Routes = new List<PageTypeRoute>();
        }

        /// <summary>
        /// Validates that the page type is correctly defined.
        /// </summary>
        public void Ensure() {
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
