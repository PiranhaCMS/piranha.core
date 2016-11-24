/*
 * Copyright (c) 2016 Billy Wolfington
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Piranha.Areas.Manager.Controllers;
using Piranha.Extend;
using Xunit;

namespace Piranha.Manager.Tests.Areas.Manager.Controllers
{
    /// <summary>
    /// Unit tests for <see cref="BlockTypeController" />
    /// </summary>
    public class BlockTypeControllerUnitTest : ManagerAreaControllerUnitTestBase<BlockTypeController>
    {
        #region Properties
        /// <summary>
        /// The number of sample block types to insert
        /// to <see cref="blockTypes" />
        /// </summary>
        private const int NUM_BLOCK_TYPES = 5;

        /// <summary>
        /// The mock block type data
        /// </summary>
        private List<BlockType> blockTypes;
        #endregion

        #region Test setup
        protected override Mock<IApi> SetupApi() {
            var api = new Mock<IApi>();
            InitializeBlockTypes();
            api.Setup(a => a.BlockTypes.Get()).Returns(blockTypes);
            api.Setup(a => a.PageTypes.Get()).Returns(new List<PageType>());
            return api;
        }
        /// <summary>
        /// Generates a list of block types to store in <see cref="blockTypes" />
        /// </summary>
        private void InitializeBlockTypes() {
            blockTypes = new List<BlockType>();
            for (int i = 1; i <= NUM_BLOCK_TYPES; i++) {
                blockTypes.Add(new BlockType {
                    Id = $"{i}",
                    Title = $"Block type {i}",
                });
            }
        }

        protected override BlockTypeController SetupController() {
            return new BlockTypeController(mockApi.Object);
        }
        #endregion

        /// <summary>
        /// Tests that <see cref="BlockTypeController.List" /> result model
        /// matches <see cref="blockTypes" />
        /// </summary>
        [Fact]
        public void ListResultGivesCorrectNumberBlockTypes() {
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
        /// <summary>
        /// Verifies that the list of block types matches <see cref="blockTypes" />
        /// </summary>
        /// <param name="result">The list of block types to verify</param>
        private void AssertBlockTypeListsMatches(IList<BlockType> result) {
            Assert.NotNull(result);
            Assert.Equal(blockTypes.Count, result.Count);
            for (int i = 0; i < blockTypes.Count; i++) {
                AssertBlockTypesMatch(blockTypes[i], result[i]);
            }
        }
        /// <summary>
        /// Verifies that the <see cref="BlockType.Id" /> and <see cref="BlockType.Title" />
        /// of the given block types match
        /// </summary>
        private void AssertBlockTypesMatch(BlockType expected, BlockType result) {
            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Title, result.Title);
        }

        /// <summary>
        /// Tests that <see cref="BlockTypeController.Edit" /> with an invalid block
        /// type Id returns a result with a null model
        /// </summary>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should NOT be in the range 
        /// [1, <see cref="NUM_BLOCK_TYPES" />]
        /// </remarks>
        [Theory]
        [InlineData("Bad-id")]
        [InlineData("7")]
        public void EditResultWithInvalidIdGivesNullModel(string blockTypeId) {
            #region Arrange
            #endregion

            #region Act
            ViewResult result = controller.Edit(blockTypeId);
            #endregion

            #region Assert
            Assert.NotNull(result);
            BlockType Model = result.Model as BlockType;
            Assert.Null(Model);
            #endregion
        }

        /// <summary>
        /// Tests that <see cref="BlockTypeController.Edit" /> with a valid block
        /// type Id returns a result with a <see cref="BlockType" /> model matching 
        /// the one in <see cref="blockTypes" />
        /// </summary>
        /// <remarks>
        /// <see cref="InlineDataAttribute" /> values should be in the range 
        /// [1, <see cref="NUM_BLOCK_TYPES" />]
        /// </remarks>
        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        [InlineData("6")]
        public void EditResultProvidesProperBlockTypeObject(string blockTypeId) {
            #region Arrange
            BlockType expectedBlockType = blockTypes.FirstOrDefault(b => b.Id == blockTypeId);
            #endregion

            #region Act
            ViewResult result = controller.Edit(blockTypeId);
            #endregion

            #region Assert
            Assert.NotNull(result);
            BlockType Model = result.Model as BlockType;
            AssertBlockTypesMatch(expectedBlockType, Model);
            #endregion
        }
    }
}