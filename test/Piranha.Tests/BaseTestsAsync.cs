/*
 * Copyright (c) 2017 Håkan Edling
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

namespace Piranha.Tests
{
    /// <summary>
    /// Base class for using the api.
    /// </summary>
    public abstract class BaseTestsAsync : IAsyncLifetime
    {
        protected IStorage storage = new Local.FileStorage("uploads/", "~/uploads/");
        protected IServiceProvider services = new ServiceCollection()
            .BuildServiceProvider();

        public abstract Task InitializeAsync();
        public abstract Task DisposeAsync();

        /// <summary>
        /// Gets the test context.
        /// </summary>
        protected IDb GetDb() {
            var builder = new DbContextOptionsBuilder<Db>();

            builder.UseSqlite("Filename=./piranha.tests.db");

            return new Db(builder.Options);
        }
    }
}
