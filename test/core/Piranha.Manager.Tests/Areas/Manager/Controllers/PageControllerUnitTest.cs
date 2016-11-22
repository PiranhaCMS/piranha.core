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
using Piranha.Areas.Manager.Controllers;
using Piranha.Areas.Manager.Models;
using Piranha.Models;
using Xunit;

namespace Piranha.Manager.Tests.Areas.Manager.Controllers
{
    public class PageControllerUnitTest : ManagerAreaControllerUnitTestBase<PageController>
    {
        protected override PageController SetupController() {
            return new PageController(mockApi.Object);
        }

        [Fact]
        public void ListResultIsNotNull() {
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
            #endregion
        }
    }
}
