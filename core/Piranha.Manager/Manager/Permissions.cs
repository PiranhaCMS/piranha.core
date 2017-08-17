/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

namespace Piranha.Manager
{
    /// <summary>
    /// The available manager permissions.
    /// </summary>
    public static class Permission 
    {
        public const string Admin = "PiranhaAdmin";
        public const string Config = "PiranhaConfig";
        public const string ConfigEdit = "PiranhaConfigEdit";
        public const string Media = "PiranhaMedia";
        public const string MediaAdd = "PiranhaMediaAdd";
        public const string MediaDelete = "PiranhaMediaDelete";
        public const string MediaEdit = "PiranhaMediaEdit";
        public const string MediaAddFolder = "PiranhaMediaAddFolder";
        public const string MediaDeleteFolder = "PiranhaMediaDeleteFolder";
        public const string Pages = "PiranhaPages";
        public const string PagesAdd = "PiranhaPagesAdd";
        public const string PagesDelete = "PiranhaPagesDelete";
        public const string PagesEdit = "PiranhaPagesEdit";
        public const string PagesPublish = "PiranhaPagesPublish";
        public const string PagesSave = "PiranhaPagesSave";
        public const string Sites = "PiranhaSites";
        public const string SitesAdd = "PiranhaSitesAdd";
        public const string SitesDelete = "PiranhaSitesDelete";
        public const string SitesEdit = "PiranhaSitesEdit";

        public const string SitesSave = "PiranhaSitesSave";

        public static string[] All() {
            return new string[] {
                Admin,
                Config,
                ConfigEdit,
                Media,
                MediaAdd,
                MediaDelete,
                MediaEdit,
                MediaAddFolder,
                MediaDeleteFolder,
                Pages,
                PagesAdd,
                PagesDelete,
                PagesEdit,
                PagesPublish,
                PagesSave,
                Sites,
                SitesAdd,
                SitesDelete,
                SitesEdit,
                SitesSave
            };
        }
    }
}