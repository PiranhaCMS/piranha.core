/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Piranha.AspNetCore.Helpers;
using Piranha.Models;

namespace Piranha.AspNetCore.Services;

/// <summary>
/// The main application service. This service must be
/// registered as a scoped service as it contains information
/// about the current requst.
/// </summary>
public class ApplicationService : IApplicationService
{
    /// <summary>
    /// Gets the current api.
    /// </summary>
    public IApi Api { get; }

    /// <summary>
    /// Gets the site helper.
    /// </summary>
    public ISiteHelper Site { get; }

    /// <summary>
    /// Gets the media helper.
    /// </summary>
    public IMediaHelper Media { get; }

    /// <summary>
    /// Gets the request helper.
    /// </summary>
    public IRequestHelper Request { get; } = new RequestHelper();

    /// <summary>
    /// Gets/sets the currently requested URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets/sets the requested hostname
    /// </summary>
    public string Hostname { get; set; }

    /// <summary>
    /// Gets/sets the id of the currently requested page.
    /// </summary>
    public Guid PageId { get; set; }

    /// <summary>
    /// Gets/sets the current page.
    /// </summary>
    public PageBase CurrentPage { get; set; }

    /// <summary>
    /// Gets/sets the current post.
    /// </summary>
    public PostBase CurrentPost { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    public ApplicationService(IApi api)
    {
        Api = api;

        Site = new SiteHelper(api);
        Media = new MediaHelper(api);
    }

    /// <summary>
    /// Initializes the service.
    /// </summary>
    public async Task InitAsync(HttpContext context)
    {
        var hostname = context.Request.Host.Host;

        // Gets the current site info
        if (!context.Request.Path.Value.StartsWith("/manager/"))
        {
            Site site = null;

            // Try to get the requested site by hostname & prefix
            var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";
            if (!string.IsNullOrEmpty(url) && url.Length > 1)
            {
                var segments = url.Substring(1).Split(new char[] { '/' });
                var prefixedHostname = $"{context.Request.Host.Host}/{segments[0]}";
                site = await Api.Sites.GetByHostnameAsync(prefixedHostname);

                if (site != null)
                {
                    context.Request.Path = "/" + string.Join("/", segments.Skip(1));
                    hostname = prefixedHostname;

                }
            }

            // Try to get the requested site by hostname
            if (site == null)
                site = await Api.Sites.GetByHostnameAsync(context.Request.Host.Host);

            // If we didn't find the site, get the default site
            if (site == null)
                site = await Api.Sites.GetDefaultAsync();

            // Store the current site id & get the sitemap
            if (site != null)
            {
                var language = await Api.Languages.GetByIdAsync(site.LanguageId);

                Site.Id = site.Id;
                Site.LanguageId = site.LanguageId;
                Site.Culture = language?.Culture;
                Site.Sitemap = await Api.Sites.GetSitemapAsync(Site.Id);

                var siteHost = GetFirstHost(site);
                Site.Host = siteHost[0];
                Site.SitePrefix = siteHost[1];
            }
        }

        // Get the current url
        Url = context.Request.Path.Value;
        Hostname = hostname;
    }

    /// <summary>
    /// Gets the gravatar URL from the given parameters.
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="size">The requested size</param>
    /// <returns>The gravatar URL</returns>
    public string GetGravatarUrl(string email, int size = 0)
    {
        using (var md5 = MD5.Create())
        {
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(email));

            var sb = new StringBuilder(bytes.Length * 2);
            for (var n = 0; n < bytes.Length; n++)
            {
                sb.Append(bytes[n].ToString("X2"));
            }
            return "https://www.gravatar.com/avatar/" + sb.ToString().ToLower() +
                    (size > 0 ? "?s=" + size : "");
        }
    }

    /// <summary>
    /// Gets the first hostname of the site.
    /// </summary>
    /// <param name="site">The site</param>
    /// <returns>The hostname split into host and prefix</returns>
    private string[] GetFirstHost(Site site)
    {
        var result = new string[2];

        if (!string.IsNullOrEmpty(site.Hostnames))
        {
            foreach (var hostname in site.Hostnames.Split(","))
            {
                var segments = hostname.Split("/", StringSplitOptions.RemoveEmptyEntries);

                result[0] = segments[0];
                result[1] = segments.Length > 1 ? segments[1] : null;
            }
        }
        return result;
    }
}
