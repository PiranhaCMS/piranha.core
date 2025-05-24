using Microsoft.AspNetCore.Mvc;
using ContentRus.TenantManagement.Models;
using ContentRus.TenantManagement.Services;
using ContentRus.TenantManagement.Configs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ContentRus.TenantManagement.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TenantService _tenantService;
    private readonly JwtSettings _jwtSettings;

    public UserController(UserService userService, TenantService tenantService, IOptions<JwtSettings> jwtOptions)
    {
        _userService = userService;
        _tenantService = tenantService;
        _jwtSettings = jwtOptions.Value;
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("TenantId", user.TenantId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [HttpPost("register")]
    public IActionResult RegisterUser([FromBody] AuthRequest authRequest)
    {
        var tenant = _tenantService.CreateTenant(authRequest.Email);
        var user = _userService.CreateUser(authRequest.Email, authRequest.Password, tenant.Id);

        var token = GenerateJwtToken(user);
        return Ok(new { token, user });
    }

    [HttpPost("login")]
    public IActionResult LoginUser([FromBody] AuthRequest authRequest)
    {
        var user = _userService.ValidateUserCredentials(authRequest.Email, authRequest.Password);
        if (user == null)
        {
            return Unauthorized();
        }

        var token = GenerateJwtToken(user);
        return Ok(new { token, user });
    }

    [HttpPut("{id:int}/password")]
    public IActionResult UpdateUserPassword(int id, [FromBody] string newPassword)
    {
        var updated = _userService.UpdateUserPassword(id, newPassword);
        return updated ? NoContent() : NotFound();
    }

    [HttpGet("{id:int}")]
    public IActionResult GetUser(int id)
    {
        var user = _userService.GetUser(id);
        return user is not null ? Ok(user) : NotFound();
    }

    // nao sei se este endpoint vai ser preciso na versao final
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _userService.GetAllUsers();
        return Ok(users);
    }
}
