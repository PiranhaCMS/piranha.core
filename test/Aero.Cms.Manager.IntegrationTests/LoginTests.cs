using Alba;
using Xunit;

namespace Aero.Cms.Manager.IntegrationTests;

public class LoginTests : ManagerTestBase
{
    [Fact]
    public async Task LoginPage_ShouldBeAccessible()
    {
        await Host.Scenario(_ =>
        {
            _.Get.Url("/manager/login");
            _.StatusCodeShouldBeOk();
        });
    }
}
