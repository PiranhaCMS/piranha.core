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
using Moq;
using Piranha.Areas.Manager.Controllers;
using Piranha.Extend;
using Xunit;

namespace Piranha.Manager.Tests.Areas.Manager.Controllers
{
    public class BlockTypeControllerUnitTest : ManagerAreaControllerUnitTestBase<BlockTypeController>
    {
        #region Properties
        #region Private Properties
        private const int NUM_BLOCK_TYPES = 5;

        private List<BlockType> blockTypes;
        #endregion
        #endregion

        #region Test setup
        protected override Mock<IApi> SetupApi() {
            var api = new Mock<IApi>();
            GenerateBlockTypes();
            api.Setup(a => a.BlockTypes.Get()).Returns(blockTypes);
            api.Setup(a => a.PageTypes.Get()).Returns(new List<PageType>());
            return api;
        }
        private void GenerateBlockTypes() {
            blockTypes = new List<BlockType>();
            for (int i = 1; i <= NUM_BLOCK_TYPES; i++) {
                blockTypes.Add(new BlockType {
                    Id = i.ToString(),
                    Title = "Block type " + i,
                });
            }
        }

        protected override BlockTypeController SetupController() {
            return new BlockTypeController(mockApi.Object);
        }
        #endregion

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
            AssertBlockTypeListsMatches(Model);
            #endregion
        }

        private void AssertBlockTypeListsMatches(IList<BlockType> result) {
            Assert.NotNull(result);
            Assert.Equal(blockTypes.Count, result.Count);
            for (int i = 0; i < blockTypes.Count; i++) {
                AssertBlockTypesMatch(blockTypes[i], result[i]);
            }
        }
        private void AssertBlockTypesMatch(BlockType expected, BlockType result)
        {
            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Title, result.Title);
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