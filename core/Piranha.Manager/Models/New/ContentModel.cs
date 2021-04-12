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
using Piranha.Extend.Fields;
using Piranha.Models;

namespace Piranha.Manager.Models
{
    public sealed class ContentModel
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the optional language id.
        /// </summary>
        public Guid? LanguageId { get; set; }

        /// <summary>
        /// Gets/sets the optional language title.
        /// </summary>
        public string LanguageTitle { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the content type id.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets/sets the content type title.
        /// </summary>
        public string TypeTitle { get; set; }

        /// <summary>
        /// Gets/sets the content type group id.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Gets/sets the content type group title.
        /// </summary>
        public string GroupTitle { get; set; }

        /// <summary>
        /// Gets/sets the available features.
        /// </summary>
        public ContentFeatures Features { get; set; } = new ContentFeatures();

        /// <summary>
        /// Gets/sets the hierarchical position if placed
        /// in a sitemap.
        /// </summary>
        public ContentPosition Position { get; set; }

        /// <summary>
        /// Gets/sets the meta information if the content
        /// is routable.
        /// </summary>
        public ContentMeta Meta { get; set; }

        /// <summary>
        /// Gets/sets how comments should be handled.
        /// </summary>
        public ContentComments Comments { get; set; }

        /// <summary>
        /// Gets/sets the permission information.
        /// </summary>
        public ContentPermissions Permissions { get; set; }

        /// <summary>
        /// Gets/sets the route information.
        /// </summary>
        public ContentRoutes Routes { get; set; }

        /// <summary>
        /// Gets/sets the taxonomy information.
        /// </summary>
        public ContentTaxonomies Taxonomies { get; set; }

        /// <summary>
        /// Gets/sets the mandatory title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the alternate title.
        /// </summary>
        public string AltTitle { get; set; }

        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the optional primary image.
        /// </summary>
        public ImageField PrimaryImage { get; set; }

        /// <summary>
        /// Gets/sets the optional excerpt.
        /// </summary>
        public string Excerpt { get; set; }

        /// <summary>
        /// Gets/sets the content status.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets/sets the published date.
        /// </summary>
        public string Published { get; set; }

        /// <summary>
        /// Gets/sets the published time.
        /// </summary>
        public string PublishedTime { get; set; }

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<Content.BlockModel> Blocks { get; set; } = new List<Content.BlockModel>();

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<Content.RegionModel> Regions { get; set; } = new List<Content.RegionModel>();

        /// <summary>
        /// Gets/sets the available custom editors.
        /// </summary>
        public IList<Content.EditorModel> Editors { get; set; } = new List<Content.EditorModel>();

        /// <summary>
        /// Gets/sets the available languages.
        /// </summary>
        public IEnumerable<Language> Languages { get; set; } = new List<Language>();
    }
}