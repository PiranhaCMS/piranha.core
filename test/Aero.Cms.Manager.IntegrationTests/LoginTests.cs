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

    [Fact]
    public async Task AdminUser_ShouldBeAbleToLogin()
    {
        // The admin user is seeded in ManagerTestBase.InitializeAsync
        // Default credentials from DefaultIdentitySeed: admin / password

        await Host.Scenario(_ =>
        {
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

    [Fact]
    public async Task AdminUser_ShouldHaveCorrectPermissions_AfterLogin()
    {
        // 1. Perform Login
        var loginResults = await Host.Scenario(_ =>
        {
            _.Post.FormData(new Dictionary<string, string>
            {
                ["username"] = "admin",
                ["password"] = "password"
            }).ToUrl("/manager/login");
            
            _.StatusCodeShouldBe(302);
        });

        // Extract auth cookie
        var identityCookies = new List<string>();
        if (loginResults.Context.Response.Headers.TryGetValue("Set-Cookie", out var setCookies))
        {
            foreach (var setCookie in setCookies)
            {
                if (setCookie.StartsWith(".AspNetCore.Identity.Application"))
                {
                    identityCookies.Add(setCookie.Split(';')[0]);
                }
            }
        }
        var cookieHeaderValue = string.Join("; ", identityCookies);

        // 2. Follow redirect to auth cookie setup
        var authResults = await Host.Scenario(_ =>
        {
            if (identityCookies.Count > 0)
            {
                _.WithRequestHeader("Cookie", cookieHeaderValue);
            }
            _.Get.Url("/manager/login/auth");
            _.StatusCodeShouldBe(302); // Redirects to /manager
        });

        // 3. Access manager dashboard and verify permissions
        var result = await Host.Scenario(_ =>
        {
            if (identityCookies.Count > 0)
            {
                _.WithRequestHeader("Cookie", cookieHeaderValue);
            }
            _.Get.Url("/manager/pages");
            _.StatusCodeShouldBeOk();
        });

        // Verify roles and claims on the current user
        Assert.True(result.Context.User.Identity?.IsAuthenticated ?? false, "User is not authenticated");
        Assert.True(result.Context.User.IsInRole("Admin") || result.Context.User.IsInRole("SysAdmin") || result.Context.User.IsInRole("SYSADMIN"), "User is not in Admin or SysAdmin role");
        
        Assert.True(result.Context.User.HasClaim(c => c.Type == Permission.Admin && c.Value == Permission.Admin), 
            $"User missing {Permission.Admin} claim.");
    }
}
