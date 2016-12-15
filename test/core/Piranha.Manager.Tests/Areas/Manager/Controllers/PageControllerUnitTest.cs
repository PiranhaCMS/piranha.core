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
    /// <summary>
    /// Unit tests for <see cref="PageController" />
    /// </summary>
    public class PageControllerUnitTest : ManagerAreaControllerUnitTestBase<PageController>
    {
        #region Properties
        #region Protected Properties
        protected override IModule[] Modules {
            get {
                return new IModule[] {
                    new Piranha.Manager.Module()
                };
            }
        }
        #endregion

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
        /// <summary>
        /// Initializes <see cref="pageTypes" /> and sets <see cref="IApi.PageTypes.Get" />
        /// return value
        /// </summary>
        private void SetupPageTypeReporitoryMethods(Mock<IApi> api) {
            InitializePageTypes();
            api.Setup(a => a.PageTypes.Get()).Returns(pageTypes);
        }
        /// <summary>
        /// Initializes <see cref="pageTypes" /> with <see cref="NUM_PAGE_TYPES" />
        /// page type objects
        /// </summary>
        private void InitializePageTypes() {
            pageTypes = new List<Extend.PageType>();
            for (int i = 1; i <= NUM_PAGE_TYPES; i++) {
                pageTypes.Add(new Extend.PageType {
                    Id = ConvertIntToGuid(i).ToString(),
                    Title = $"Page Type {i}",
                });
            }
        }
        /// <summary>
        /// Converts an integer value to a <see cref="Guid" />
        /// </summary>
        private Guid ConvertIntToGuid(int value) {
            byte[] valueAsBytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(valueAsBytes, 0);
            return new Guid(valueAsBytes);
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
        /// <summary>
        /// Initializes <see cref="pages" /> and sets the <see cref="mockApi.Pages.GetById" /> and
        /// <see cref="mockApi.Pages.GetStartpage" /> retun values
        /// </summary>
        private void SetupPageRepositoryMethods() {
            InitializePages();
            mockApi.Setup(a => a.Pages.GetById(It.IsAny<Guid>())).Returns(
                (Func<Guid, DynamicPage>)((id) => pages.FirstOrDefault(p => p.Id == id))
            );
            mockApi.Setup(a => a.Pages.GetStartpage()).Returns(pages.FirstOrDefault(p => p.IsStartPage));
        }
        /// <summary>
        /// Initializes <see cref="pages" /> with <see cref="NUM_PAGES" />
        /// page objects
        /// </summary>
        private void InitializePages() {
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
        /// <summary>
        /// Builds a list of sitemap items from <see cref="pages" />
        /// </summary>
        /// <param name="onlyPublished">Only get pages that are published</param>
        private IList<SitemapItem> GetSitemapItems(bool onlyPublished)
        {
            List<SitemapItem> items = new List<SitemapItem>();
            foreach (DynamicPage page in pages.Where(p => p.ParentId == null)) {
                if (!onlyPublished || page.Published != null) {
                    items.Add(ConvertDynamicPageToSitemapItem(page));
                }
            }
            return items;
        }
        /// <summary>
        /// Builds a sitemap item from a given page
        /// </summary>
        /// <param name="currentPage">The page to convert</param>
        private SitemapItem ConvertDynamicPageToSitemapItem(DynamicPage currentPage)
        {
            SitemapItem sitemapItem = new SitemapItem {
                Id = currentPage.Id,
                Title = currentPage.Title,
                Published = currentPage.Published,
                NavigationTitle = currentPage.NavigationTitle,
                Created = currentPage.Created,
                LastModified = currentPage.LastModified,
            };
            foreach (DynamicPage page in pages.Where(p => p.ParentId == currentPage.Id)) {
                sitemapItem.Items.Add(ConvertDynamicPageToSitemapItem(page));
            }
            return sitemapItem;
        }
        #endregion

        #region Unit tests
        /// <summary>
        /// Tests that <see cref="PageController.List" /> returns a result with
        /// a model containing the same number of page types as <see cref="pageTypes" />
        /// </summary>
        [Fact]
        public void ListIsNotNullAndHasCorrectNumberPageTypes() {
            #region Arrange
            #endregion

            #region Act
            ViewResult result = controller.List();
            #endregion

            #region Assert
            Assert.NotNull(result);
            PageListModel Model = result.Model as PageListModel;
            Assert.NotNull(Model);
            Assert.Equal(pageTypes.Count, Model.PageTypes.Count);
            Assert.Equal(pages.Count, Model.Sitemap.Count + Model.Sitemap.Select(i => i.Items.Count).Sum());
            #endregion
        }

        #region PageController.Edit
        /// <summary>
        /// Tests that <see cref="PageController.Edit" /> with an invalid page Id
        /// throws a <see cref="KeyNotFoundException" />
        /// </summary>
        /// <param name="pageIdAsInt">
        /// The integer Id of the page to convert to a <see cref="Guid" />
        /// </param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should NOT be in the range
        /// [1, <see cref="NUM_PAGES" />]
        /// </remarks>
        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGES + 1)]
        public void Edit_WithInvalidPageIdGivesThrowsException(int pageIdAsInt) {
            #region Arrange
            Guid invalidPageId = ConvertIntToGuid(pageIdAsInt);
            bool exceptionCaught = false;
            #endregion

            #region Act
            try {
                ViewResult result = controller.Edit(invalidPageId) as ViewResult;
            } catch (KeyNotFoundException e) {
                Assert.Equal($"No page found with the id '{invalidPageId}'", e.Message);
                exceptionCaught = true;
            }
            #endregion

            #region Assert
            Assert.True(exceptionCaught);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.Edit" /> with a valid page Id returns
        /// a result with a <see cref="PageEditModel" /> for the given page Id
        /// </summary>
        /// <param name="pageIdAsInt">
        /// The integer Id of the page to convert to a <see cref="Guid" />
        /// </param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should be in the range
        /// [1, <see cref="NUM_PAGES" />]
        /// </remarks>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Edit_WithValidPageIdGivesCorrectPageEditModel(int pageIdAsInt) {
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
        /// <summary>
        /// Tests that <see cref="PageController.Add" /> throws a <see cref="KeyNotFoundException" />
        /// when called while <see cref="App.PageTypes" /> is empty
        /// </summary>
        [Fact]
        public void Add_ResultWithNoPageTypesThrowsException() {
            #region Arrange
            pageTypes.Clear();
            App.ReloadPageTypes(mockApi.Object);
            #endregion

            #region Act
            AddWithInvalidPageTypeIdThrowsException(1);
            #endregion

            #region Assert
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.Add" /> with an invalid page type Id
        /// throws a <see cref="KeyNotFoundException" />
        /// </summary>
        /// <param name="pageTypeIdAsInt">
        /// The integer Id of the page type to convert to a <see cref="Guid" />
        /// </param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should NOT be in the range
        /// [1, <see cref="NUM_PAGE_TYPES" />]
        /// </remarks>
        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGE_TYPES + 1)]
        public void Add_WithInvalidPageTypeIdThrowsException(int pageTypeIdAsInt) {
            #region Arrange
            string pageTypeId = ConvertIntToGuid(pageTypeIdAsInt).ToString();
            bool exceptionCaught = false;
            #endregion

            #region Act
            try {
                ViewResult result = controller.Add(pageTypeId) as ViewResult;
            } catch (KeyNotFoundException e) {
                exceptionCaught = true;
                Assert.Equal($"No page type found with the id '{pageTypeId}'", e.Message);
            }
            #endregion

            #region Assert
            Assert.True(exceptionCaught);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.Add" /> with a valid page type Id
        /// returns a result with a new <see cref="PageEditModel" />
        /// </summary>
        /// <param name="pageTypeIdAsInt">
        /// The integer Id of the page type to convert to a <see cref="Guid" />
        /// </param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should be in the range
        /// [1, <see cref="NUM_PAGE_TYPES" />]
        /// </remarks>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Add_WithValidPageTypeIdGivesCorrectPageEditModel(int pageTypeIdAsInt) {
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

        #region PageController.Save
        /// <summary>
        /// Tests that <see cref="PageController.Save" /> with an invalid page type Id
        /// on a new page throws a <see cref="KeyNotFoundException" />
        /// </summary>
        /// <param name="pageTypeIdAsInt">
        /// The integer Id of the page type to conver to a <see cref="Guid" />
        /// </param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should NOT be in the range
        /// [1, <see cref="NUM_PAGE_TYPES" />]
        /// </remarks>
        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGE_TYPES + 1)]
        public void Save_NewPageWithInvalidPageTypeIdThrowsException(int pageTypeIdAsInt) {
            #region Arrange
            Guid pageTypeId = ConvertIntToGuid(pageTypeIdAsInt);
            PageEditModel pageToSave = PageEditModelForPageType(pageTypeId);
            bool exceptionCaught = false;
            #endregion

            #region Act
            try {
                IActionResult result = controller.Save(pageToSave);
            } catch (KeyNotFoundException e) {
                exceptionCaught = true;
                Assert.Equal($"No page type found with id '{pageTypeId}'", e.Message);
            }
            #endregion

            #region Assert
            Assert.True(exceptionCaught);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.Save" /> with a new <see cref="PageEditModel" />
        /// returns a <see cref="RedirectToActionResult" /> to the <see cref="PageController.List" /> method
        /// </summary>
        [Fact]
        public void Save_NewPageIsSuccessfulAndRedirectsToList() {
            #region Arrange
            int pageTypeIdAsInt = 1;
            PageEditModel pageToSave = PageEditModelForPageType(ConvertIntToGuid(pageTypeIdAsInt));
            #endregion

            #region Act
            RedirectToActionResult result = controller.Save(pageToSave) as RedirectToActionResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal("List", result.ActionName);
            mockApi.Verify(a => a.Pages.Save(
                    It.Is<DynamicPage>(p => p.Id == pageToSave.Id)
                ), Times.Once
            );
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.Save" /> updates the existing
        /// page in <see cref="pages" /> and returns a <see cref="RedirectToActionResult" />
        /// </summary>
        /// <param name="pageIdAsInt">
        /// The integer Id of the page to convert to a <see cref="Guid" />
        /// </param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should be in the range
        /// [1, <see cref="NUM_PAGES" />]
        /// </remarks>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Save_WithExistingPageUpdatesAndRedirects(int pageIdAsInt) {
            #region Arrange
            Guid pageId = ConvertIntToGuid(pageIdAsInt);
            string pageTitleUpdate = $"Updated title {pageIdAsInt}";
            DynamicPage page = pages.FirstOrDefault(p => p.Id == pageId);

            PageEditModel pageToSave = PageEditModelForPageType(new Guid(page.TypeId));
            DateTime? expectedPublishTime = pageToSave.Published;
            pageToSave.Id = pageId;
            pageToSave.Title = pageTitleUpdate;
            pageToSave.Published = expectedPublishTime;
            #endregion

            #region Act
            RedirectToActionResult result = controller.Save(pageToSave) as RedirectToActionResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal("List", result.ActionName);
            Assert.Equal(pageTitleUpdate, page.Title);
            Assert.Equal(expectedPublishTime, page.Published);
            mockApi.Verify(a => a.Pages.Save(It.Is<DynamicPage>(p => p.Id == pageId)), Times.Once);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.Save" /> returns the standard View method
        /// when <see cref="IApi.Pages.Save" /> throws some an exception
        /// </summary>
        [Fact]
        public void Save_NewPageWithFailedSaveReturnsView() {
            #region Arrange
            int pageTypeIdAsInt = 1;
            PageEditModel pageToSave = PageEditModelForPageType(ConvertIntToGuid(pageTypeIdAsInt));
            mockApi.Setup(a => a.Pages.Save(It.Is<DynamicPage>(p => p.Id == pageToSave.Id))).Throws(new Exception("DbUpdateConcurrencyException"));
            #endregion

            #region Act
            ViewResult result = controller.Save(pageToSave) as ViewResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal(pageToSave, result.Model);
            #endregion
        }
        #endregion

        #region PageController.Publish
        /// <summary>
        /// Tests that <see cref="PageController.Publish" /> with an invalid page type Id
        /// on a new page throws a <see cref="KeyNotFoundException" />
        /// </summary>
        /// <param name="pageTypeIdAsInt">
        /// The integer Id of the page type to conver to a <see cref="Guid" />
        /// </param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should NOT be in the range
        /// [1, <see cref="NUM_PAGE_TYPES" />]
        /// </remarks>
        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGE_TYPES + 1)]
        public void Publish_NewPageWithInvalidPageTypeIdThrowsKeyNotFoundException(int pageTypeIdAsInt) {
            #region Arrange
            Guid pageTypeId = ConvertIntToGuid(pageTypeIdAsInt);
            PageEditModel pageToPublish = PageEditModelForPageType(pageTypeId);
            bool exceptionCaught = false;
            #endregion

            #region Act
            try {
                IActionResult result = controller.Publish(pageToPublish);
            } catch (KeyNotFoundException e) {
                exceptionCaught = true;
                Assert.Equal($"No page type found with id '{pageTypeId}'", e.Message);
            }
            #endregion

            #region Assert
            Assert.True(exceptionCaught);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.Publish" /> with a new <see cref="PageEditModel" />
        /// returns a <see cref="RedirectToActionResult" /> to the <see cref="PageController.List" /> method
        /// </summary>
        [Fact]
        public void Publish_NewPageIsSuccessfulAndRedirectsToList() {
            #region Arrange
            int pageTypeIdAsInt = 1;
            PageEditModel pageToPublish = PageEditModelForPageType(ConvertIntToGuid(pageTypeIdAsInt));
            #endregion

            #region Act
            RedirectToActionResult result = controller.Publish(pageToPublish) as RedirectToActionResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal("List", result.ActionName);
            mockApi.Verify(a => a.Pages.Save(
                    It.Is<DynamicPage>(p => p.Id == pageToPublish.Id)
                ), Times.Once
            );
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.Publish" /> updates the existing
        /// page in <see cref="pages" /> and returns a <see cref="RedirectToActionResult" />
        /// </summary>
        /// <param name="pageIdAsInt">
        /// The integer Id of the page to convert to a <see cref="Guid" />
        /// </param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should be in the range
        /// [1, <see cref="NUM_PAGES" />]
        /// </remarks>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Publish_WithExistingPageUpdatesAndRedirects(int pageIdAsInt) {
            #region Arrange
            Guid pageId = ConvertIntToGuid(pageIdAsInt);
            string pageTitleUpdate = $"Updated title {pageIdAsInt}";
            DynamicPage page = pages.FirstOrDefault(p => p.Id == pageId);

            PageEditModel pageToPublish = PageEditModelForPageType(new Guid(page.TypeId));
            DateTime? originalPublishTime = pageToPublish.Published;
            pageToPublish.Id = pageId;
            pageToPublish.Title = pageTitleUpdate;
            pageToPublish.Published = originalPublishTime;
            #endregion

            #region Act
            RedirectToActionResult result = controller.Publish(pageToPublish) as RedirectToActionResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal("List", result.ActionName);
            Assert.Equal(pageTitleUpdate, page.Title);
            Assert.NotEqual(originalPublishTime, page.Published);
            mockApi.Verify(a => a.Pages.Save(It.Is<DynamicPage>(p => p.Id == pageId)), Times.Once);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.Publish" /> returns the standard View method
        /// when <see cref="IApi.Pages.Save" /> throws some an exception
        /// </summary>
        [Fact]
        public void Publish_NewPageWithFailedPublishReturnsView() {
            #region Arrange
            int pageTypeIdAsInt = 1;
            PageEditModel pageToPublish = PageEditModelForPageType(ConvertIntToGuid(pageTypeIdAsInt));
            mockApi.Setup(a => a.Pages.Save(It.Is<DynamicPage>(p => p.Id == pageToPublish.Id))).Throws(new Exception("DbUpdateConcurrencyException"));
            #endregion

            #region Act
            ViewResult result = controller.Publish(pageToPublish) as ViewResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal(pageToPublish, result.Model);
            #endregion
        }
        #endregion

        #region PageController.UnPublish
        /// <summary>
        /// Tests that <see cref="PageController.UnPublish" /> with an invalid page type Id
        /// on a new page throws a <see cref="KeyNotFoundException" />
        /// </summary>
        /// <param name="pageTypeIdAsInt">
        /// The integer Id of the page type to conver to a <see cref="Guid" />
        /// </param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should NOT be in the range
        /// [1, <see cref="NUM_PAGE_TYPES" />]
        /// </remarks>
        [Theory]
        [InlineData(0)]
        [InlineData(NUM_PAGE_TYPES + 1)]
        public void UnPublish_NewPageWithInvalidPageTypeIdThrowsKeyNotFoundException(int pageTypeIdAsInt) {
            #region Arrange
            Guid pageTypeId = ConvertIntToGuid(pageTypeIdAsInt);
            PageEditModel pageToPublish = PageEditModelForPageType(pageTypeId);
            bool exceptionCaught = false;
            #endregion

            #region Act
            try {
                IActionResult result = controller.UnPublish(pageToPublish);
            } catch (KeyNotFoundException e) {
                exceptionCaught = true;
                Assert.Equal($"No page type found with id '{pageTypeId}'", e.Message);
            }
            #endregion

            #region Assert
            Assert.True(exceptionCaught);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.UnPublish" /> with a new <see cref="PageEditModel" />
        /// returns a <see cref="RedirectToActionResult" /> to the <see cref="PageController.List" /> method
        /// </summary>
        [Fact]
        public void UnPublish_NewPageIsSuccessfulAndRedirectsToList() {
            #region Arrange
            int pageTypeIdAsInt = 1;
            PageEditModel pageToPublish = PageEditModelForPageType(ConvertIntToGuid(pageTypeIdAsInt));
            #endregion

            #region Act
            RedirectToActionResult result = controller.UnPublish(pageToPublish) as RedirectToActionResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal("List", result.ActionName);
            mockApi.Verify(a => a.Pages.Save(
                    It.Is<DynamicPage>(p => p.Id == pageToPublish.Id)
                ), Times.Once
            );
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.UnPublish" /> updates the existing
        /// page in <see cref="pages" /> and returns a <see cref="RedirectToActionResult" />
        /// </summary>
        /// <param name="pageIdAsInt">
        /// The integer Id of the page to convert to a <see cref="Guid" />
        /// </param>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should be in the range
        /// [1, <see cref="NUM_PAGES" />]
        /// </remarks>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void UnPublish_WithExistingPageUpdatesAndRedirects(int pageIdAsInt) {
            #region Arrange
            Guid pageId = ConvertIntToGuid(pageIdAsInt);
            string pageTitleUpdate = $"Updated title {pageIdAsInt}";
            DynamicPage page = pages.FirstOrDefault(p => p.Id == pageId);

            PageEditModel pageToPublish = PageEditModelForPageType(new Guid(page.TypeId));
            DateTime? originalPublishTime = pageToPublish.Published;
            pageToPublish.Id = pageId;
            pageToPublish.Title = pageTitleUpdate;
            pageToPublish.Published = originalPublishTime;
            #endregion

            #region Act
            RedirectToActionResult result = controller.UnPublish(pageToPublish) as RedirectToActionResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal("List", result.ActionName);
            Assert.Equal(pageTitleUpdate, page.Title);
            Assert.Null(page.Published);
            mockApi.Verify(a => a.Pages.Save(It.Is<DynamicPage>(p => p.Id == pageId)), Times.Once);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="PageController.UnPublish" /> returns the standard View method
        /// when <see cref="IApi.Pages.Save" /> throws some an exception
        /// </summary>
        [Fact]
        public void UnPublish_NewPageWithFailedPublishReturnsView() {
            #region Arrange
            int pageTypeIdAsInt = 1;
            PageEditModel pageToPublish = PageEditModelForPageType(ConvertIntToGuid(pageTypeIdAsInt));
            mockApi.Setup(a => a.Pages.Save(It.Is<DynamicPage>(p => p.Id == pageToPublish.Id))).Throws(new Exception("DbUpdateConcurrencyException"));
            #endregion

            #region Act
            ViewResult result = controller.UnPublish(pageToPublish) as ViewResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal(pageToPublish, result.Model);
            #endregion
        }
        #endregion

        /// <summary>
        /// Tests that <see cref="PageController.Delete" /> always results
        /// in a redirect to <see cref="PageController.List" />
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void Delete_PageAlwaysRedirectsToList(int pageIdAsInt) {
            #region Arrange
            Guid pageId = ConvertIntToGuid(pageIdAsInt);
            #endregion

            #region Act
            RedirectToActionResult result = controller.Delete(pageId) as RedirectToActionResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal("List", result.ActionName);
            #endregion
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Verifies that the provided <see cref="PageEditModel" /> is a reference to
        /// the given <see cref="DynamicPage" />
        /// </summary>
        /// <param name="page">The expected page</param>
        /// <param name="Model">The model to verify</param>
        private void AssertPageEditModelMatchesPage(DynamicPage page, PageEditModel Model) {
            Assert.NotNull(Model);
            Assert.Equal(page.Id, Model.Id);
            Assert.Equal(page.Title, Model.Title);
            Assert.Equal(page.ParentId, Model.ParentId);
            Assert.Equal(page.SortOrder, Model.SortOrder);
            Assert.Equal(((ExpandoObject)page.Regions).Count(), Model.Regions.Count);
        }

        /// <summary>
        /// Create a new <see cref="PageEditModel" /> for a given page type Id
        /// </summary>
        /// <param name="pageTypeId">The Id of the page type to base the model on</param>
        /// <returns>
        /// The new page model
        /// </returns>
        private PageEditModel PageEditModelForPageType(Guid pageTypeId) {
            return new PageEditModel {
                Id = ConvertIntToGuid(NUM_PAGES + 1),
                TypeId = pageTypeId.ToString(),
                PageType = pageTypes.FirstOrDefault(t => t.Id == pageTypeId.ToString())
            };
        }
        #endregion
    }
}
