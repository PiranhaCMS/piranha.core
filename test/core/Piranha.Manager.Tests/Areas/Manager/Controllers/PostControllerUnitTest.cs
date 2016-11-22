using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Areas.Manager.Controllers;

namespace Piranha.Manager.Tests.Areas.Manager.Controllers
{
    public class PostControllerUnitTest : ManagerAreaControllerUnitTestBase<PostController>
    {
        protected override PostController SetupController() {
            return new PostController(mockApi.Object);
        }
    }
}
