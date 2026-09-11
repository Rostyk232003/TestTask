// PROMPT v2.2: Локальний JWT token endpoint

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ConferenceRoomBooking.API.Controllers;

/// <summary>
/// Видає demo JWT для локальної перевірки захищених API-операцій.
/// У production credentials мають надходити з окремого identity provider.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private readonly IConfiguration _configuration;

  public AuthController(IConfiguration configuration)
  {
    this._configuration = configuration;
  }

  /// <summary>
  /// Повертає JWT для demo admin credentials з конфігурації.
  /// </summary>
  [HttpPost("token")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public ActionResult<object> CreateToken([FromBody] TokenRequest request)
  {
    string configuredUsername = this._configuration["Jwt:DemoUsername"] ?? string.Empty;
    string configuredPassword = this._configuration["Jwt:DemoPassword"] ?? string.Empty;
    if (request.Username != configuredUsername || request.Password != configuredPassword)
    {
      return this.Unauthorized();
    }

    string issuer = this._configuration["Jwt:Issuer"] ?? "ConferenceRoomBooking.Api";
    string key = this._configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
    SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    List<Claim> claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, request.Username),
      new Claim(ClaimTypes.Role, "Admin"),
    };
    DateTime expiresAt = DateTime.UtcNow.AddHours(1);
    JwtSecurityToken token = new JwtSecurityToken(issuer, issuer, claims, expires: expiresAt, signingCredentials: credentials);
    return this.Ok(new { accessToken = new JwtSecurityTokenHandler().WriteToken(token), expiresAt });
  }
}

/// <summary>
/// Demo credentials для отримання локального токена.
/// </summary>
public class TokenRequest
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}