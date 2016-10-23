using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class PageController : Controller
    {
        [Route("manager/pages")]
        public IActionResult List()
        {
            return View();
        }
    }
}
