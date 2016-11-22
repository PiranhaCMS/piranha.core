using Piranha.Areas.Manager.Controllers;
using Moq;

namespace Piranha.Manager.Tests.Areas.Manager.Controllers
{
    public abstract class ManagerAreaControllerUnitTestBase<TController>
        where TController : ManagerAreaControllerBase
    {
        #region Properties
        #region Protected Properties
        protected readonly TController _controller;

        protected readonly Mock<IApi> _api;
        #endregion
        #endregion

        #region Test initialize
        public ManagerAreaControllerUnitTestBase()
        {
            _api = SetupApi();
            _controller = SetupController();
        }

        protected virtual Mock<IApi> SetupApi()
        {
            return new Mock<IApi>();
        }

        protected virtual TController SetupController()
        {
            return new ManagerAreaControllerBase(_api.Object) as TController;
        }
        #endregion
    }
}