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
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Piranha.Areas.Manager.Controllers;
using Piranha.Areas.Manager.Models;
using Piranha.Extend;
using Piranha.Models;
using Xunit;

namespace Piranha.Manager.Tests.Areas.Manager.Controllers
{
    public class PageControllerUnitTest : ManagerAreaControllerUnitTestBase<PageController>
    {
        #region Properties
        #region Private Properties
        /// <summary>
        /// The number of sample page types to insert
        /// to <see href="pageTypes" />
        /// </summary>
        private const int NUM_PAGE_TYPES = 2;
        /// <summary>
        /// The number of sample pages to insert
        /// to <see href="pages" />
        /// </summary>
        private const int NUM_PAGES = 5;

        /// <summary>
        /// The mock page type data
        /// </summary>
        private List<Extend.PageType> pageTypes;
        /// <summary>
        /// The mock page data
        /// </summary>
        private List<DynamicPage> pages;
        #endregion
        #endregion

        #region Test setup
        protected override Mock<IApi> SetupApi() {
            var api = new Mock<IApi>();

            api.Setup(a => a.BlockTypes.Get()).Returns(new List<Extend.BlockType>());
            SetupPageTypeReporitoryMethods(api);

            return api;
        }
        private void SetupPageTypeReporitoryMethods(Mock<IApi> api) {
            GeneratePageTypes();
            api.Setup(a => a.PageTypes.Get()).Returns(pageTypes);
        }
        private void GeneratePageTypes() {
            pageTypes = new List<Extend.PageType>();
            for (int i = 1; i <= NUM_PAGE_TYPES; i++) {
                pageTypes.Add(new Extend.PageType {
                    Id = ConvertIntToGuid(i).ToString(),
                    Title = $"Page Type {i}",
                });
            }
        }
        private Guid ConvertIntToGuid(int value) {
            byte[] valueAsBytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(valueAsBytes, 0);
            return new Guid(valueAsBytes);
        }

        protected override IModule[] IncludedModules() {
            return new IModule[] {
                new Piranha.Manager.Module()
            };
        }

        protected override PageController SetupController() {
            return new PageController(mockApi.Object);
        }

        protected override void AdditionalSetupAfterAppInit() {
            SetupPageRepositoryMethods();
            mockApi.Setup(a => a.Sitemap.Get(It.IsAny<bool>())).Returns(
                (Func<bool, IList<SitemapItem>>)GetSitemapItems
            );
        }
        private void SetupPageRepositoryMethods() {
            GeneratePages();
            mockApi.Setup(a => a.Pages.GetById(It.IsAny<Guid>())).Returns(
                (Func<Guid, DynamicPage>)((id) => pages.FirstOrDefault(p => p.Id == id))
            );
            mockApi.Setup(a => a.Pages.GetStartpage()).Returns(pages.FirstOrDefault(p => p.IsStartPage));
        }
        private void GeneratePages() {
            pages = new List<DynamicPage>();
            for (int i = 1; i <= NUM_PAGES; i++) {
                int pageTypeId = (i % NUM_PAGE_TYPES) + 1;
                DynamicPage pageToAdd = Models.Page<DynamicPage>.Create(
                    ConvertIntToGuid(pageTypeId).ToString()
                );
                pageToAdd.Id = ConvertIntToGuid(i);
                pageToAdd.SortOrder = i - 1;
                pageToAdd.Title = $"Page {i}";
                pageToAdd.ParentId = null;
                pageToAdd.Published = DateTime.Now.AddHours(-(NUM_PAGES - i));
                pages.Add(pageToAdd);
            }
        }
        private IList<SitemapItem> GetSitemapItems(bool onlyPublished)
        {
            List<SitemapItem> items = new List<SitemapItem>();
            foreach (DynamicPage page in pages)
            {
                if (!onlyPublished || page.Published != null)
                {
                    SitemapItem itemToAdd = new SitemapItem
                    {
                        Id = page.Id,
                        Title = page.Title,
                        Published = page.Published,
                    };
                }
            }
            return items;
        }
        #endregion

