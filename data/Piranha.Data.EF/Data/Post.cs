/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Data;

[Serializable]
public sealed class Post : RoutedContentBase<PostField>, ITranslatable
{
    /// <summary>
    /// The currently selected language id. This is only used for
    /// mapping and is not stored in the database.
    /// </summary>
    internal Guid? SelectedLanguageId { get; set; }

    /// <summary>
    /// Gets/sets the post type id.
    /// </summary>
    public string PostTypeId { get; set; }

    /// <summary>
    /// Gets/sets the id of the blog page this
    /// post belongs to.
    /// </summary>
    public Guid BlogId { get; set; }

    /// <summary>
    /// Gets/sets the category id.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Gets/sets the optional primary image id.
    /// </summary>
    public Guid? PrimaryImageId { get; set; }

    /// <summary>
    /// Gets/sets the optional excerpt.
    /// </summary>
    public string Excerpt { get; set; }

    /// <summary>
    /// Gets/sets the optional redirect.
    /// </summary>
    /// <returns></returns>
    public string RedirectUrl { get; set; }

    /// <summary>
    /// Gets/sets the redirect type.
    /// </summary>
    /// <returns></returns>
    public Models.RedirectType RedirectType { get; set; }

    /// <summary>
    /// Gets/sets if comments should be enabled.
    /// </summary>
    /// <value></value>
    public bool EnableComments { get; set; }

    /// <summary>
    /// Gets/sets after how many days after publish date comments
    /// should be closed. A value of 0 means never.
    /// </summary>
    public int CloseCommentsAfterDays { get; set; }

    /// <summary>
    /// Gets/sets the associated post type.
    /// </summary>
    public PostType PostType { get; set; }

    /// <summary>
    /// Gets/sets the blog page this category belongs to.
    /// </summary>
    public Page Blog { get; set; }

    /// <summary>
    /// Gets/sets the post category.
    /// </summary>
    public Category Category { get; set; }

    /// <summary>
    /// Gets/sets the available tags.
    /// </summary>
    public IList<PostTag> Tags { get; set; } = new List<PostTag>();

    /// <summary>
    /// Gets/sets the available post blocks.
    /// </summary>
    public IList<PostBlock> Blocks { get; set; } = new List<PostBlock>();

    /// <summary>
    /// Gets/sets the available permissions.
    /// </summary>
    public IList<PostPermission> Permissions { get; set; } = new List<PostPermission>();

    /// <summary>
    /// Gets/sets the available translations.
    /// </summary>
    public IList<PostTranslation> Translations { get; set; } = new List<PostTranslation>();

    /// <summary>
    /// Sets the translation for the specified language.
    /// </summary>
    public void SetTranslation(Guid parentId, Guid languageId, object model)
    {
        if (model is Models.PostBase post)
        {
            var translation = Translations.FirstOrDefault(t => t.LanguageId == languageId);

            if (translation == null)
            {
                translation = new PostTranslation
                {
                    PostId = post.Id,
                    LanguageId = languageId
                };
                Translations.Add(translation);
            }
            translation.Title = post.Title;
            translation.Slug = post.Slug;
            translation.Excerpt = post.Excerpt;
            translation.MetaTitle = post.MetaTitle;
            translation.MetaKeywords = post.MetaKeywords;
            translation.MetaDescription = post.MetaDescription;
            translation.OgTitle = post.OgTitle;
            translation.OgDescription = post.OgDescription;
            translation.LastModified = DateTime.Now;
        }
    }

    /// <summary>
    /// Gets the translation for the specified language.
    /// </summary>
    public object GetTranslation(Guid languageId)
    {
        return Translations.FirstOrDefault(t => t.LanguageId == languageId);
    }
}
