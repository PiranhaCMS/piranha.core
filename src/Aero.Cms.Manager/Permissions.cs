

namespace Aero.Cms.Manager;

/// <summary>
/// The available manager permissions.
/// </summary>
public static class Permission
{
    public const string Admin = "AeroAdmin";
    public const string Aliases = "AeroAliases";
    public const string AliasesDelete = "AeroAliasesDelete";
    public const string AliasesEdit = "AeroAliasesEdit";
    public const string Comments = "AeroComments";
    public const string CommentsApprove = "AeroCommentsApprove";
    public const string CommentsDelete = "AeroCommentsDelete";
    public const string Config = "AeroConfig";
    public const string ConfigEdit = "AeroConfigEdit";
    public const string Content = "AeroContent";
    public const string ContentAdd = "AeroContentAdd";
    public const string ContentDelete = "AeroContentDelete";
    public const string ContentEdit = "AeroContentEdit";
    public const string ContentSave = "AeroContentSave";
    public const string Language = "Language";
    public const string LanguageAdd = "LanguageAdd";
    public const string LanguageEdit = "LanguageEdit";
    public const string LanguageDelete = "LanguageDelete";
    public const string Media = "AeroMedia";
    public const string MediaAdd = "AeroMediaAdd";
    public const string MediaDelete = "AeroMediaDelete";
    public const string MediaEdit = "AeroMediaEdit";
    public const string MediaAddFolder = "AeroMediaAddFolder";
    public const string MediaDeleteFolder = "AeroMediaDeleteFolder";
    public const string Modules = "AeroModules";
    public const string Pages = "AeroPages";
    public const string PagesAdd = "AeroPagesAdd";
    public const string PagesDelete = "AeroPagesDelete";
    public const string PagesEdit = "AeroPagesEdit";
    public const string PagesPublish = "AeroPagesPublish";
    public const string PagesSave = "AeroPagesSave";
    public const string Posts = "AeroPosts";
    public const string PostsAdd = "AeroPostsAdd";
    public const string PostsDelete = "AeroPostsDelete";
    public const string PostsEdit = "AeroPostsEdit";
    public const string PostsPublish = "AeroPostsPublish";
    public const string PostsSave = "AeroPostsSave";
    public const string Sites = "AeroSites";
    public const string SitesAdd = "AeroSitesAdd";
    public const string SitesDelete = "AeroSitesDelete";
    public const string SitesEdit = "AeroSitesEdit";
    public const string SitesSave = "AeroSitesSave";

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
