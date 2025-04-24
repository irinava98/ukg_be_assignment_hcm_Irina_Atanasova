using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace People_Management.Services
{
   public class JWTService
   {
      private readonly string _key;
      private readonly string _issuer;

      public JWTService(IConfiguration configuration)
      {
         _key = configuration["Jwt:Key"]!;
         _issuer = configuration["Jwt:Issuer"]!;
      }

      public string GenerateToken(ClaimsPrincipal user)
      {
         var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty),
                new Claim(ClaimTypes.Role, user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty)
            };

         var departmentClaim = user.FindFirst("Department");
         if (departmentClaim != null)
         {
            claims.Add(new Claim("Department", departmentClaim.Value));
         }

         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

         var token = new JwtSecurityToken(
             issuer: _issuer,
             audience: _issuer,
             claims: claims,
             expires: DateTime.UtcNow.AddHours(2),
             signingCredentials: creds
         );

         return new JwtSecurityTokenHandler().WriteToken(token);
      }
   }
}