/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager;

/// <summary>
/// The available manager permissions.
/// </summary>
public static class Permission
{
    public const string Admin = "PiranhaAdmin";
    public const string Aliases = "PiranhaAliases";
    public const string AliasesDelete = "PiranhaAliasesDelete";
    public const string AliasesEdit = "PiranhaAliasesEdit";
    public const string Comments = "PiranhaComments";
    public const string CommentsApprove = "PiranhaCommentsApprove";
    public const string CommentsDelete = "PiranhaCommentsDelete";
    public const string Config = "PiranhaConfig";
    public const string ConfigEdit = "PiranhaConfigEdit";
    public const string Content = "PiranhaContent";
    public const string ContentAdd = "PiranhaContentAdd";
    public const string ContentDelete = "PiranhaContentDelete";
    public const string ContentEdit = "PiranhaContentEdit";
    public const string ContentSave = "PiranhaContentSave";
    public const string Language = "Language";
    public const string LanguageAdd = "LanguageAdd";
    public const string LanguageEdit = "LanguageEdit";
    public const string LanguageDelete = "LanguageDelete";
    public const string Media = "PiranhaMedia";
    public const string MediaAdd = "PiranhaMediaAdd";
    public const string MediaDelete = "PiranhaMediaDelete";
    public const string MediaEdit = "PiranhaMediaEdit";
    public const string MediaAddFolder = "PiranhaMediaAddFolder";
    public const string MediaDeleteFolder = "PiranhaMediaDeleteFolder";
    public const string Modules = "PiranhaModules";
    public const string Pages = "PiranhaPages";
    public const string PagesAdd = "PiranhaPagesAdd";
    public const string PagesDelete = "PiranhaPagesDelete";
    public const string PagesEdit = "PiranhaPagesEdit";
    public const string PagesPublish = "PiranhaPagesPublish";
    public const string PagesSave = "PiranhaPagesSave";
    public const string Posts = "PiranhaPosts";
    public const string PostsAdd = "PiranhaPostsAdd";
    public const string PostsDelete = "PiranhaPostsDelete";
    public const string PostsEdit = "PiranhaPostsEdit";
    public const string PostsPublish = "PiranhaPostsPublish";
    public const string PostsSave = "PiranhaPostsSave";
    public const string Sites = "PiranhaSites";
    public const string SitesAdd = "PiranhaSitesAdd";
    public const string SitesDelete = "PiranhaSitesDelete";
    public const string SitesEdit = "PiranhaSitesEdit";
    public const string SitesSave = "PiranhaSitesSave";

    public static readonly PermissionsStructure PermissionsStructure =
        // Admin Permission
        new(Admin, new PermissionsStructure[]
        {
            new(Aliases, new PermissionsStructure[] {new(AliasesDelete), new(AliasesEdit)}),
            new(Comments, new PermissionsStructure[] {new(CommentsApprove), new(CommentsDelete) }),
            new(Config, new PermissionsStructure[] {new(ConfigEdit) }),
            new(Content, new PermissionsStructure[] {new(ContentAdd), new(ContentEdit) , new(ContentSave) , new(ContentDelete) }),
            new(Language, new PermissionsStructure[] {new(LanguageAdd), new(LanguageEdit) , new(LanguageDelete) }),
            new(Media, new PermissionsStructure[] {new(MediaAdd), new(MediaDelete) , new(MediaEdit) , new(MediaAddFolder), new(MediaDeleteFolder) }),
            new(Modules),
            new(Pages, new PermissionsStructure[] {new(PagesAdd), new(PagesDelete) , new(PagesEdit) , new(PagesPublish), new(PagesSave) }),
            new(Posts, new PermissionsStructure[] {new(PostsAdd), new(PostsDelete) , new(PostsEdit) , new(PostsPublish), new(PostsSave) }),
            new(Sites, new PermissionsStructure[] {new(SitesAdd), new(SitesDelete) , new(SitesEdit) , new(SitesSave) }),
        });

    public static string[] All() {
        return new [] {
            Admin,
            Aliases,
            AliasesDelete,
            AliasesEdit,
            Comments,
            CommentsApprove,
            CommentsDelete,
            Config,
            ConfigEdit,
            Content,
            ContentAdd,
            ContentDelete,
            ContentEdit,
            ContentSave,
            Language,
            LanguageAdd,
            LanguageDelete,
            LanguageEdit,
            Media,
            MediaAdd,
            MediaDelete,
            MediaEdit,
            MediaAddFolder,
            MediaDeleteFolder,
            Modules,
            Pages,
            PagesAdd,
            PagesDelete,
            PagesEdit,
            PagesPublish,
            PagesSave,
            Posts,
            PostsAdd,
            PostsDelete,
            PostsEdit,
            PostsPublish,
            PostsSave,
            Sites,
            SitesAdd,
            SitesDelete,
            SitesEdit,
            SitesSave
        };
    }
}
