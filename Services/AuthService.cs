using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;
using OnlineStoreBackend.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;

namespace OnlineStoreBackend.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly OnlineStoreContext _context;

        public AuthService(IConfiguration configuration, OnlineStoreContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password");

            return GenerateJwtToken(user);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // Implement password verification logic here, like using a hash (e.g. bcrypt)
            return password == passwordHash;  // For simplicity, replace with actual hash verification
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Jwt:SecretKey is not configured in the application settings.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
