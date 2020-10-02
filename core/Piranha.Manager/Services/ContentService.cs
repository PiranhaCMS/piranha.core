/*
 * Copyright (c) .NET Foundation and Contributors
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
using System.Threading.Tasks;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Services;

namespace Piranha.Manager.Services
{
    public class ContentService
    {
        private readonly IApi _api;
        private readonly IContentFactory _factory;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="factory">The content factory</param>
        public ContentService(IApi api, IContentFactory factory)
        {
            _api = api;
            _factory = factory;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <param name="contentGroup">Name of the content group</param>
        /// <returns>Gets the list model.</returns>
        public async Task<ContentListModel> GetListAsync(string contentGroup)
        {
            var group = _api.ContentGroups.GetByIdAsync(contentGroup);
            var items = _api.Content.GetAllAsync<ContentInfo>(contentGroup);
            var types = _api.ContentTypes.GetByGroupAsync(contentGroup);

            await Task.WhenAll(group, items, types);

            return new ContentListModel
            {
                Group = group.Result,
                Items = items.Result,
                Types = types.Result
                    .Select(t =>
                        new ContentTypeModel
                        {
                            Id = t.Id,
                            Title = t.Title,
                            AddUrl = $"manager/content/add/{t.Id}"
                        })
                    .ToList()
            };
        }
    }
}
