using System;
using Microsoft.AspNetCore.Mvc;
using Piranha.Areas.Manager.Controllers;
using Xunit;

namespace Piranha.Manager.Tests.Areas.Manager.Controllers
{
    public class BlockControllerUnitTest : ManagerAreaControllerUnitTestBase<BlockController>
    {
        protected override BlockController SetupController()
        {
            return new BlockController(_api.Object);
        }

        [Fact]
        public void ListViewResultIsNotNull()
        {
            #region Arrange
            #endregion

            #region Act
            ViewResult result = _controller.List() as ViewResult;
            #endregion

            #region Assert
            Assert.NotNull(result);
            #endregion
        }
    }
}