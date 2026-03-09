

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aero.Cms.Manager.Controllers;

[Area("Manager")]
[Route("manager/login/auth")]
[Authorize(Policy = Permission.Admin)]
[ApiController]
public sealed class AuthController : Controller
{
    private readonly IAntiforgery _antiForgery;
    private readonly ManagerOptions _options;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="antiforgery">The antiforgery service</param>
    /// <param name="options">The manager options</param>
    public AuthController(IAntiforgery antiforgery, IOptions<ManagerOptions> options)
    {
        _antiForgery = antiforgery;
        _options = options.Value;
    }

    [Route("{returnUrl?}")]
    [HttpGet]
    public IActionResult SetAuthCookie([FromQuery]string returnUrl = null)
    {
        var tokens = _antiForgery.GetAndStoreTokens(HttpContext);
        Response.Cookies.Append(_options.XsrfCookieName, tokens.RequestToken, new CookieOptions
        {
            HttpOnly = false,
            IsEssential = true
        });
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }
        return LocalRedirect("~/manager");
    }
}
