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
        private const int NUM_PAGE_TYPES = 5;

        /// <summary>
        /// The mock page type data
        /// </summary>
        private List<PageType> pageTypes;
        #endregion
        #endregion

        #region Test setup
        protected override Mock<IApi> SetupApi() {
            var api = new Mock<IApi>();

            GeneratePageTypes();
            api.Setup(a => a.BlockTypes.Get()).Returns(new List<BlockType>());
            api.Setup(a => a.PageTypes.Get()).Returns(pageTypes);

            return api;
        }
        private void GeneratePageTypes() {
            pageTypes = new List<PageType>();
            for (int i = 1; i <= NUM_PAGE_TYPES; i++) {
                pageTypes.Add(new PageType {
                    Id = $"{i}",
                    Title = $"Page Type {i}"
                });
            }
        }

        protected override PageController SetupController() {
            return new PageController(mockApi.Object);
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
    }
}
