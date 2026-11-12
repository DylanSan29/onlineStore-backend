using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class EmailVerificationService(OnlineStoreContext context)
{
    public async Task<string?> RequestAsync(string username)
    {
        var user = await context.Users.SingleOrDefaultAsync(item => item.Username == username);
        if (user is null) return null;
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        context.EmailVerificationTokens.Add(new EmailVerificationToken
        {
            UserId = user.Id, TokenHash = Hash(rawToken), ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
        await context.SaveChangesAsync();
        return rawToken;
    }

    public async Task<bool> ConfirmAsync(string rawToken)
    {
        var token = await context.EmailVerificationTokens
            .SingleOrDefaultAsync(item => item.TokenHash == Hash(rawToken));
        if (token is null || token.VerifiedAt != null || token.ExpiresAt <= DateTime.UtcNow) return false;
        token.VerifiedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
