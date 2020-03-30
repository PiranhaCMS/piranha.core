/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Linq;
using Piranha.Manager.Models;

namespace Piranha.Manager.Services
{
    public class ModuleService
    {
        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        public ModuleListModel GetList()
        {
            return new ModuleListModel
            {
                Items = App.Modules
                    .OrderBy(m => m.Instance.Author)
                    .ThenBy(m => m.Instance.Name)
                    .Select(m => new ModuleListModel.ListItem
                    {
                        Author = m.Instance.Author,
                        Name = m.Instance.Name,
                        Version = m.Instance.Version,
                        Description = m.Instance.Description,
                        PackageUrl = m.Instance.PackageUrl,
                        IconUrl = m.Instance.IconUrl
                    }).ToList()
            };
        }
    }
}