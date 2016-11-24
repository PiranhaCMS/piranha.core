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
using Piranha.EF.Repositories;

namespace Piranha.EF.Tests.Repositories
{
    public class PageRepositoryUnitTest : RepositoryUnitTestBase<PageRepository>
    {
        #region Properties
        private const int NUM_PAGE_TYPES = 3;
        private const int NUM_PAGES = 7;

        private IQueryable<Data.PageType> pageTypes;
        private IQueryable<Data.Page> pages;
        private readonly Mock<DbSet<Data.PageType>> mockPageTypeSet = new Mock<DbSet<Data.PageType>>();
        private readonly Mock<DbSet<Data.Page>> mockPagesSet = new Mock<DbSet<Data.Page>>();
        
        private readonly Mock<IDataService> mockDataService = new Mock<IDataService>();
        #endregion

        protected override void SetupMockDbData() {
            CreateMockPageTypes();
            CreateMockPages();
            mockDb.Setup(db => db.Pages).Returns(mockPagesSet.Object);
        }
        private void CreateMockPageTypes() {
            List<Data.PageType> listOfPageTypes = new List<Data.PageType>();
            for (int i = 0; i < NUM_PAGE_TYPES; i++) {
                listOfPageTypes.Add(new Data.PageType {

                });
            }
            pageTypes = listOfPageTypes.AsQueryable();
        }
        private void CreateMockPages() {
            List<Data.Page> listOfPages = new List<Data.Page>();
            for (int i = 0; i < NUM_PAGES; i++) {
                listOfPages.Add(new Data.Page {
                    Id = ConvertIntToGuid(i + 1),

                });
            }
            pages = listOfPages.AsQueryable();
        }

        protected override PageRepository SetupRepository() {
            return new PageRepository(mockDataService.Object, mockDb.Object);
        }
    }
}