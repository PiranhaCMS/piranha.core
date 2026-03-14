

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Aero.Cms.AspNetCore.Identity.Models;
using Aero.Cms.Manager.Controllers;
using Marten;
using Microsoft.Extensions.Logging;


namespace Aero.Cms.AspNetCore.Identity.Controllers;

[Area("Manager")]
[AutoValidateAntiforgeryToken]
public class RoleController(IIdentityDb db, ILogger<RoleController> log) : ManagerController
{
    [HttpGet("/manager/roles")]
    [Authorize(Policy = Permissions.Roles)]
    public async Task<IActionResult> List()
    {
        var list = await RoleListModel.Get(db);
        
        return View(list);
    }

    [HttpGet("/manager/role/{id}")]
    [Authorize(Policy = Permissions.RolesEdit)]
    public async Task<IActionResult> Edit(string id)
    {
        var role = await RoleEditModel.GetById(db, id);
        
        return View("Edit", role);
    }

    [HttpGet("/manager/role")]
    [Authorize(Policy = Permissions.RolesAdd)]
    public async Task<IActionResult> Add()
    {
        var role = await RoleEditModel.Create();
        
        return View("Edit", role);
    }

    [HttpPost("/manager/role/save")]
    [Authorize(Policy = Permissions.RolesSave)]
    public async Task<IActionResult> Save(RoleEditModel model)
    {
        if (await model.Save(db))
        {
            SuccessMessage("The role has been saved.");
            return RedirectToAction("Edit", new { id = model.Role.Id });
        }

        ErrorMessage("The role could not be saved.", false);
        return View("Edit", model);
    }

    [HttpPost]
    [Route("/manager/role/delete")]
    [Authorize(Policy = Permissions.RolesDelete)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var role = await db.Roles
            .FirstOrDefaultAsync(r => r.Id == id, token: ct);

        if (role != null)
        {
            //_db.Roles.Remove(role);
            db.session.Delete(role);
            await db.SaveChangesAsync(ct);

            SuccessMessage("The role has been deleted.");
            return RedirectToAction("List");
        }

        ErrorMessage("The role could not be deleted.", false);
        return RedirectToAction("List");
    }
}
