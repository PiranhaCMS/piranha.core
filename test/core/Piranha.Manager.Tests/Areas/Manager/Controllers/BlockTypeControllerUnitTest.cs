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
using Microsoft.AspNetCore.Mvc;
using Piranha.Areas.Manager.Controllers;
using Piranha.Extend;
using Xunit;

namespace Piranha.Manager.Tests.Areas.Manager.Controllers
{
    public class BlockTypeControllerUnitTest : ManagerAreaControllerUnitTestBase<BlockTypeController>
    {
        protected override BlockTypeController SetupController() {
            return new BlockTypeController(mockApi.Object);
        }

        [Fact]
        public void ListResultIsNotNull() {
            #region Arrange
            #endregion

            #region Act
            ViewResult result = controller.List();
            #endregion

            #region Assert
            Assert.NotNull(result);
            IList<BlockType> Model = result.Model as IList<BlockType>;
            Assert.NotNull(Model);
            #endregion
        }

        [Fact]
        public void EditResultWithEmptyApiProduces() {
            #region Arrange
            #endregion

            #region Act
            ViewResult result = controller.Edit("no-id");
            #endregion

            #region Assert
            Assert.NotNull(result);
            BlockType Model = result.Model as BlockType;
            Assert.Null(Model);
            #endregion
        }
    }
}