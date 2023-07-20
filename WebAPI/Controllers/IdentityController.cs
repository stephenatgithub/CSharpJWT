using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace WebAPI.Controllers
{
    public class IdentityController : ControllerBase
    {
        private const string TokenSecret = "bJs3iqzDSP1qiTzWeMJa2cMsQFji2q6DL5exm0wVKo21NczRvpfE5m7oUE1VCp4F";
        private static readonly TimeSpan TokenLifeTime = TimeSpan.FromHours(8);

        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody]TokenGenerationRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenSecret);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, request.Email),
                new(JwtRegisteredClaimNames.Email, request.Email),
                new("userid", request.UserId.ToString()),
                new ("roles", "Admin"),
                new("roles", "Users"),
            };

            //foreach(var claimPair in request.CustomClaims)
            //{
            //    var jsonElemnt = (JsonElement)claimPair.Value;
            //    var valueType = jsonElemnt.ValueKind switch
            //    {
            //        JsonValueKind.True => ClaimValueTypes.Boolean,
            //        JsonValueKind.False => ClaimValueTypes.Boolean,
            //        JsonValueKind.Number => ClaimValueTypes.Double,
            //        _ => ClaimValueTypes.String
            //    };

            //    var claim = new Claim(claimPair.Key, claimPair.Value.ToString()!, valueType);
            //    claims.Add(claim);
            //};

            // Create SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TokenLifeTime),
                Issuer = "string",
                Audience = "string",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);
            return Ok(serializeToken);
        }
    }

    public class TokenGenerationRequest
    {
        public string Email { get; set; }
        public string UserId { get; set; }
        public CustomClaims CustomClaims { get; set; }
    }

    public class CustomClaims
    {
        public bool admin { get; set; }
    }
}
