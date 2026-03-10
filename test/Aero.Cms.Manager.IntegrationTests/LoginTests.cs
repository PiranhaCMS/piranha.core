using Alba;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    [Fact]
    public async Task AdminUser_ShouldBeAbleToLogin()
    {
        // The admin user is seeded in ManagerTestBase.InitializeAsync
        // Default credentials from DefaultIdentitySeed: admin / password

        await Host.Scenario(_ =>
        {
            // Note: For Razor Pages, properties are typically prefixed with the class name or used directly.
            // In Login.cshtml.cs, it's [BindProperty] public InputModel Input { get; set; }
            // So the fields are Input.Username and Input.Password.
            
            _.Post.FormData(new Dictionary<string, string>
            {
                ["username"] = "admin",
                ["password"] = "password"
            }).ToUrl("/manager/login");

            // Successful login redirects to ~/manager/login/auth
            _.StatusCodeShouldBe(302);
            _.Header("Location").ShouldHaveValues("/manager/login/auth");
        });
    }
}
