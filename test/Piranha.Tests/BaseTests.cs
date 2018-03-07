﻿/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using Microsoft.EntityFrameworkCore;
using Piranha.Local;

namespace Piranha.Tests
{
    /// <summary>
    /// Base class for using the api.
    /// </summary>
    public abstract class BaseTests : IDisposable
    {
        protected IStorage storage = new FileStorage("uploads/", "~/uploads/");

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseTests() {
            Init();
        }

        /// <summary>
        /// Disposes the test class.
        /// </summary>
        public void Dispose() {
            Cleanup();
        }

        /// <summary>
        /// Sets up & initializes the tests.
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// Cleans up any possible data and resources
        /// created by the test.
        /// </summary>
        protected abstract void Cleanup();

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
