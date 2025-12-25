using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using CouponMaster.Models;
using CouponMaster.Models.DTOs;

namespace CouponMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // 1. Check if user exists
                var checkSql = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
                var exists = await connection.ExecuteScalarAsync<bool>(checkSql, new { request.Username });

                if (exists) return BadRequest("Username already taken.");

                // 2. Hash Password (NEVER store plain text)
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // 3. Insert User
                var insertSql = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)";
                await connection.ExecuteAsync(insertSql, new { request.Username, PasswordHash = passwordHash });

                return Ok(new { message = "User registered successfully." });
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // 1. Find User
                var sql = "SELECT * FROM Users WHERE Username = @Username";
                var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { request.Username });

                // 2. Verify Password
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return Unauthorized("Invalid Username or Password.");
                }

                // 3. Generate Token
                var token = GenerateToken(user);
                return Ok(new { Token = token });
            }
        }

        private string GenerateToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role) 
                }),
                Expires = DateTime.UtcNow.AddHours(4), // Token valid for 4 hours
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}