using Microsoft.AspNetCore.Mvc;
using System;

namespace BasicWeb.Controllers
{
    /// <summary>
    /// This controller is only used when the project is first started
    /// and no pages has been added to the database. Feel free to remove it.
    /// </summary>
    public class SetupController : Controller
    {
        [Route("/")]
        public IActionResult Index() {
            return RedirectToRoute("~/");
        }
    }
}
