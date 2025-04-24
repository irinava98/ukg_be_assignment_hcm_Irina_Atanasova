using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using People_Management.Services;
using System.Security.Claims;

namespace People_Management.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class AuthController : ControllerBase
   {
      private readonly ApplicationDbContext _context;
      private readonly JWTService _jwtService;

      public AuthController(ApplicationDbContext context, JWTService service)
      {
         _context = context;
         _jwtService = service;
      }

      [AllowAnonymous]
      [HttpPost("register")]
      public IActionResult Register([FromBody] RegisterRequest request)
      {
         if (_context.Users.Any(u => u.Username == request.Username))
         {
            return BadRequest("Username already exists.");
         }

         var person = _context.People.FirstOrDefault(p => p.Id == request.PersonId);
         if (person == null)
         {
            return BadRequest("Invalid PersonId — no such person exists.");
         }

         var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

         var user = new AppUser
         {
            Username = request.Username,
            Password = hashedPassword,
            Role = request.Role,
            PersonId = request.PersonId
         };

         _context.Users.Add(user);
         _context.SaveChanges();

         return Ok("User registered successfully.");
      }

      [AllowAnonymous]
      [HttpPost("login")]
      public IActionResult Login([FromBody] LoginRequest request)
      {
         var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

         if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
         {
            return Unauthorized("Invalid credentials.");
         }

         var person = _context.People.FirstOrDefault(p => p.Id == user.PersonId);

         var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.PersonId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

         if (!string.IsNullOrEmpty(person?.Department))
         {
            claims.Add(new Claim("Department", person.Department));
         }

         var identity = new ClaimsIdentity(claims, "Bearer");
         var principal = new ClaimsPrincipal(identity);

         var token = _jwtService.GenerateToken(principal);

         return Ok(new
         {
            token,
            role = user.Role,
            personId = user.PersonId,
            department = person?.Department,
            username = user.Username
         });
      }
   }
}