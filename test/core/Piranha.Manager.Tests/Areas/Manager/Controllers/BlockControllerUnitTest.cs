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