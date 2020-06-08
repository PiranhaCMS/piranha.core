using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha;
using Piranha.Extend.Fields;
using Piranha.Models;

/// <summary>
/// Example data item for a DataSelectListField that can be used
/// to select pages. This has to be it's own class so that the static
/// metods GetById() and GetList() can be defined.
/// </summary>
public class PageItem
{
    /// <summary>
    /// Gets/sets the page id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets/sets the related page model.
    /// </summary>
    public PageInfo Model { get; set; }

    /// <summary>
    /// Gets the page item from thhe given id.
    /// </summary>
    /// <param name="id">The selected id. Has to be a string</param>
    /// <param name="api">The curret api, injected parameter</param>
    /// <returns>The item</returns>
    static async Task<PageItem> GetById(string id, IApi api)
    {
        return new PageItem
        {
            Id = new Guid(id),
            Model = await api.Pages.GetByIdAsync<PageInfo>(new Guid(id))
        };
    }

    /// <summary>
    /// Gets the list of selected items to choose from. This is used in
    /// the manager interface.
    /// </summary>
    /// <param name="api">The current api, injected parameter</param>
    /// <returns>The available items</returns>
    static async Task<IEnumerable<DataSelectFieldItem>> GetList(IApi api)
    {
        var pages = await api.Pages.GetAllAsync();

        return pages.Select(p => new DataSelectFieldItem
        {
            Id = p.Id.ToString(),
            Name = p.Title
        });
    }
}