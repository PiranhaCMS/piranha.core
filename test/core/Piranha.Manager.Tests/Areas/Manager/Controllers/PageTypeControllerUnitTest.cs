using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Areas.Manager.Controllers;

namespace Piranha.Manager.Tests.Areas.Manager.Controllers
{
    public class PageTypeControllerUnitTest : ManagerAreaControllerUnitTestBase<PageTypeController>
    {
        protected override PageTypeController SetupController() {
            return new PageTypeController(mockApi.Object);
        }
    }
}
