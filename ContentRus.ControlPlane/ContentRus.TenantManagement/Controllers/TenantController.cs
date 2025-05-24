using Microsoft.AspNetCore.Mvc;
using ContentRus.TenantManagement.Models;
using ContentRus.TenantManagement.Services;

namespace ContentRus.TenantManagement.Controllers;

[ApiController]
[Route("api/tenant")]
public class TenantController : ControllerBase
{
    private readonly TenantService _tenantService;

    public TenantController(TenantService tenantService)
    {
        _tenantService = tenantService;
    }

    /*
    [HttpPost]
    public IActionResult CreateTenant([FromBody] TenantDTO request)
    {
        var tenant = _tenantService.CreateTenant(request.Email);
        return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
    }
    */

    [HttpPut("{id:guid}/state")]
    public IActionResult UpdateTenantState(Guid id, [FromBody] TenantState newState)
    {
        var updated = _tenantService.UpdateTenantState(id, newState);
        return updated ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/tier")]
    public IActionResult UpdateTenantTier(Guid id, [FromBody] TenantTier newTier)
    {
        var updated = _tenantService.UpdateTenantTier(id, newTier);
        return updated ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetTenant(Guid id)
    {
        var tenant = _tenantService.GetTenant(id);
        return tenant is not null ? Ok(tenant) : NotFound();
    }

    // nao sei se este endpoint vai ser preciso na versao final
    [HttpGet]
    public IActionResult GetAllTenants()
    {
        var tenants = _tenantService.GetAllTenants();
        return Ok(tenants);
    }
}
