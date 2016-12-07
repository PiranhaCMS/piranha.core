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
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using Piranha.EF.Repositories;
using Xunit;

namespace Piranha.EF.Tests.Repositories
{
    public class PageTypeRepositoryUnitTest : RepositoryUnitTestBase<PageTypeRepository>
    {
        #region Properties
        /// <summary>
        /// Number of page types to create for testing
        /// </summary>
        private const int NUM_PAGE_TYPES = 5;

        /// <summary>
        /// Queryable page types behind <see cref="mockPageTypeSet" />
        /// </summary>
        private IQueryable<Data.PageType> PageTypes {
            get {
                return pageTypesList.AsQueryable();
            }
        }

        /// <summary>
        /// Mock page type data
        /// </summary>
        private List<Data.PageType> pageTypesList = new List<Data.PageType>();
        /// <summary>
        /// Mock <see cref="DbSet" /> behind <see cref="repository.PageTypes" />
        /// </summary>
        private Mock<DbSet<Data.PageType>> mockPageTypeSet = new Mock<DbSet<Data.PageType>>();
        #endregion

        #region Test initialization
        protected override void SetupMockDbData() {
            CreateMockPageTypes();
            SetupMockDbSet(mockPageTypeSet, PageTypes);
            mockDb.Setup(db => db.PageTypes).Returns(mockPageTypeSet.Object);
        }
        /// <summary>
        /// Populates <see cref="pageTypesList" /> with fake data
        /// </summary>
        private void CreateMockPageTypes() {
            for (int i = 0; i < NUM_PAGE_TYPES; i++) {
                string pageTypeId = $"PageType{i+1}";
                pageTypesList.Add(new Data.PageType {
                    Id = pageTypeId,
                    Body = $"{{\"View\":null,\"Id\":\"{pageTypeId}\",\"Title\":\"Html block\",\"Regions\":[{{\"Id\":\"Content\",\"Title\":\"Main Content\",\"Collection\":false,\"Max\":0,\"Min\":0,\"Fields\":[{{\"Id\":\"Default\",\"Title\":\"Default\",\"Type\":\"Html\"}}]}}]}}",
                    Created = DateTime.Now.AddDays(-(i + 1)),
                    LastModified = DateTime.Now.AddDays(-(i + 1)),
                    Pages = new List<Data.Page>(),
                });
            }
        }

        protected override PageTypeRepository SetupRepository() {
            return new PageTypeRepository(mockDb.Object);
        }
        #endregion

