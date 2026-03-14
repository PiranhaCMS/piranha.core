




using Marten;

namespace Aero.Cms.AspNetCore.Identity.Models;

public class RoleListModel
{
    public List<ListItem> Roles { get; set; } = [];

    public static async Task<RoleListModel> Get(IIdentityDb db)
    {
        var roles = await db.Roles
            .OrderBy(r => r.Name)
            .Select(r => new ListItem
            {
                Id = r.Id,
                Name = r.Name,
                NormalizedName = r.NormalizedName
            }).ToListAsync();
         var model = new RoleListModel
        {
            Roles = roles.ToList()
        };

        foreach (var role in model.Roles)
        {
            role.UserCount = await db.UserRoles
                .CountAsync(r => r.RoleId == role.Id);
        }

        return model;
    }

    public class ListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public int UserCount { get; set; }
    }
}
