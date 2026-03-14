

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aero.Cms.WebApi;

[ApiController]
[Route("api/sitemap")]
public class SitemapApiController : Controller
{
    private readonly IApi _api;
    private readonly IAuthorizationService _auth;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="auth">The authorization service</param>
    public SitemapApiController(IApi api, IAuthorizationService auth)
    {
        _api = api;
        _auth = auth;
    }

    /// <summary>
    /// Gets the public sitemap for the default or specified site.
    /// </summary>
    /// <param name="id">The optional site id</param>
    /// <returns>The sitemap</returns>
    [HttpGet]
    [Route("{id}")]
    public virtual async Task<IActionResult> GetById(string? id = null)
    {
        if (!Module.AllowAnonymousAccess)
        {
            if (!(await _auth.AuthorizeAsync(User, Permissions.Sitemap)).Succeeded)
            {
                return Unauthorized();
            }
        }
        return Json(await _api.Sites.GetSitemapAsync(id));
    }
}
