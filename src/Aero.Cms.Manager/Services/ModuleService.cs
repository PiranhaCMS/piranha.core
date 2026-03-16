

using Aero.Cms.Manager.Models;

namespace Aero.Cms.Manager.Services;

public class ModuleService
{
    /// <summary>
    /// Gets the list model.
    /// </summary>
    /// <returns>The list model</returns>
    public ModuleListModel GetList()
    {
        return new ModuleListModel
        {
            Items = App.Modules
                .OrderBy(m => m.Instance.Author)
                .ThenBy(m => m.Instance.Name)
                .Select(m => new ModuleListModel.ModuleItem
                {
                    Author = m.Instance.Author,
                    Name = m.Instance.Name,
                    Version = m.Instance.Version,
                    Description = m.Instance.Description,
                    PackageUrl = m.Instance.PackageUrl,
                    IconUrl = m.Instance.IconUrl
                }).ToList()
        };
    }
}
