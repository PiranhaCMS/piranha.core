using Microsoft.AspNetCore.Mvc;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("manager")]
    public class ManagerAreaControllerBase : Controller
    {
        #region Properties
        #region Protected Properties
        /// <summary>
        /// The current api
        /// </summary>
        protected readonly IApi _api;
        #endregion
        #endregion

        public ManagerAreaControllerBase(IApi api)
        {
            _api = api;
        }
    }
}