        #region Unit tests
        #region PageTypeRepository.GetById
        /// <summary>
        /// Tests that <see cref="PageTypeRepository.GetById" /> returns null when its
        /// data source is empty
        /// </summary>
        /// <param name="pageTypeIdAsInt">The integer Id value to use as part of the page type Id</param>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(NUM_PAGE_TYPES + 1)]
        public void GetById_EmptySetGivesNull(int pageTypeIdAsInt) {
            #region Arrange
            pageTypesList.Clear();
            string pageTypeId = $"PageType{pageTypeIdAsInt}";
            #endregion
        
            #region Act
            Extend.PageType result = repository.GetById(pageTypeId);
            #endregion
        
            #region Assert
            Assert.Null(result);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageTypeRepository.GetById" /> with an invalid Id 
        /// returns null
        /// </summary>
        /// <param name="pageTypeIdAsInt">The integer Id value to use as part of the page type Id</param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should NOT be in the range
        /// [1, <see cref="NUM_PAGE_TYPES" />]
        /// </remarks>
        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGE_TYPES + 1)]
        public void GetById_InvalidIdGivesNull(int pageTypeIdAsInt) {
            #region Arrange
            string pageTypeId = $"PageType{pageTypeIdAsInt}";
            #endregion
        
            #region Act
            Extend.PageType result = repository.GetById(pageTypeId);
            #endregion
        
            #region Assert
            Assert.Null(result);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageTypeRepository.GetById" /> with an empty string or null
        /// returns null
        /// </summary>
        /// <param name="pageTypeId">The page type Id</param>
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetById_EmptyIdGivesNull(string pageTypeId) {
            #region Arrange
            #endregion
        
            #region Act
            Extend.PageType result = repository.GetById(pageTypeId);
            #endregion
        
            #region Assert
            Assert.Null(result);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageTypeRepository.GetById" /> with an valid Id 
        /// returns the proper page type
        /// </summary>
        /// <param name="pageTypeIdAsInt">The integer Id value to use as part of the page type Id</param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should be in the range
        /// [1, <see cref="NUM_PAGE_TYPES" />]
        /// </remarks>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetById_ValidIdGivesProperPageType(int pageTypeIdAsInt) {
            #region Arrange
            string pageTypeId = $"PageType{pageTypeIdAsInt}";
            Data.PageType pageType = pageTypesList.FirstOrDefault(t => t.Id == pageTypeId);
            Extend.PageType expectedPageType = JsonConvert.DeserializeObject<Extend.PageType>(pageType.Body);
            #endregion
        
            #region Act
            Extend.PageType result = repository.GetById(pageTypeId);
            #endregion
        
            #region Assert
            AssertPageTypesMatch(expectedPageType, result);
            #endregion
        }
        #endregion

        #region PageTypeRepository.Get
        /// <summary>
        /// Tests that <see cref="PageTypeRepository.Get" /> returns an empty list when
        /// the <see cref="IDb.PageTyps" /> is empty
        /// </summary>
        [Fact]
        public void Get_EmptySourceGivesEmptyList() {
            #region Arrange
            pageTypesList.Clear();
            SetupMockDbSet(mockPageTypeSet, PageTypes);
            #endregion
        
            #region Act
            IList<Extend.PageType> result = repository.Get();
            #endregion
        
            #region Assert
            Assert.Empty(result);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageTypeRepository.Get" /> returns the page types
        /// in title sorted order
        /// </summary>
        [Fact]
        public void Get_GivesCorrectList() {
            #region Arrange
            List<Extend.PageType> pageTypes = new List<Extend.PageType>();
            foreach (Data.PageType pageType in pageTypesList) {
                pageTypes.Add(JsonConvert.DeserializeObject<Extend.PageType>(pageType.Body));
            }
            pageTypes = pageTypes.OrderBy(t => t.Title).ToList();
            #endregion
        
            #region Act
            IList<Extend.PageType> result = repository.Get();
            #endregion
        
            #region Assert
            Assert.Equal(pageTypes.Count, result.Count);
            for (int i = 0; i < pageTypes.Count; i++) {
                AssertPageTypesMatch(pageTypes[i], result[i]);
            }
            #endregion
        }
        #endregion

        #region PageTypeRepository.Delete
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Delete_ValidObjectRemovesItem(int pageTypeIdAsInt) {
            #region Arrange
            string pageTypeId = $"PageType{pageTypeIdAsInt}";
            Data.PageType dataPageType = pageTypesList.FirstOrDefault(t => t.Id == pageTypeId);
            Extend.PageType extendPageType = JsonConvert.DeserializeObject<Extend.PageType>(dataPageType.Body);
            #endregion
        
            #region Act
            repository.Delete(extendPageType);
            #endregion
        
            #region Assert
            mockPageTypeSet.Verify(db => db.Remove(It.Is<Data.PageType>(
                t => t.Id == pageTypeId
            )), Times.Once());
            mockDb.Verify(db => db.SaveChanges(),Times.Once);
            #endregion
        }

        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGE_TYPES + 1)]
        public void Delete_ByIdWithInvalidIdDoesntCallRemove(int pageTypeIdAsInt) {
            #region Arrange
            string pageTypeId = $"PageType{pageTypeIdAsInt}";
            #endregion
        
            #region Act
            repository.Delete(pageTypeId);
            #endregion
        
            #region Assert
            mockPageTypeSet.Verify(db => db.Remove(It.IsAny<Data.PageType>()), Times.Never());
            mockDb.Verify(db => db.SaveChanges(), Times.Never());
            #endregion
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Delete_ByIdWithValidIdCallsRemove(int pageTypeIdAsInt) {
            #region Arrange
            string pageTypeId = $"PageType{pageTypeIdAsInt}";
            #endregion
        
            #region Act
            repository.Delete(pageTypeId);
            #endregion
        
            #region Assert
            mockPageTypeSet.Verify(db => db.Remove(It.Is<Data.PageType>(
                t => t.Id == pageTypeId
            )), Times.Once());
            mockDb.Verify(db => db.SaveChanges(), Times.Once());
            #endregion
        }
        #endregion

        #region PageTypeRepository.Save
        [Fact]
        public void Save_NonExistentTypeCreatesNewPageType() {
            #region Arrange
            Extend.PageType newPageType = new Extend.PageType {
                View = null,
                Id = "NewPageType",
                Title = "New Page Type",
                Regions = new List<Extend.RegionType> {
                    new Extend.RegionType {
                        Id = "Content",
                        Title = "Main Content",
                        Fields = new List<Extend.FieldType> {
                            new Extend.FieldType {
                                Type = "Html",
                            }
                        }
                    }
                },
            };
            #endregion

            #region Act
            repository.Save(newPageType);
            #endregion

            #region Assert
            mockPageTypeSet.Verify(db => db.Add(
                It.Is<Data.PageType>(t => t.Id == newPageType.Id)
            ), Times.Once());
            mockDb.Verify(db => db.SaveChanges(), Times.Once());
            #endregion
        }

        [Fact]
        public void Save_ExistingTypeUpdatesPageType() {
            #region Arrange
            Data.PageType pageTypeData = pageTypesList[0];
            Extend.PageType existingPageType = JsonConvert.DeserializeObject<Extend.PageType>(pageTypeData.Body);
            #endregion

            #region Act
            repository.Save(existingPageType);
            #endregion

            #region Assert
            mockPageTypeSet.Verify(db => db.Add(It.IsAny<Data.PageType>()), Times.Never());
            mockDb.Verify(db => db.SaveChanges(), Times.Once());
            #endregion
        }
        #endregion
        #endregion

        #region Helpers
        /// <summary>
        /// Asserts that the given page types are the same
        /// </summary>
        /// <param name="expected">The expected page type</param>
        /// <param name="actual">The actual page type</param>
        private void AssertPageTypesMatch(Extend.PageType expected, Extend.PageType actual) {
            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Route, actual.Route);
            Assert.Equal(expected.Title, actual.Title);
        }
        #endregion
    }
}