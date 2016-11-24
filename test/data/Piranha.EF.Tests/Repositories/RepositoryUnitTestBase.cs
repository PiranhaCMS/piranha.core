/*
 * Copyright (c) 2016 Billy Wolfington
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using Moq;

namespace Piranha.EF.Tests.Repositories
 {
     public abstract class RepositoryUnitTestBase<TRepository>
        where TRepository : class
    {
        #region Properties
        /// <summary>
        /// The databse api to inject into <see cref="repository" />
        /// </summary>
        protected readonly Mock<IDb> mockDb;

        /// <summary>
        /// The repository being tested
        /// </summary>
        protected readonly TRepository repository;
        #endregion

        #region Test initialization
        /// <summary>
        /// Default constructor/test initialization
        /// </summary>
        public RepositoryUnitTestBase() {
            mockDb = new Mock<IDb>();
            repository = SetupRepository();
            SetupMockDbData();
        }

        /// <summary>
        /// Constructs the repository to be assigned to <see cref="repository" />
        /// </summary>
        /// <returns>
        /// The repository
        /// </returns>
        protected abstract TRepository SetupRepository();

        /// <summary>
        /// Setup <see cref="mockDb" /> data for use in all tests
        /// </summary>
        protected abstract void SetupMockDbData();
        #endregion

        protected Guid ConvertIntToGuid(int value) {
            byte[] valueAsBytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(valueAsBytes, 0);
            return new Guid(valueAsBytes);
        }
    }
 }