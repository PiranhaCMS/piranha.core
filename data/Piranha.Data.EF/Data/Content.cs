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

namespace Piranha.Data
{
    [Serializable]
    public sealed class Content : ContentBase<ContentField>, ICategorized, ITranslatable
    {
        /// <summary>
        /// The currently selected language id. This is only used for
        /// mapping and is not stored in the database.
        /// </summary>
        internal Guid? SelectedLanguageId { get; set; }

        /// <summary>
        /// Gets/sets the optional category id.
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Gets/sets the id of the content type.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets/sets the optional primary image id.
        /// </summary>
        public Guid? PrimaryImageId { get; set; }

        /// <summary>
        /// Gets/sets the optional excerpt.
        /// </summary>
        public string Excerpt { get; set; }

        /// <summary>
        /// Gets/sets the optional category.
        /// </summary>
        public Taxonomy Category { get; set; }

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<ContentBlock> Blocks { get; set; } = new List<ContentBlock>();

        /// <summary>
        /// Gets/sets the available tags.
        /// </summary>
        public IList<ContentTaxonomy> Tags { get; set; } = new List<ContentTaxonomy>();

        /// <summary>
        /// Gets/sets the available translations.
        /// </summary>
        public IList<ContentTranslation> Translations { get; set; } = new List<ContentTranslation>();

        /// <summary>
        /// Gets/sets the content type.
        /// </summary>
        public ContentType Type { get; set; }

        /// <summary>
        /// Sets the translation for the specified language.
        /// </summary>
        /// <param name="parentId">The parent id</param>
        /// <param name="languageId">The language id</param>
        /// <param name="model">The model</param>
        public void SetTranslation(Guid parentId, Guid languageId, object model)
        {
            if (model is Models.GenericContent content)
            {
                var translation = Translations.FirstOrDefault(t => t.LanguageId == languageId);

                if (translation == null)
                {
                    translation = new ContentTranslation
                    {
                        ContentId = content.Id,
                        LanguageId = languageId
                    };
                    Translations.Add(translation);
                }
                translation.Title = content.Title;
                translation.Excerpt = content.Excerpt;
                translation.LastModified = DateTime.Now;
            }
        }

        /// <summary>
        /// Gets the translation for the specified language.
        /// </summary>
        /// <param name="languageId">The language id</param>
        /// <returns>The translation</returns>
        public object GetTranslation(Guid languageId)
        {
            return Translations.FirstOrDefault(t => t.LanguageId == languageId);
        }
    }
}
