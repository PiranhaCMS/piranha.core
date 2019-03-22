using System;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Manager.Models;

namespace Piranha.Manager.Services
{
    public class AliasService
    {
        private readonly IApi _api;

        public AliasService(IApi api)
        {
            _api = api;
        }

        public async Task<AliasListModel> GetList(Guid? siteId = null)
        {
            if (!siteId.HasValue)
            {
                var site = await _api.Sites.GetDefaultAsync();
                siteId = site.Id;
            }

            var model = new AliasListModel
            {
                SiteId = siteId.Value
            };

            var aliases = await _api.Aliases.GetAllAsync(siteId.Value);

            model.Items = aliases.Select(a => new AliasListModel.ListItem
            {
                Id = a.Id,
                SiteId = a.SiteId,
                AliasUrl = a.AliasUrl,
                RedirectUrl = a.RedirectUrl,
                IsPermanent = a.Type == RedirectType.Permanent
            }).ToList();

            return model;
        }

        public async Task Save(AliasListModel.ListItem model)
        {
            await _api.Aliases.SaveAsync(new Alias
            {
                Id = model.Id.HasValue ? model.Id.Value : Guid.NewGuid(),
                SiteId = model.SiteId,
                AliasUrl = model.AliasUrl,
                RedirectUrl = model.RedirectUrl,
                Type = model.IsPermanent ? RedirectType.Permanent : RedirectType.Temporary
            });
        }

        public async Task<Alias> Delete(Guid id)
        {
            var alias = await _api.Aliases.GetByIdAsync(id);

            if (alias != null)
            {
                await _api.Aliases.DeleteAsync(alias);

                return alias;
            }
            return null;
        }
    }
}