        #region Unit tests
        [Fact]
        public void ListResultIsNotNullAndHasCorrectNumberPageTypes() {
            #region Arrange
            mockApi.Setup(a => a.Sitemap.Get(false)).Returns(new List<SitemapItem>());
            #endregion

            #region Act
            ViewResult result = controller.List();
            #endregion

            #region Assert
            Assert.NotNull(result);
            PageListModel Model = result.Model as PageListModel;
            Assert.NotNull(Model);
            Assert.Equal(pageTypes.Count, Model.PageTypes.Count);
            #endregion
        }

        #region PageController.Edit
        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGES + 1)]
        public void EditResultWithInvalidPageIdGivesThrowsException(int pageIdAsInt) {
            #region Arrange
            Guid invalidPageId = ConvertIntToGuid(pageIdAsInt);
            bool exceptionCaught = false;
            #endregion
        
            #region Act
            try {
                ViewResult result = controller.Edit(invalidPageId) as ViewResult;
            }
            catch (KeyNotFoundException e) {
                Assert.Equal($"No page found with the id '{invalidPageId}'", e.Message);
                exceptionCaught = true;
            }
            #endregion
        
            #region Assert
            Assert.True(exceptionCaught);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void EditResultWithValidPageIdGivesCorrectPageEditModel(int pageIdAsInt) {
            #region Arrange
            Guid pageId = ConvertIntToGuid(pageIdAsInt);
            DynamicPage page = pages.FirstOrDefault(p => p.Id == pageId);
            #endregion
        
            #region Act
            ViewResult result = controller.Edit(pageId) as ViewResult;
            #endregion
        
            #region Assert
            Assert.NotNull(result);
            PageEditModel Model = result.Model as PageEditModel;
            AssertPageEditModelMatchesPage(page, Model);
            #endregion
        }
        #endregion

        #region PageController.Add
        [Fact]
        public void AddResultWithNoPageTypesThrowsException() {
            #region Arrange
            pageTypes.Clear();
            App.PageTypes.Clear();
            #endregion

            #region Act
            AddResultWithInvalidPageTypeIdThrowsException(1);
            #endregion

            #region Assert
            #endregion
        }

        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGE_TYPES + 1)]
        public void AddResultWithInvalidPageTypeIdThrowsException(int pageTypeIdAsInt) {
            #region Arrange
            string pageTypeId = ConvertIntToGuid(pageTypeIdAsInt).ToString();
            bool exceptionCaught = false;
            #endregion
        
            #region Act
            try {
                ViewResult result = controller.Add(pageTypeId) as ViewResult;
            }
            catch (KeyNotFoundException e) {
                exceptionCaught = true;
                Assert.Equal($"No page type found with the id '{pageTypeId}'", e.Message);
            }
            #endregion
        
            #region Assert
            Assert.True(exceptionCaught);
            #endregion
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void AddResultWithValidPageTypeIdGivesCorrectPageEditModel(int pageTypeIdAsInt) {
            #region Arrange
            string pageTypeId = ConvertIntToGuid(pageTypeIdAsInt).ToString();
            PageType expectedPageType = pageTypes.FirstOrDefault(t => t.Id == pageTypeId);
            #endregion
        
            #region Act
            ViewResult result = controller.Add(pageTypeId) as ViewResult;
            #endregion
        
            #region Assert
            Assert.NotNull(result);
            PageEditModel Model = result.Model as PageEditModel;
            Assert.NotNull(Model);
            Assert.Equal(expectedPageType.Id, Model.PageType.Id);
            Assert.Equal(expectedPageType.Title, Model.PageType.Title);
            Assert.Equal(GetSitemapItems(false).Count, Model.SortOrder);
            #endregion
        }
        #endregion
        #endregion

        #region Helper methods
        private void AssertPageEditModelMatchesPage(DynamicPage page, PageEditModel Model) {
            Assert.NotNull(Model);
            Assert.Equal(page.Id, Model.Id);
            Assert.Equal(page.Title, Model.Title);
            Assert.Equal(page.ParentId, Model.ParentId);
            Assert.Equal(page.SortOrder, Model.SortOrder);
            Assert.Equal(((ExpandoObject)page.Regions).Count(), Model.Regions.Count);
        }
        #endregion
    }
}
