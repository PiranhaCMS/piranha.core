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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Piranha.EF.Tests.Repositories
{
    public class CategoryRepositoryUnitTest : RepositoryUnitTestBase<CategoryRepository>
    {
        #region Properties
        /// <summary>
        /// Numver of categories to create for testing
        /// </summary>
        private const int NUM_CATEGORIES = 5;

        /// <summary>
        /// Queryable categories behind <see cref="mockCategoryDbSet" />
        /// </summary>
        private IQueryable<Data.Category> Categories {
            get {
                return categoriesList.AsQueryable();
            }
        }

        private readonly List<Data.Category> categoriesList = new List<Data.Category>();
        /// <summary>
        /// Mock <see cref="DbSet" /> behind <see cref="repository" />
        /// </summary>
        private readonly Mock<DbSet<Data.Category>> mockCategoryDbSet = new Mock<DbSet<Data.Category>>();
        #endregion

        #region Test initialization
        protected override void SetupMockDbData() {
            SetupMockDbSet(mockCategoryDbSet, Categories);
        }
        private void CreateCategories() {
            for (int i = 1; i <= NUM_CATEGORIES; i++) {
                Data.Category newCategory = new Data.Category {
                    Id = ConvertIntToGuid(NUM_CATEGORIES - i + 1),
                    Created = DateTime.Now.AddDays(-i),
                    LastModified = DateTime.Now.AddDays(-i)
                };
            }
           
        }

        protected override CategoryRepository SetupRepository() {
            return new CategoryRepository(mockDb.Object);
        }
        #endregion
    }
}
