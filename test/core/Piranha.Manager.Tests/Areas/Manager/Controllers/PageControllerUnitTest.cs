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
        protected override PageController SetupController()
        {
            return new PageController(_api.Object);
        }

        [Fact]
        public void ListResultIsNotNull()
        {
            #region Arrange
            _api.Setup(a => a.Sitemap.Get(false)).Returns(new List<SitemapItem>());
            #endregion

            #region Act
            ViewResult result = _controller.List();
            #endregion

            #region Assert
            Assert.NotNull(result);
            PageListModel Model = result.Model as PageListModel;
            Assert.NotNull(Model);
            #endregion
        }
    }
}
