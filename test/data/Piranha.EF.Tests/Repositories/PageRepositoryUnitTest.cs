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
using Xunit;

namespace Piranha.EF.Tests.Repositories
{
    public class PageRepositoryUnitTest : RepositoryUnitTestBase<PageRepository>
    {
        #region Properties
        private const int NUM_PAGE_TYPES = 3;
        private const int NUM_PAGES = 7;

        private IQueryable<Data.PageType> PageTypes {
            get {
                return pageTypesList.AsQueryable();
            }
        }
        private IQueryable<Data.Page> Pages {
            get {
                return pagesList.AsQueryable();
            }
        }

        private List<Data.PageType> pageTypesList = new List<Data.PageType>();
        private List<Data.Page> pagesList = new List<Data.Page>();
        private readonly Mock<DbSet<Data.PageType>> mockPageTypeSet = new Mock<DbSet<Data.PageType>>();
        private readonly Mock<DbSet<Data.Page>> mockPageSet = new Mock<DbSet<Data.Page>>();
        
        private readonly Mock<IDataService> mockDataService = new Mock<IDataService>();
        #endregion

        protected override void SetupMockDbData() {
            InitializeMockPageTypes();
            InitializeMockPages();
            mockDb.Setup(db => db.PageTypes).Returns(mockPageTypeSet.Object);
            mockDataService.Setup(db => db.PageTypes).Returns(new PageTypeRepository(mockDb.Object));
            mockDb.Setup(db => db.Pages).Returns(mockPageSet.Object);
        }
        private void InitializeMockPageTypes() {
            CreateMockPageTypes();
            SetupMockDbSet(mockPageTypeSet, PageTypes);
        }
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
        private void InitializeMockPages() {
            CreateMockPages();
            SetupMockDbSet(mockPageSet, Pages);
        }
        private void CreateMockPages() {
            for (int i = 0; i < NUM_PAGES; i++) {
                Data.PageType typeForNewPage = pageTypesList[i % NUM_PAGE_TYPES];
                Data.Page newPage = new Data.Page {
                    Id = ConvertIntToGuid(i + 1),
                    TypeId = typeForNewPage.Id,
                    ParentId = null,
                    SortOrder = i,
                    Title = $"Page Title {i}",
                    NavigationTitle = $"Nav title {i}",
                    Slug = $"page-title-{i}",
                    Published = DateTime.Now.AddDays(-(NUM_PAGES - i)),
                    LastModified = DateTime.Now.AddDays(-(NUM_PAGES - i)),
                    Type =  typeForNewPage,
                    Fields = new List<Data.PageField>(),
                };
                typeForNewPage.Pages.Add(newPage);
                pagesList.Add(newPage);
            }
        }

        protected override PageRepository SetupRepository() {
            return new PageRepository(mockDataService.Object, mockDb.Object);
        }
    }
}