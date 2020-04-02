/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Piranha.Data.EF.SQLite;
using Piranha.ImageSharp;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests
{
    /// <summary>
    /// Base class for using the api.
    /// </summary>
    public abstract class BaseTestsAsync : IAsyncLifetime
    {
        protected IStorage _storage = new Local.FileStorage("uploads/", "~/uploads/");
        protected IImageProcessor _processor = new ImageSharpProcessor();
        protected IServiceProvider _services = new ServiceCollection()
            .AddPiranhaFileStorage()
            .BuildServiceProvider();
        protected ICache _cache;

        public abstract Task InitializeAsync();
        public abstract Task DisposeAsync();

        /// <summary>
        /// Gets the test context.
        /// </summary>
        protected IDb GetDb() {
            var builder = new DbContextOptionsBuilder<SQLiteDb>();

            builder.UseSqlite("Filename=./piranha.tests.db");

            return new SQLiteDb(builder.Options);
        }

        /// <summary>
        /// Creates a new api.
        /// </summary>
        protected virtual IApi CreateApi()
        {
            var factory = new ContentFactory(_services);
            var serviceFactory = new ContentServiceFactory(factory);

            var db = GetDb();

            return new Api(
                factory,
                new AliasRepository(db),
                new ArchiveRepository(db),
                new Piranha.Repositories.MediaRepository(db),
                new PageRepository(db, serviceFactory),
                new PageTypeRepository(db),
                new ParamRepository(db),
                new PostRepository(db, serviceFactory),
                new PostTypeRepository(db),
                new SiteRepository(db, serviceFactory),
                new SiteTypeRepository(db),
                cache: _cache,
                storage: _storage,
                processor: _processor
            );
        }
    }
}
