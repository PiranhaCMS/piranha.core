using Microsoft.AspNetCore.Mvc;
using ContentRus.TenantManagement.Models;
using ContentRus.TenantManagement.Services;
using Microsoft.AspNetCore.Authorization;

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

    [HttpPut("state")]
    public IActionResult UpdateTenantState([FromBody] TenantState newState)
    {
        var id = GetTenantIdFromClaims();
        var updated = _tenantService.UpdateTenantState(id, newState);
        return updated ? NoContent() : NotFound();
    }

    [HttpPut("tier")]
    public IActionResult UpdateTenantTier([FromBody] TenantTier newTier)
    {
        var id = GetTenantIdFromClaims();
        var updated = _tenantService.UpdateTenantTier(id, newTier);
        return updated ? NoContent() : NotFound();
    }

    [HttpPut("info")]
    public IActionResult UpdateTenantInfo([FromBody] TenantInfoDTO tenantInfo)
    {
        var id = GetTenantIdFromClaims();

        var updated = _tenantService.UpdateTenantInfo(id, tenantInfo);
        return updated ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpGet("")]
    public IActionResult GetTenant()
    {

        var tenantId = GetTenantIdFromClaims();
        var tenant = _tenantService.GetTenant(tenantId);

        return tenant is not null ? Ok(tenant) : NotFound();
    }

    // nao sei se este endpoint vai ser preciso na versao final
    [HttpGet("all")]
    public IActionResult GetAllTenants()
    {
        var tenants = _tenantService.GetAllTenants();
        return Ok(tenants);
    }

    private Guid GetTenantIdFromClaims()
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;
        if (tenantIdClaim == null)
            throw new UnauthorizedAccessException("TenantId not found in token.");
        Console.WriteLine($"TenantId from claims: {tenantIdClaim}");

        return Guid.Parse(tenantIdClaim);
    }
}
