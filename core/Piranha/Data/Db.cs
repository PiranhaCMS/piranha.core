/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

namespace Piranha.Data
{
    /// <summary>
    /// Database extensions.
    /// </summary>
    public static class Db {
        /// <summary>
        /// The currently available migrations.
        /// </summary>
        public static readonly DbMigration[] Migrations = new DbMigration[] {
            new DbMigration() {
                Name = "InitialCreate",
                Script = "Piranha.Data.Migrations.1.sql"
            },
            new DbMigration() {
                Name = "AddMedia",
                Script = "Piranha.Data.Migrations.2.sql"
            },
            new DbMigration() {
                Name = "AddPageRedirects",
                Script = "Piranha.Data.Migrations.3.sql"
            },
            new DbMigration() {
                Name = "AddMediaType",
                Script = "Piranha.Data.Migrations.4.sql"
            },
            new DbMigration() {
                Name = "UpdateMediaTypes",
                Script = "Piranha.Data.Migrations.5.sql"
            },
            new DbMigration() {
                Name = "RemoveMediaCache",
                Script = "Piranha.Data.Migrations.6.sql"
            }
        };

        /// <summary>
        /// Seeds the database with default data.
        /// </summary>
        /// <param name="api">The current api</param>
        public static void Seed(IApi api) {
            //
            // Default site
            //
            var site = api.Sites.GetDefault();
            if (site == null) {
                site = new Site() {
                    InternalId = "Default",
                    Title = "Default Site",
                    IsDefault = true
                };
                api.Sites.Save(site);
            }

            //
            // Params
            //
            var param = api.Params.GetByKey(Config.CACHE_EXPIRES_PAGES);
            if (param == null)
                api.Params.Save(new Param() {
                    Key = Config.CACHE_EXPIRES_PAGES,
                    Value = 0.ToString()
                });

            param = api.Params.GetByKey(Config.PAGES_HIERARCHICAL_SLUGS);
            if (param == null)
                api.Params.Save(new Param() {
                    Key = Config.PAGES_HIERARCHICAL_SLUGS,
                    Value = true.ToString()
                });

            param = api.Params.GetByKey(Config.MANAGER_EXPANDED_SITEMAP_LEVELS);
            if (param == null)
                api.Params.Save(new Param() {
                    Key = Config.MANAGER_EXPANDED_SITEMAP_LEVELS,
                    Value = "0"
                });            
        }
    }
}
