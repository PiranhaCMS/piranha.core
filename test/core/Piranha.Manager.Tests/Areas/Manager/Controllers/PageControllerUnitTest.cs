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
                pages.Add(pageToAdd);
            }
        }
        #endregion

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

        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGES + 1)]
        public void EditResultWithInvalidIdsGivesNullModel(int idAsInt) {
            #region Arrange
            Guid invalidPageId = ConvertIntToGuid(idAsInt);
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
        public void EditResultWithValidIdGivesCorrectPageEditModel(int idAsInt) {
            #region Arrange
            Guid pageId = ConvertIntToGuid(idAsInt);
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

        private void AssertPageEditModelMatchesPage(DynamicPage page, PageEditModel Model) {
            Assert.NotNull(Model);
            Assert.Equal(page.Id, Model.Id);
            Assert.Equal(page.Title, Model.Title);
            Assert.Equal(page.ParentId, Model.ParentId);
            Assert.Equal(page.SortOrder, Model.SortOrder);
            Assert.Equal(((ExpandoObject)page.Regions).Count(), Model.Regions.Count);
        }
    }
}
