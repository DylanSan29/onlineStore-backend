using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;
using OnlineStoreBackend.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        public async Task<(string Token, string RefreshToken, User User, DateTime AccessTokenExpiresAt)> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password");

            var tokenPair = CreateTokenPair(user);
            await _context.SaveChangesAsync();

            return (tokenPair.AccessToken, tokenPair.RefreshToken, user, tokenPair.AccessTokenExpiresAt);
        }

        public async Task<AuthTokenResponse> RefreshAsync(string refreshToken)
        {
            var token = await FindActiveRefreshTokenAsync(refreshToken);
            var user = await _context.Users.FindAsync(token.UserId)
                ?? throw new UnauthorizedAccessException("Refresh token is invalid.");

            token.RevokedAt = DateTime.UtcNow;
            var replacement = CreateTokenPair(user);
            await _context.SaveChangesAsync();
            return replacement;
        }

        public async Task RevokeRefreshTokenAsync(int userId, string refreshToken)
        {
            var tokenHash = HashRefreshToken(refreshToken);
            var token = await _context.RefreshTokens
                .SingleOrDefaultAsync(item => item.UserId == userId && item.TokenHash == tokenHash && item.RevokedAt == null);

            if (token != null)
            {
                token.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task Register(RegisterDto registerDto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username);

            if (existingUser != null)
                throw new InvalidOperationException("Username is already taken");

            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = HashPassword(registerDto.Password),
                Role = "client"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetResult> RequestPasswordResetAsync(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(item => item.Username == username);
            if (user is null) return new PasswordResetResult { UserFound = false };

            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
            _context.PasswordResetTokens.Add(new PasswordResetToken
            {
                UserId = user.Id, TokenHash = HashRefreshToken(rawToken), ExpiresAt = DateTime.UtcNow.AddHours(1)
            });
            await _context.SaveChangesAsync();
            return new PasswordResetResult { UserFound = true, Token = rawToken };
        }

        public async Task<bool> ResetPasswordAsync(string rawToken, string newPassword)
        {
            var token = await _context.PasswordResetTokens
                .SingleOrDefaultAsync(item => item.TokenHash == HashRefreshToken(rawToken));
            if (token is null || token.UsedAt != null || token.ExpiresAt <= DateTime.UtcNow) return false;

            var user = await _context.Users.FindAsync(token.UserId);
            if (user is null) return false;
            user.PasswordHash = HashPassword(newPassword);
            token.UsedAt = DateTime.UtcNow;
            var otherTokens = await _context.PasswordResetTokens
                .Where(item => item.UserId == user.Id && item.UsedAt == null && item.Id != token.Id).ToListAsync();
            foreach (var otherToken in otherTokens) otherToken.UsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        private AuthTokenResponse CreateTokenPair(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Jwt:SecretKey is not configured in the application settings.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(GetAccessTokenMinutes());
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: accessTokenExpiresAt,
                signingCredentials: creds);

            var rawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = HashRefreshToken(rawRefreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenDays())
            });

            return new AuthTokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = rawRefreshToken,
                AccessTokenExpiresAt = accessTokenExpiresAt
            };
        }

        private async Task<RefreshToken> FindActiveRefreshTokenAsync(string rawRefreshToken)
        {
            if (string.IsNullOrWhiteSpace(rawRefreshToken))
                throw new UnauthorizedAccessException("Refresh token is invalid.");

            var token = await _context.RefreshTokens
                .SingleOrDefaultAsync(item => item.TokenHash == HashRefreshToken(rawRefreshToken));

            if (token == null || token.RevokedAt != null || token.ExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token is invalid or expired.");

            return token;
        }

        private static string HashRefreshToken(string rawRefreshToken)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawRefreshToken));
            return Convert.ToHexString(hash);
        }

        private int GetAccessTokenMinutes() =>
            _configuration.GetValue("Jwt:AccessTokenMinutes", 15);

        private int GetRefreshTokenDays() =>
            _configuration.GetValue("Jwt:RefreshTokenDays", 7);
    }
}
