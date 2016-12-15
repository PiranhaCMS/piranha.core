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
using Microsoft.Extensions.Configuration;

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

        /// <summary>
        /// The mocked <see cref="IConfigurationRoot"/> passed to <see cref="App.Init(IApi, IConfigurationRoot, IModule[])"/>
        /// </summary>
        protected readonly Mock<IConfigurationRoot> mockConfig = new Mock<IConfigurationRoot>();
        #endregion
        #endregion

        #region Test initialize
        /// <summary>
        /// Default constructor/test initializer
        /// </summary>
        public ManagerAreaControllerUnitTestBase() {
            mockApi = SetupApi();
            mockConfig = SetupConfig();
            controller = SetupController();
            
            App.Init(mockApi.Object, mockConfig.Object, Modules);
            App.ReloadBlockTypes(mockApi.Object);
            App.ReloadPageTypes(mockApi.Object);
            AdditionalSetupAfterAppInit();
        }

        /// <summary>
        /// Creates the mocked API which is assigned to <see cref="mockApi" />
        /// </summary>
        /// <returns>
        /// The mocked API
        /// </returns>
        protected virtual Mock<IApi> SetupApi() {
            var api = new Mock<IApi>();
            api.Setup(a => a.PageTypes.Get()).Returns(new List<PageType>());
            api.Setup(a => a.BlockTypes.Get()).Returns(new List<BlockType>());
            return api;
        }

        /// <summary>
        /// Creates the mocked Config which is assigned to <see cref="mockConfig"/>
        /// </summary>
        /// <returns>
        /// The mocked config
        /// </returns>
        protected virtual Mock<IConfigurationRoot> SetupConfig()
        {
            Mock<IConfigurationRoot> config = new Mock<IConfigurationRoot>();
            Mock<IConfigurationSection> configSection = new Mock<IConfigurationSection>();
            config.Setup(c => c.GetSection(It.IsAny<string>())).Returns(configSection.Object);
            configSection.Setup(c => c.GetChildren()).Returns(new List<IConfigurationSection>());
            return config;
        }

        /// <summary>
        /// Creates the controller which is assigned to <see cref="controller" />
        /// </summary>
        /// <returns>
        /// The controller instance
        /// </returns>
        protected abstract TController SetupController();

        /// <summary>
        /// Hook which allows for additional initialization after Init method of <see cref="App" /> is called
        /// </summary>
        protected virtual void AdditionalSetupAfterAppInit() { /* Nothing to do in base */ }
        #endregion
    }
}