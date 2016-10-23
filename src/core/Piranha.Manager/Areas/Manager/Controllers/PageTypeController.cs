using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class PageTypeController : Controller
    {
        private IApi api;

        public PageTypeController(IApi api) {
            this.api = api;
        }

        [Route("manager/pagetypes")]
        public IActionResult List() {
            return View(App.PageTypes);
        }
    }
}
