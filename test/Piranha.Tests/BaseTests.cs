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
using System.Data.SqlClient;

namespace Piranha.Tests
{
    /// <summary>
    /// Base class for using the api.
    /// </summary>
    public abstract class BaseTests : IDisposable
    {
        /// <summary>
        /// The default test db options.
        /// </summary>
        protected Action<DbBuilder> options = o => {
            o.Connection = new SqlConnection("data source=(localdb)\\MSSQLLocalDB;initial catalog=piranha.dapper.tests;integrated security=true");
            o.Migrate = true;
        };

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
    }
}
