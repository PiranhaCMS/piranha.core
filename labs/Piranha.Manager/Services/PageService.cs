/*
 * Copyright (c) 2019 Håkan Edling
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
using Piranha.Models;
using Piranha.Manager.Models;

namespace Piranha.Manager.Services
{
    public class PageService
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PageService(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        public PageListModel GetList()
        {
            return new PageListModel {
                Items = new List<PageListModel.ListItem>
                {
                    new PageListModel.ListItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Cras Lorem Amet",
                        TypeName = "Standard page",
                        Published = DateTime.Now.ToString("yyyy-MM-dd")
                    },
                    new PageListModel.ListItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Nullam Sit",
                        TypeName = "Standard page",
                        Published = DateTime.Now.ToString("yyyy-MM-dd"),
                        Items = new List<PageListModel.ListItem>
                        {
                            new PageListModel.ListItem
                            {
                                Id = Guid.NewGuid(),
                                Title = "Egestas Cras",
                                TypeName = "Standard page",
                                Published = DateTime.Now.ToString("yyyy-MM-dd")
                            },
                            new PageListModel.ListItem
                            {
                                Id = Guid.NewGuid(),
                                Title = "Fusce Ornare",
                                TypeName = "Standard page",
                                Published = DateTime.Now.ToString("yyyy-MM-dd")
                            }
                        }
                    },
                    new PageListModel.ListItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Bibendum Mattis Vehicula",
                        TypeName = "Standard page",
                        Published = DateTime.Now.ToString("yyyy-MM-dd")
                    }
                }
            };
        }
    }
}