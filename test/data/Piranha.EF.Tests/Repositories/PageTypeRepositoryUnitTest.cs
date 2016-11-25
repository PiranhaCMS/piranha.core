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
        protected override void SetupMockDbData()
        {
            CreateMockPageTypes();
            SetupMockDbSet(mockPageTypeSet, PageTypes);
            mockDb.Setup(db => db.PageTypes).Returns(mockPageTypeSet.Object);
        }
        /// <summary>
        /// Populates <see cref="pageTypesList" /> with fake data
        /// </summary>
        private void CreateMockPageTypes() {
            for (int i = 0; i < NUM_PAGE_TYPES; i++) {
                pageTypesList.Add(new Data.PageType {
                    Id = ConvertIntToGuid(i + 1).ToString(),
                    Body = "{\"View\":null,\"Id\":\"Html\",\"Title\":\"Html block\",\"Regions\":[{\"Id\":\"Content\",\"Title\":\"Main Content\",\"Collection\":false,\"Max\":0,\"Min\":0,\"Fields\":[{\"Id\":\"Default\",\"Title\":\"Default\",\"Type\":\"Html\"}]}]}",
                    Created = DateTime.Now.AddDays(-(i + 1)),
                    LastModified = DateTime.Now.AddDays(-(i + 1)),
                    Pages = new List<Data.Page>(),
                });
            }
        }

        protected override PageTypeRepository SetupRepository()
        {
            return new PageTypeRepository(mockDb.Object);
        }
        #endregion

        #region Unit tests
        #region PageTypeRepository.GetById
        /// <summary>
        /// Tests that <see cref="PageTypeRepository.GetById" /> returns null when its
        /// data source is empty
        /// </summary>
        /// <param name="pageTypeIdAsInt">The integer Id value to convert to a <see cref="Guid" /></param>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(NUM_PAGE_TYPES + 1)]
        public void GetByIdWithEmptySetGivesNull(int pageTypeIdAsInt) {
            #region Arrange
            pageTypesList.Clear();
            string pageTypeId = ConvertIntToGuid(pageTypeIdAsInt).ToString();
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
        /// <param name="pageTypeIdAsInt">The integer Id value to convert to a <see cref="Guid" /></param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should NOT be in the range
        /// [1, <see cref="NUM_PAGE_TYPES" />]
        /// </remarks>
        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGE_TYPES + 1)]
        public void GetByIdWithInvalidIdGivesNull(int pageTypeIdAsInt) {
            #region Arrange
            string pageTypeId = ConvertIntToGuid(pageTypeIdAsInt).ToString();
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
        public void GetByIdWithEmptyIdGivesNull(string pageTypeId) {
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
        /// <param name="pageTypeIdAsInt">The integer Id value to convert to a <see cref="Guid" /></param>
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
        public void GetByIdWithValidIdGivesProperPageType(int pageTypeIdAsInt) {
            #region Arrange
            string pageTypeId = ConvertIntToGuid(pageTypeIdAsInt).ToString();
            Data.PageType pageType = pageTypesList.FirstOrDefault(t => t.Id == pageTypeId);
            Extend.PageType expectedPageType = JsonConvert.DeserializeObject<Extend.PageType>(pageType.Body);
            #endregion
        
            #region Act
            Extend.PageType result = repository.GetById(pageTypeId);
            #endregion
        
            #region Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPageType.Id, result.Id);
            Assert.Equal(expectedPageType.Route, result.Route);
            Assert.Equal(expectedPageType.Title, result.Title);
            #endregion
        }
        #endregion
        #endregion

        #region Helpers
        #endregion
    }
}