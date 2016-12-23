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
using Xunit;

namespace Piranha.EF.Tests.Repositories {
    public class CategoryRepositoryUnitTest : RepositoryUnitTestBase<CategoryRepository> {
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
            CreateCategories();
            SetupMockDbSet(mockCategoryDbSet, Categories);
            mockDb.Setup(db => db.Set<Data.Category>()).Returns(mockCategoryDbSet.Object);
        }
        private void CreateCategories() {
            for (int i = 1; i <= NUM_CATEGORIES; i++) {
                Data.Category newCategory = new Data.Category {
                    Id = ConvertIntToGuid(NUM_CATEGORIES - i + 1),
                    Created = DateTime.Now.AddDays(-i),
                    LastModified = DateTime.Now.AddDays(-i),
                    Title = $"Category {i}",
                    Slug = $"Slug{i}",
                    Description = $"Description for category with Id {i}"
                };
                categoriesList.Add(newCategory);
            }

        }

        protected override CategoryRepository SetupRepository() {
            return new CategoryRepository(mockDb.Object);
        }
        #endregion

        #region Unit tests
        #region CategoryRepository.GetBySlug
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetBySlug_ValidSlugReturnsCorrectCategory(int slugNumber) {
            #region Arrange
            string slug = $"Slug{slugNumber}";
            Models.CategoryItem expectedCategory = categoriesList.FirstOrDefault(c => c.Slug == slug);
            #endregion

            #region Act
            Models.CategoryItem result = repository.GetBySlug(slug);
            #endregion

            #region Assert
            Assert_CategoryItemsMatch(expectedCategory, result);
            #endregion
        }

        [Theory]
        [InlineData(0)]
        [InlineData(NUM_CATEGORIES + 1)]
        public void GetBySlug_InvalidSlugReturnsNull(int slugNumber) {
            #region Arrange
            string slug = $"Slug{slugNumber}";
            #endregion

            #region Act
            Models.CategoryItem result = repository.GetBySlug(slug);
            #endregion

            #region Assert
            Assert_CategoryItemsMatch(null, result);
            #endregion
        }
        #endregion

        #region CategoryRepository.GetModelById
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetModelById_ValidIdReturnsCorrectCategory(int idAsInt) {
            #region Arrange
            Guid categoryId = ConvertIntToGuid(idAsInt);
            Data.Category expectedCategory = categoriesList.FirstOrDefault(c => c.Id == categoryId);
            Models.Category expectedModelCategory = new Models.Category {
                Id = expectedCategory.Id,
                Title = expectedCategory.Title,
                Description = expectedCategory.Description,
                Slug = expectedCategory.Slug
            };
            #endregion

            #region Act
            Models.Category result = repository.GetModelById(categoryId);
            #endregion

            #region Assert
            Assert_CategoriesMatch(expectedModelCategory, result);
            #endregion
        }

        [Theory]
        [InlineData(0)]
        [InlineData(NUM_CATEGORIES + 1)]
        public void GetModelById_InvalidIdReturnsNull(int idAsInt) {
            #region Arrange
            Guid categoryId = ConvertIntToGuid(idAsInt);
            #endregion

            #region Act
            Models.Category result = repository.GetModelById(categoryId);
            #endregion

            #region Assert
            Assert_CategoriesMatch(null, result);
            #endregion
        }
        #endregion

        #region CategoryRepository.GetModelBySlug
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetModelBySlug_ValidSlugReturnsCorrectModel(int slugNumber) {
            #region Arrange
            string slug = $"Slug{slugNumber}";
            Data.Category expectedCategory = categoriesList.FirstOrDefault(c => c.Slug == slug);
            Models.Category expectedModelCategory = new Models.Category {
                Id = expectedCategory.Id,
                Title = expectedCategory.Title,
                Slug = expectedCategory.Slug,
                Description = expectedCategory.Description
            };
            #endregion

            #region Act
            Models.Category result = repository.GetModelBySlug(slug);
            #endregion

            #region Assert
            Assert_CategoriesMatch(expectedModelCategory, result);
            #endregion
        }

        [Theory]
        [InlineData(0)]
        [InlineData(NUM_CATEGORIES + 1)]
        public void GetModelBySlug_InvalidSlugReturnsNull(int slugNumber) {
            #region Arrange
            string slug = $"Slug{slugNumber}";
            #endregion

            #region Act
            Models.Category result = repository.GetModelBySlug(slug);
            #endregion

            #region Assert
            Assert_CategoriesMatch(null, result);
            #endregion
        }
        #endregion

        #region CategoryRepository.Get
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Get_ReturnsCorrectListOrderedByTitle(int numItemsToRemove) {
            #region Arrange
            Shuffle(categoriesList);
            categoriesList.RemoveRange(0, numItemsToRemove);

            List<Models.CategoryItem> expectedList = new List<Models.CategoryItem>();
            foreach (var category in categoriesList.OrderBy(c => c.Title)) {
                expectedList.Add(new Models.Category {
                    Id = category.Id,
                    Title = category.Title,
                    Slug = category.Slug,
                    Description = category.Description
                });
            }
            #endregion

            #region Act
            IList<Models.CategoryItem> result = repository.Get();
            #endregion

            #region Assert
            Assert.Equal(expectedList.Count, result.Count);
            for (int i = 0; i < expectedList.Count; i++) {
                Assert_CategoryItemsMatch(expectedList[i], result[i]);
            }
            #endregion
        }
        #endregion

        #region CategoryRepository.GetModels
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetModels_ReturnsCorrectListOrderedByTitle(int numItemsToRemove) {
            #region Arrange
            Shuffle(categoriesList);
            categoriesList.RemoveRange(0, numItemsToRemove);

            List<Models.Category> expectedList = new List<Models.Category>();
            foreach (var category in categoriesList.OrderBy(c => c.Title)) {
                expectedList.Add(new Models.Category {
                    Id = category.Id,
                    Title = category.Title,
                    Slug = category.Slug,
                    Description = category.Description
                });
            }
            #endregion

            #region Act
            IList<Models.Category> result = repository.GetModels();
            #endregion

            #region Assert
            Assert.Equal(expectedList.Count, result.Count);
            #endregion
        }
        #endregion
        #endregion

        #region Private helper methods
        private void Assert_CategoryItemsMatch(Models.CategoryItem expected, Models.CategoryItem actual) {
            if (expected == null) {
                Assert.Null(actual);
            } else {
                Assert.Equal(expected.Description, actual.Description);
                Assert.Equal(expected.Slug, actual.Slug);
                Assert.Equal(expected.Title, actual.Title);
                Assert.Equal(expected.Id, actual.Id);
            }
        }
        private void Assert_CategoriesMatch(Models.Category expected, Models.Category actual) {
            if (expected == null) {
                Assert.Null(actual);
            } else {
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Slug, actual.Slug);
                Assert.Equal(expected.Description, actual.Description);
                Assert.Equal(expected.Title, actual.Title);
            }
        }
        #endregion
    }
}
