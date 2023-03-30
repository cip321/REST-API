using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using REST_API.Api.Entities;
using REST_API.Api.Repositories.Contracts;
using REST_API.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace REST_API.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration Config { get; }
        public IUserRepository UserRepository { get; }
        public AuthController(IConfiguration config, IUserRepository userRepository)
        {
            Config = config;
            UserRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Check if the username is already in use
            var existingUser = await UserRepository.GetUser(registerDto.Email);

            if (existingUser is not null)
            {
                return BadRequest("Email address is already in use");
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create a new user entity
            var newUser = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = hashedPassword,
                Role = "User"
            };

            // Add the user to the database
            await UserRepository.AddUser(newUser);

            return Ok("User registered successfully");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await Authenticate(loginDto);
            if (user is not null)
            {
                var token = GenerateToken(user);
                Response.Headers.Add("Authorization", "Bearer " + token);
                return Ok("Login successful");
            }
            return NotFound("User not found");
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Headers.Remove("Authorization");
            return Ok("User logged out");
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var token = new JwtSecurityToken(Config["Jwt:Issuer"],
                Config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<User?> Authenticate(LoginDto loginDto)
        {
            var currentUser = await UserRepository.GetUser(loginDto.Email);
            if (currentUser is not null)
            {
                return currentUser;
            }
            return null;
        }
    }
}
