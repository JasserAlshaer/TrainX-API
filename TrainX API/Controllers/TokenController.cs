using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TrainX_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly string _secretKey = "SuperSecretKeyForLocalDev123!"; // Replace with a secure and random key for production
        private readonly string _issuer = "https://localhost:44391";
        private readonly string _audience = "https://localhost:44391";

        // 1. Generate Token
        [HttpPost("generate")]
        public IActionResult GenerateToken([FromQuery] string username, [FromQuery] string[] roles)
        {
            var randomUserId = Guid.NewGuid().ToString();

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, randomUserId),
                new Claim("userId", randomUserId),
                new Claim(ClaimTypes.Role, string.Join(",", roles))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        // 2. Validate Token
        [HttpPost("validate")]
        public IActionResult ValidateToken([FromQuery] string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = creds.Key,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience
                }, out _);

                return Ok("Token is valid");
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid token: {ex.Message}");
            }
        }

        // 3. Decode Token
        [HttpPost("decode")]
        public IActionResult DecodeToken([FromQuery] string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var claims = jsonToken?.Claims.Select(c => new { c.Type, c.Value });
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to decode token: {ex.Message}");
            }
        }

        // 4. Refresh Token
        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromQuery] string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
                return BadRequest("Invalid token");

            var username = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var roles = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value?.Split(',');
            var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

            if (string.IsNullOrEmpty(username) || roles == null || roles.Length == 0)
                return BadRequest("Invalid token data");

            return GenerateToken(username, roles);
        }

        // 5. Check Authorization for Specific Role
        [HttpPost("authorize")]
        public IActionResult IsUserAuthorized([FromQuery] string token, [FromQuery] string requiredRole)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
                return BadRequest("Invalid token");

            var roles = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value?.Split(',');

            if (roles != null && roles.Contains(requiredRole))
                return Ok($"User has the required role: {requiredRole}");

            return Forbid("User is not authorized");
        }
    }
}
