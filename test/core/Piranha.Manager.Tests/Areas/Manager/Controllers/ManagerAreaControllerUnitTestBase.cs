/*
 * Copyright (c) 2016 Billy Wolfington
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Areas.Manager.Controllers;
using Moq;
using Piranha.Extend;
using System.Collections.Generic;

namespace Piranha.Manager.Tests.Areas.Manager.Controllers
{
    /// <summary>
    /// Base class from which unit tests for controllers in <see cref="Piranha.Areas.Manager.Controllers" /> derive
    /// </summary>
    public abstract class ManagerAreaControllerUnitTestBase<TController>
        where TController : ManagerAreaControllerBase
    {
        #region Properties
        #region Protected Properties
        /// <summary>
        /// Array of <see cref="IModule" /> to pass to Init method of <see cref="App" />
        /// </summary>
        protected virtual IModule[] Modules {
            get {
                return null;
            }
        }

        /// <summary>
        /// The controller being tested
        /// </summary>
        protected readonly TController controller;

        /// <summary>
        /// The mocked <see cref="IApi" />
        /// </summary>
        protected readonly Mock<IApi> mockApi;
        #endregion
        #endregion

        #region Test initialize
        /// <summary>
        /// Default constructor/test initializer
        /// </summary>
        public ManagerAreaControllerUnitTestBase() {
            mockApi = SetupApi();
            controller = SetupController();
            App.Init(mockApi.Object, Modules);
            AdditionalSetupAfterAppInit();
        }

        /// <summary>
        /// Creates the mocked API which is assigned to <see cref="mockApi" />
        /// </summary>
        protected virtual Mock<IApi> SetupApi() {
            var api = new Mock<IApi>();
            api.Setup(a => a.PageTypes.Get()).Returns(new List<PageType>());
            api.Setup(a => a.BlockTypes.Get()).Returns(new List<BlockType>());
            return api;
        }

        /// <summary>
        /// Creates the controller which is assigned to <see cref="controller" />
        /// </summary>
        protected abstract TController SetupController();

        /// <summary>
        /// Hook which allows for additional initialization after Init method of <see cref="App" /> is called
        /// </summary>
        protected virtual void AdditionalSetupAfterAppInit() { /* Nothing to do in base */ }
        #endregion
    }
}