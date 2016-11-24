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
using Microsoft.EntityFrameworkCore;
using Moq;
using Piranha.EF.Repositories;

namespace Piranha.EF.Tests.Repositories
{
    public class PageRepositoryUnitTest : RepositoryUnitTestBase<PageRepository>
    {
        #region Properties
        private const int NUM_PAGES = 7;

        private readonly List<Data.Page> pages = new List<Data.Page>();
        private readonly Mock<IDataService> mockDataService = new Mock<IDataService>();
        private readonly Mock<DbSet<Data.Page>> mockPagesSet = new Mock<DbSet<Data.Page>>();
        #endregion

        protected override void SetupMockDbData() {
            CreateMockPages();
            mockDb.Setup(db => db.Pages).Returns(mockPagesSet.Object);
        }
        private void CreateMockPages() {
            for (int i = 0; i < NUM_PAGES; i++) {
                pages.Add(new Data.Page {
                    Id = ConvertIntToGuid(i + 1),

                });
            }
        }

        protected override PageRepository SetupRepository() {
            return new PageRepository(mockDataService.Object, mockDb.Object);
        }
    }
}